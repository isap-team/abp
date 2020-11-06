using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Isap.Converters;
using JetBrains.Annotations;

namespace Isap.CommonCore.ModelBinding
{
	public class BindingContext: IValueAccessor
	{
		private readonly IValueAccessor _modelAccessor;

		private BindingContext(Type modelType, IValueAccessor modelAccessor)
		{
			_modelAccessor = modelAccessor;
			ModelType = modelType;
			Properties = GetProperties(ModelType);
		}

		public IValueConverter Converter => _modelAccessor.Converter;
		public Type ModelType { get; }
		public Dictionary<string, PropertyDescriptor> Properties { get; }

		public object Model
		{
			get => _modelAccessor.GetValue();
			set => _modelAccessor.SetValue(value);
		}

		Type IValueAccessor.ValueType => _modelAccessor.ValueType;

		object IValueAccessor.GetValue()
		{
			return _modelAccessor.GetValue();
		}

		void IValueAccessor.SetValue(object value)
		{
			_modelAccessor.SetValue(value);
		}

		public object EnsureValue()
		{
			return _modelAccessor.EnsureValue();
		}

		private static Dictionary<string, PropertyDescriptor> GetProperties(Type modelType)
		{
			IEnumerable<FormPropertyAttribute> GetFormProps(PropertyDescriptor descriptor)
			{
				List<FormPropertyAttribute> attributes = descriptor.Attributes.OfType<FormPropertyAttribute>().ToList();
				return attributes.Any()
						? attributes
						: Enumerable.Repeat(new FormPropertyAttribute(descriptor.Name), 1)
					;
			}

			return TypeDescriptor.GetProperties(modelType)
				.Cast<PropertyDescriptor>()
				.Where(i => !i.Attributes.OfType<FormIgnoreAttribute>().Any())
				.SelectMany(i => GetFormProps(i).Select(p => Tuple.Create(p, i)))
				.ToDictionary(i => i.Item1.PropertyName, i => i.Item2);
		}

		public void EnsureModel()
		{
			if (Model == null)
				Model = Activator.CreateInstance(ModelType);
		}

		public static BindingContext Create([NotNull] IValueConverter converter, [NotNull] object model)
		{
			if (converter == null) throw new ArgumentNullException(nameof(converter));
			if (model == null) throw new ArgumentNullException(nameof(model));
			return new BindingContext(model.GetType(), new BasicValueAccessor(converter, model.GetType(), model));
		}

		public static BindingContext Create([NotNull] BindingContext parent, [NotNull] string modelPropertyName)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			if (modelPropertyName == null) throw new ArgumentNullException(nameof(modelPropertyName));

			if (int.TryParse(modelPropertyName, out int index))
			{
				if (typeof(IEnumerable).IsAssignableFrom(parent.ModelType))
				{
					if (parent.ModelType.IsGenericType)
					{
						Type typeDef = parent.ModelType.GetGenericTypeDefinition();
						if (typeDef == typeof(List<>))
						{
							Type elementType = parent.ModelType.GetGenericArguments()[0];
							IValueAccessor valueAccessor = ListElementAccessor.Create(elementType, parent, index);
							return new BindingContext(elementType, valueAccessor);
						}
					}
				}
			}
			else if (parent.Properties.TryGetValue(modelPropertyName, out var property))
				return new BindingContext(property.PropertyType, new PropertyValueAccessor(parent, property));

			return new BindingContext(typeof(object), new BasicValueAccessor(parent.Converter, typeof(object), new object()));
		}
	}
}
