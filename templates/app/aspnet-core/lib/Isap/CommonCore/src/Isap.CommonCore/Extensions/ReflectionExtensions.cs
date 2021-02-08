using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Isap.Converters.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Isap.CommonCore.Extensions
{
	public static class ReflectionExtensions
	{
		public static Dictionary<string, object> Assign(this Dictionary<string, object> map, object newValues,
			bool deep = false, bool ignoreDefaults = false, Action<AssignMemberState> onAssignMember = null)
		{
			if (newValues != null)
			{
				Type type = newValues.GetType();
				TypeInfo typeInfo = type.GetTypeInfo();
				foreach (PropertyInfo member in typeInfo.GetProperties())
				{
					object value = member.GetValue(newValues);
					map.Assign(member, member.PropertyType, value, deep, ignoreDefaults, onAssignMember);
				}

				foreach (FieldInfo member in typeInfo.GetFields())
				{
					object value = member.GetValue(newValues);
					map.Assign(member, member.FieldType, value, deep, ignoreDefaults, onAssignMember);
				}
			}

			return map;
		}

		internal static Dictionary<string, object> Assign(this Dictionary<string, object> map, MemberInfo member, Type valueType, object value,
			bool deep = false, bool ignoreDefaults = false, Action<AssignMemberState> onAssignMember = null)
		{
			bool ignore = ignoreDefaults && Equals(valueType.GetDefaultValue(), value) || member.GetCustomAttribute<JsonIgnoreAttribute>() != null;
			var attr = member.GetCustomAttribute<JsonPropertyAttribute>();
			string name = attr?.PropertyName ?? member.Name;
			if (deep)
				value = AsNameToObjectMapTree(value, ignoreDefaults, map.Comparer, onAssignMember);

			onAssignMember = onAssignMember ?? (dummy =>
				{
				});
			AssignMemberState state = new AssignMemberState(member, name, value, ignore);
			onAssignMember(state);
			if (!state.Ignore)
				map.Add(state.Name, state.Value);
			return map;
		}

		public static object AsNameToObjectMapTree(this object value, bool ignoreDefaults = true, IEqualityComparer<string> comparer = null,
			Action<AssignMemberState> onAssignMember = null)
		{
			switch (value)
			{
				case null:
					return null;
				case ValueType @struct:
					return @struct;
				case string str:
					return str;
				case JToken jToken:
					return jToken;
				case ICollection collection:
					return collection
						.Cast<object>()
						.Select(o => o.AsNameToObjectMapTree(ignoreDefaults, comparer, onAssignMember))
						.ToList();
				default:
					return value.AsNameToObjectMap(comparer, true, ignoreDefaults, onAssignMember);
			}
		}

		public static Dictionary<string, string> Assign(this Dictionary<string, string> map, object newValues,
			bool ignoreDefaults = false, Action<AssignMemberState> onAssignMember = null)
		{
			if (newValues != null)
			{
				Type type = newValues.GetType();
				TypeInfo typeInfo = type.GetTypeInfo();
				foreach (PropertyInfo member in typeInfo.GetProperties())
				{
					object value = member.GetValue(newValues);
					map.Assign(member, value, ignoreDefaults, onAssignMember);
				}

				foreach (FieldInfo member in typeInfo.GetFields())
				{
					object value = member.GetValue(newValues);
					map.Assign(member, value, ignoreDefaults, onAssignMember);
				}
			}

			return map;
		}

		internal static Dictionary<string, string> Assign(this Dictionary<string, string> map, MemberInfo member, object value,
			bool ignoreDefaults = false, Action<AssignMemberState> onAssignMember = null)
		{
			bool ignore = ignoreDefaults && value.IsDefaultValue() || member.GetCustomAttribute<JsonIgnoreAttribute>() != null;
			var attr = member.GetCustomAttribute<JsonPropertyAttribute>();
			string name = attr?.PropertyName ?? member.Name;
			AssignMemberState state = new AssignMemberState(member, name, value, ignore);
			onAssignMember = onAssignMember ?? (dummy =>
				{
				});
			onAssignMember(state);
			if (!state.Ignore)
				map.Add(state.Name, state.Value.ToStringOrNull());
			return map;
		}

		public static Dictionary<string, object> AsNameToObjectMap(this object obj, IEqualityComparer<string> comparer = null,
			bool deep = false, bool ignoreDefaults = false, Action<AssignMemberState> onAssignMember = null)
		{
			return new Dictionary<string, object>(comparer ?? EqualityComparer<string>.Default).Assign(obj, deep, ignoreDefaults, onAssignMember);
		}

		public static Dictionary<string, string> AsNameToStringMap(this object obj, IEqualityComparer<string> comparer = null,
			bool ignoreDefaults = false, Action<AssignMemberState> onAssignMember = null)
		{
			return new Dictionary<string, string>(comparer ?? EqualityComparer<string>.Default).Assign(obj, ignoreDefaults, onAssignMember);
		}
	}
}
