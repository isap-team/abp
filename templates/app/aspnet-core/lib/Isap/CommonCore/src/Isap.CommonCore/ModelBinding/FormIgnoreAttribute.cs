using System;

namespace Isap.CommonCore.ModelBinding
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class FormIgnoreAttribute: Attribute
	{
	}
}
