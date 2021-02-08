using System;
using Isap.CommonCore.Factories;

namespace Isap.Abp.FileStorage.Configurations
{
	[AttributeUsage(AttributeTargets.Class)]
	public class StorageTypeAttribute: Attribute, IFactoryProductMarker
	{
		public StorageTypeAttribute(string storageTypeName)
		{
			StorageTypeName = storageTypeName;
		}

		public string StorageTypeName { get; }

		string IFactoryProductMarker.ProductKey => StorageTypeName;
	}
}
