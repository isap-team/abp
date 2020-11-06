using System;
using System.Collections;
using System.Collections.Generic;
using Isap.CommonCore.Extensions;

namespace Isap.CommonCore.Utils
{
	public static class FormDataHelpers
	{
		public static IEnumerable<Tuple<string, string>> ToFormData(this object obj, string prefix = null)
		{
			switch (obj)
			{
				case null:
					yield break;
				case string s:
					yield return Tuple.Create(prefix, s);
					break;
				case ValueType @struct:
					yield return Tuple.Create(prefix, Convert.ToString(@struct));
					break;
				case IList list:
					for (int i = 0; i < list.Count; i++)
						foreach (Tuple<string, string> tuple in list[i].ToFormData($"{prefix}[{i}]"))
							yield return tuple;
					break;
				default:
					string objSelector = prefix == null ? "" : prefix + '.';
					foreach (KeyValuePair<string, object> pair in obj.AsNameToObjectMap())
					foreach (Tuple<string, string> tuple in pair.Value.ToFormData($"{objSelector}{pair.Key}"))
						yield return tuple;
					break;
			}
		}

		public static IEnumerable<Tuple<string, string>> ToPhpHttpQuery(this object obj, string prefix = null)
		{
			switch (obj)
			{
				case null:
					yield break;
				case string s:
					yield return Tuple.Create(prefix, s);
					break;
				case ValueType @struct:
					yield return Tuple.Create(prefix, Convert.ToString(@struct));
					break;
				case IList list:
					for (int i = 0; i < list.Count; i++)
						foreach (Tuple<string, string> tuple in list[i].ToPhpHttpQuery($"{prefix}[{i}]"))
							yield return tuple;
					break;
				case Dictionary<string, object> map:
					foreach (KeyValuePair<string, object> pair in map)
					{
						string objSelector = prefix == null ? pair.Key : $"{prefix}[{pair.Key}]";
						foreach (Tuple<string, string> tuple in pair.Value.ToPhpHttpQuery(objSelector))
							yield return tuple;
					}

					break;
				default:
					foreach (Tuple<string, string> tuple in obj.AsNameToObjectMap().ToPhpHttpQuery(prefix))
						yield return tuple;
					break;
			}
		}
	}
}
