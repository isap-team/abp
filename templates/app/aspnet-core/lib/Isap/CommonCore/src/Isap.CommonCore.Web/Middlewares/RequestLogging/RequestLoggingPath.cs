using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Isap.CommonCore.Web.Middlewares.RequestLogging
{
	public class RequestLoggingPath
	{
		public RequestLoggingPath(PathString path, bool include)
		{
			Path = path;
			Include = include;
		}

		public PathString Path { get; set; }
		public bool Include { get; set; }

		public static RequestLoggingPath Parse(string path)
		{
			string[] items = path.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries).ToArray();
			switch (items.Length)
			{
				case 1:
					return new RequestLoggingPath(PathString.FromUriComponent(items[0]), true);
				case 2:
					return new RequestLoggingPath(PathString.FromUriComponent(items[1]), items[0] != "-");
				default:
					throw new InvalidOperationException($"Request logging path rule is in invalid format.");
			}
		}
	}
}
