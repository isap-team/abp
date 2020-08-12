using System;

namespace Isap.CommonCore.API
{
	/// <summary>
	///     Used to mark enum value as unconvertable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ApiEnumIgnoreAttribute: Attribute
	{
	}
}
