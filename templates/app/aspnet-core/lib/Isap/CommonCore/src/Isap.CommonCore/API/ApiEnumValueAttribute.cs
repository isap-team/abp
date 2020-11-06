using System;

namespace Isap.CommonCore.API
{
	/// <summary>
	///     Used to convert enum value to/from string value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ApiEnumValueAttribute: Attribute
	{
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="value">API enum value representation.</param>
		public ApiEnumValueAttribute(string value)
		{
			Value = value;
		}

		/// <summary>
		///     API enum value representation.
		/// </summary>
		public string Value { get; }
	}
}
