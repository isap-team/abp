using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Isap.Converters;

namespace Isap.CommonCore.Accessors
{
	public interface IPropertyAccessor
	{
		string PropertyName { get; }
		Type GetPropertyType();
		object GetValue(object target);
		bool SetValue(object target, object value);
	}

	public interface IPropertyAccessor<in TEntity>: IPropertyAccessor
	{
		object GetValue(TEntity target);
		bool SetValue(TEntity target, object value);
	}

	public interface IEntityAccessor
	{
		IPropertyAccessor TryGetProperty(string memberName);
		IPropertyAccessor GetProperty(string memberName);
		object GetValue(object target, string memberName);
		void SetValue(object target, string memberName, object value);
		bool TrySetValue(object target, string memberName, object value);
	}

	public interface IEntityAccessor<in TEntity>: IEntityAccessor
	{
		new IPropertyAccessor<TEntity> TryGetProperty(string memberName);
		new IPropertyAccessor<TEntity> GetProperty(string memberName);
		object GetValue(TEntity target, string memberName);
		void SetValue(TEntity target, string memberName, object value);
		bool TrySetValue(TEntity target, string memberName, object value);
	}

	public class EntityAccessor<TEntity>: IEntityAccessor<TEntity>
	{
		private class PropertyAccessor: IPropertyAccessor<TEntity>
		{
			private readonly PropertyInfo _property;
			private readonly IValueConverter _converter;

			public PropertyAccessor(PropertyInfo property, IValueConverter converter)
			{
				_property = property;
				_converter = converter;
			}

			public string PropertyName => _property.Name;

			public Type GetPropertyType() => _property.PropertyType;

			object IPropertyAccessor.GetValue(object target) => GetValue((TEntity) target);

			bool IPropertyAccessor.SetValue(object target, object value) => SetValue((TEntity) target, value);

			public object GetValue(TEntity target)
			{
				return _property.GetValue(target);
			}

			public bool SetValue(TEntity target, object value)
			{
				if (!_property.CanWrite)
					return false;
				value = _converter.ConvertTo(_property.PropertyType, value);
				_property.SetValue(target, value);
				return true;
			}
		}

		private class FieldAccessor: IPropertyAccessor<TEntity>
		{
			private readonly FieldInfo _property;
			private readonly IValueConverter _converter;

			public FieldAccessor(FieldInfo property, IValueConverter converter)
			{
				_property = property;
				_converter = converter;
			}

			public string PropertyName => _property.Name;

			public Type GetPropertyType() => _property.FieldType;

			object IPropertyAccessor.GetValue(object target) => GetValue((TEntity) target);

			bool IPropertyAccessor.SetValue(object target, object value) => SetValue((TEntity) target, value);

			public object GetValue(TEntity target)
			{
				return _property.GetValue(target);
			}

			public bool SetValue(TEntity target, object value)
			{
				value = _converter.ConvertTo(_property.FieldType, value);
				_property.SetValue(target, value);
				return true;
			}
		}

		private readonly IValueConverter _converter;

		private readonly Dictionary<string, IPropertyAccessor<TEntity>> _memberMap;

		public EntityAccessor(IValueConverter converter)
		{
			_converter = converter;
			_memberMap = typeof(TEntity).GetMembers(BindingFlags.Instance | BindingFlags.Public)
				.Select(CreateAccessor)
				.Where(accessor => accessor != null)
				.ToDictionary(accessor => accessor.PropertyName, StringComparer.OrdinalIgnoreCase);
		}

		IPropertyAccessor IEntityAccessor.TryGetProperty(string memberName)
		{
			return TryGetProperty(memberName);
		}

		public IPropertyAccessor<TEntity> TryGetProperty(string memberName)
		{
			if (!_memberMap.TryGetValue(memberName, out var accessor))
				return null;
			return accessor;
		}

		IPropertyAccessor IEntityAccessor.GetProperty(string memberName)
		{
			return GetProperty(memberName);
		}

		public IPropertyAccessor<TEntity> GetProperty(string memberName)
		{
			if (!_memberMap.TryGetValue(memberName, out var accessor))
				throw new InvalidOperationException();
			return accessor;
		}

		object IEntityAccessor.GetValue(object target, string memberName)
		{
			return GetValue((TEntity) target, memberName);
		}

		void IEntityAccessor.SetValue(object target, string memberName, object value)
		{
			SetValue((TEntity) target, memberName, value);
		}

		bool IEntityAccessor.TrySetValue(object target, string memberName, object value)
		{
			return TrySetValue((TEntity) target, memberName, value);
		}

		public object GetValue(TEntity target, string memberName)
		{
			if (!_memberMap.TryGetValue(memberName, out var accessor))
				throw new InvalidOperationException();
			return accessor.GetValue(target);
		}

		public void SetValue(TEntity target, string memberName, object value)
		{
			if (!TrySetValue(target, memberName, value))
				throw new InvalidOperationException($"Couldn't find writable property or field with name '{memberName}' in type '{typeof(TEntity).FullName}'.");
		}

		public bool TrySetValue(TEntity target, string memberName, object value)
		{
			if (!_memberMap.TryGetValue(memberName, out var accessor))
				return false;
			return accessor.SetValue(target, value);
		}

		private IPropertyAccessor<TEntity> CreateAccessor(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					return new FieldAccessor((FieldInfo) member, _converter);
				case MemberTypes.Property:
					return new PropertyAccessor((PropertyInfo) member, _converter);
				default:
					return null;
			}
		}
	}

	public static class EntityAccessors
	{
		private static readonly IValueConverter _converter = ValueConverterProviders.Default.GetConverter();
		private static readonly ConcurrentDictionary<Type, IEntityAccessor> _accessorMap = new ConcurrentDictionary<Type, IEntityAccessor>();

		public static IEntityAccessor<TEntity> Get<TEntity>()
		{
			return (IEntityAccessor<TEntity>) _accessorMap.GetOrAdd(typeof(TEntity), _ => new EntityAccessor<TEntity>(_converter));
		}

		public static IEntityAccessor Get(Type entityType)
		{
			return _accessorMap.GetOrAdd(entityType,
				_ => (IEntityAccessor) Activator.CreateInstance(typeof(EntityAccessor<>).MakeGenericType(entityType), _converter));
		}
	}
}
