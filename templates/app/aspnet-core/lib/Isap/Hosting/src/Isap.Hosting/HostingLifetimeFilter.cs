using Serilog.Core;
using Serilog.Events;

namespace Isap.Hosting
{
	public class HostingLifetimeFilter: ILogEventFilter
	{
		private const string HostingLifetimeLoggingSourceContext = "Microsoft.Hosting.Lifetime";

		public bool IsEnabled(LogEvent logEvent)
		{
			if (!logEvent.Properties.TryGetValue("SourceContext", out var value))
				return true;
			string sourceContext = value.ToString().Trim('"');
			return sourceContext == HostingLifetimeLoggingSourceContext;
		}
	}
}
