using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Isap.CommonCore.Utils
{
	/// <summary>
	///     Serializer Class.
	/// </summary>
	public class PhpSerializer
	{
		private readonly NumberFormatInfo _nfi;
		//types:
		// N = null
		// s = string
		// i = int
		// d = double
		// a = array (hashtable)

		private Dictionary<Hashtable, bool> _seenHashtables; //for serialize (to infinte prevent loops)
		private Dictionary<ArrayList, bool> _seenArrayLists; //for serialize (to infinte prevent loops) lol

		private int _pos; //for unserialize

		/// <summary>
		///     This member tells the serializer wether or not to strip carriage returns from strings when serializing and adding
		///     them back in when deserializing http://www.w3.org/TR/REC-xml/#sec-line-ends
		/// </summary>
		public bool XmlSafe = true;

		public Encoding StringEncoding = new UTF8Encoding();

		public PhpSerializer()
		{
			_nfi = new NumberFormatInfo
				{
					NumberGroupSeparator = "",
					NumberDecimalSeparator = "."
				};
		}

		public string Serialize(object obj)
		{
			_seenArrayLists = new Dictionary<ArrayList, bool>();
			_seenHashtables = new Dictionary<Hashtable, bool>();

			return InternalSerialize(obj, new StringBuilder()).ToString();
		} //Serialize(object obj)

		private StringBuilder InternalSerialize(object obj, StringBuilder sb)
		{
			if (obj == null)
			{
				return sb.Append("N;");
			}

			if (obj is string)
			{
				string str = (string) obj;
				if (XmlSafe)
				{
					str = str.Replace("\r\n", "\n"); //replace \r\n with \n
					str = str.Replace("\r", "\n"); //replace \r not followed by \n with a single \n  Should we do this?
				}

				return sb.Append("s:" + StringEncoding.GetByteCount(str) + ":\"" + str + "\";");
			}

			if (obj is bool)
			{
				return sb.Append("b:" + (((bool) obj) ? "1" : "0") + ";");
			}

			if (obj is int)
			{
				int i = (int) obj;
				return sb.Append("i:" + i.ToString(_nfi) + ";");
			}

			if (obj is double)
			{
				double d = (double) obj;
				return sb.Append("d:" + d.ToString(_nfi) + ";");
			}

			var arrayList = obj as ArrayList;
			if (arrayList != null)
			{
				if (_seenArrayLists.ContainsKey(arrayList))
					return sb.Append("N;"); //cycle detected

				_seenArrayLists.Add(arrayList, true);

				ArrayList a = arrayList;
				sb.Append("a:" + a.Count + ":{");
				for (int i = 0; i < a.Count; i++)
				{
					InternalSerialize(i, sb);
					InternalSerialize(a[i], sb);
				}

				sb.Append("}");
				return sb;
			}

			if (obj is Hashtable)
			{
				if (_seenHashtables.ContainsKey((Hashtable) obj))
					return sb.Append("N;"); //cycle detected

				_seenHashtables.Add((Hashtable) obj, true);

				Hashtable a = (Hashtable) obj;
				sb.Append("a:" + a.Count + ":{");
				foreach (DictionaryEntry entry in a)
				{
					InternalSerialize(entry.Key, sb);
					InternalSerialize(entry.Value, sb);
				}

				sb.Append("}");
				return sb;
			}

			return sb;
		} //Serialize(object obj)

		public object Deserialize(string str)
		{
			_pos = 0;
			return InternalDeserialize(str);
		} //Deserialize(string str)

		private object InternalDeserialize(string str)
		{
			if (str == null || str.Length <= _pos)
				return new Object();

			int start, end, length;
			string stLen;
			switch (str[_pos])
			{
				case 'N':
					_pos += 2;
					return null;
				case 'b':
					char chBool;
					chBool = str[_pos + 2];
					_pos += 4;
					return chBool == '1';
				case 'i':
					string stInt;
					start = str.IndexOf(":", _pos, StringComparison.Ordinal) + 1;
					end = str.IndexOf(";", start, StringComparison.Ordinal);
					stInt = str.Substring(start, end - start);
					_pos += 3 + stInt.Length;
					return Int32.Parse(stInt, _nfi);
				case 'd':
					string stDouble;
					start = str.IndexOf(":", _pos, StringComparison.Ordinal) + 1;
					end = str.IndexOf(";", start, StringComparison.Ordinal);
					stDouble = str.Substring(start, end - start);
					_pos += 3 + stDouble.Length;
					return Double.Parse(stDouble, _nfi);
				case 's':
					start = str.IndexOf(":", _pos, StringComparison.Ordinal) + 1;
					end = str.IndexOf(":", start, StringComparison.Ordinal);
					stLen = str.Substring(start, end - start);
					int bytelen = Int32.Parse(stLen);
					length = bytelen;
					//This is the byte length, not the character length - so we migth  
					//need to shorten it before usage. This also implies bounds checking
					if ((end + 2 + length) >= str.Length) length = str.Length - 2 - end;
					string stRet = str.Substring(end + 2, length);
					while (StringEncoding.GetByteCount(stRet) > bytelen)
					{
						length--;
						stRet = str.Substring(end + 2, length);
					}

					_pos += 6 + stLen.Length + length;
					if (XmlSafe)
					{
						stRet = stRet.Replace("\n", "\r\n");
					}

					return stRet;
				case 'a':
					//if keys are ints 0 through N, returns an ArrayList, else returns Hashtable
					start = str.IndexOf(":", _pos, StringComparison.Ordinal) + 1;
					end = str.IndexOf(":", start, StringComparison.Ordinal);
					stLen = str.Substring(start, end - start);
					length = Int32.Parse(stLen);
					Hashtable htRet = new Hashtable(length);
					ArrayList alRet = new ArrayList(length);
					_pos += 4 + stLen.Length; //a:Len:{
					for (int i = 0; i < length; i++)
					{
						//read key
						object key = Deserialize(str);
						//read value
						object val = Deserialize(str);

						if (alRet != null)
						{
							if (key is int && (int) key == alRet.Count)
								alRet.Add(val);
							else
								alRet = null;
						}

						htRet[key] = val;
					}

					_pos++; //skip the }
					if (_pos < str.Length && str[_pos] == ';') //skipping our old extra array semi-colon bug (er... php's weirdness)
						_pos++;
					if (alRet != null)
						return alRet;
					else
						return htRet;
				default:
					return "";
			} //switch
		} //unserialzie(object)	
	} //class Serializer
}
