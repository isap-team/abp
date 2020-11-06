using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Isap.CommonCore.Web.Middlewares.RequestLogging
{
	public class IsapRequestLoggingOptions
	{
		public IsapRequestLoggingOptions(params RequestLoggingPath[] basePaths)
		{
			BasePaths = new List<RequestLoggingPath>(basePaths);
		}

		public IsapRequestLoggingOptions(params PathString[] basePaths)
			: this(basePaths.Select(i => new RequestLoggingPath(i, true)).ToArray())
		{
		}

		public IsapRequestLoggingOptions(params string[] basePaths)
			: this(basePaths.Select(RequestLoggingPath.Parse).ToArray())
		{
		}

		public IsapRequestLoggingOptions()
		{
		}

		public List<RequestLoggingPath> BasePaths { get; set; } = new List<RequestLoggingPath>();

		public bool IsEnabled { get; set; }

		public bool IsRequestLoggingEnabled(PathString requestPath)
		{
			if (!IsEnabled) return false;
			if (BasePaths.Count == 0) return true;
			RequestLoggingPath requestLoggingPath = BasePaths.LastOrDefault(i => i.Path.Value == "/" || requestPath.StartsWithSegments(i.Path));
			return requestLoggingPath != null && requestLoggingPath.Include;
		}

		public void AddBasePaths(IEnumerable<RequestLoggingPath> basePaths)
		{
			BasePaths.AddRange(basePaths);
		}

		public void AddBasePaths(params PathString[] basePaths)
		{
			AddBasePaths(basePaths.Select(i => new RequestLoggingPath(i, true)));
		}

		public void AddBasePaths(params string[] basePaths)
		{
			AddBasePaths(basePaths.Select(RequestLoggingPath.Parse));
		}
	}
}
