using System;
using System.Collections.Immutable;
using System.Threading;
using Castle.Core.Logging;

namespace Isap.CommonCore.Logging
{
	public interface ILoggingContext
	{
		IDisposable WithLogicalProperty(ILogger logger, string name, object value);
	}

	public class LoggingContext: ILoggingContext
	{
		private class NullLoggerPropertyValue: DisposableBase, ILoggerPropertyValue
		{
			#region Overrides of DisposableBase

			protected override void InternalDispose()
			{
			}

			#endregion

			#region Implementation of ILoggerPropertyValue

			public string Name { get; } = null;
			public object Value { get; } = null;

			#endregion
		}

		private class NullLoggerContextPropertyFactory: ILoggerContextPropertyFactory
		{
			#region Implementation of ILoggerContextPropertyFactory

			public ILoggerPropertyValue Create(ILogger logger, string name, object value)
			{
				return new NullLoggerPropertyValue();
			}

			#endregion
		}

		private class StackBookmark: DisposableBase
		{
			private readonly LoggingContext _context;
			private readonly ImmutableStack<ILoggerPropertyValue> _stack;
			private readonly ILoggerPropertyValue _propertyValue;

			public StackBookmark(LoggingContext context, ImmutableStack<ILoggerPropertyValue> stack, ILoggerPropertyValue propertyValue)
			{
				_context = context;
				_stack = stack;
				_propertyValue = propertyValue;
			}

			#region Overrides of DisposableBase

			protected override void InternalDispose()
			{
				_context._valuesStack.Value = _stack;
				_propertyValue.Dispose();
			}

			#endregion
		}

		private static readonly AsyncLocal<ILoggingContext> _current = new AsyncLocal<ILoggingContext>();
		private static ILoggingContext _default = new LoggingContext(new NullLoggerContextPropertyFactory());

		private readonly ILoggerContextPropertyFactory _factory;
		private readonly AsyncLocal<ImmutableStack<ILoggerPropertyValue>> _valuesStack = new AsyncLocal<ImmutableStack<ILoggerPropertyValue>>();

		public LoggingContext(ILoggerContextPropertyFactory factory)
		{
			_factory = factory;
		}

		public static ILoggingContext Current => _current.Value ?? _default;

		public IDisposable WithLogicalProperty(ILogger logger, string name, object value)
		{
			ImmutableStack<ILoggerPropertyValue> stack = GetOrCreateValuesStack();
			ILoggerPropertyValue propertyValue = _factory.Create(logger, name, value);
			StackBookmark bookmark = new StackBookmark(this, stack, propertyValue);
			_valuesStack.Value = stack.Push(propertyValue);
			return bookmark;
		}

		public static void SetDefault(ILoggingContext context)
		{
			_default = context;
		}

		public static void SetCurrent(ILoggingContext context)
		{
			_current.Value = context;
		}

		private ImmutableStack<ILoggerPropertyValue> GetOrCreateValuesStack()
		{
			ImmutableStack<ILoggerPropertyValue> stack = _valuesStack.Value;
			if (stack == null)
				_valuesStack.Value = stack = ImmutableStack<ILoggerPropertyValue>.Empty;
			return stack;
		}
	}
}
