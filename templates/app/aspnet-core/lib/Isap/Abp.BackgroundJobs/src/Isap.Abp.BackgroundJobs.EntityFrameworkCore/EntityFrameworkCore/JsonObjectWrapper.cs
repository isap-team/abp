using Newtonsoft.Json.Linq;

namespace Isap.Abp.BackgroundJobs.EntityFrameworkCore
{
	public class JsonObjectWrapper
	{
		public string TypeName { get; set; }
		public JToken Value { get; set; }
	}
}
