namespace Isap.Abp.Extensions.Metadata
{
	public class EntityReferenceOptions
	{
		public EntityReferenceOptions(string serviceName, string keyPropertyName, string displayPropertyName)
		{
			ServiceName = serviceName;
			KeyPropertyName = keyPropertyName;
			DisplayPropertyName = displayPropertyName;
		}

		public string ServiceName { get; }
		public string KeyPropertyName { get; }
		public string DisplayPropertyName { get; }
	}
}
