using System;
using Castle.Core.Logging;
using Isap.CommonCore;
using Isap.CommonCore.Logging;
using Serilog.Context;

namespace Isap.Hosting
{
	public class SerilogLoggerContextPropertyFactory: ILoggerContextPropertyFactory
	{
		private class SerilogLoggerPropertyValue: DisposableBase, ILoggerPropertyValue
		{
			private readonly IDisposable _bookmark;

			public SerilogLoggerPropertyValue(string name, object value)
			{
				Name = name;
				Value = value;
				_bookmark = LogContext.PushProperty(name, value);
			}

			public string Name { get; }
			public object Value { get; }

			protected override void InternalDispose()
			{
				_bookmark.Dispose();
			}
		}

		public ILoggerPropertyValue Create(ILogger logger, string name, object value)
		{
			return new SerilogLoggerPropertyValue(name, value);
		}
	}
}
