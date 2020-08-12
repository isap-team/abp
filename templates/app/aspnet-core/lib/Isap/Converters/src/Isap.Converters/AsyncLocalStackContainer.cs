using System;
using System.Collections.Immutable;
using System.Threading;

namespace Isap.Converters
{
	public class AsyncLocalStackContainer<T>
	{
		private class StackBookmark: IDisposable
		{
			private readonly AsyncLocal<ImmutableStack<T>> _asyncLocalStack;
			private readonly ImmutableStack<T> _stack;

			public StackBookmark(AsyncLocal<ImmutableStack<T>> asyncLocalStack, ImmutableStack<T> stack)
			{
				_asyncLocalStack = asyncLocalStack;
				_stack = stack;
			}

			public void Dispose()
			{
				_asyncLocalStack.Value = _stack;
			}
		}

		private readonly AsyncLocal<ImmutableStack<T>> _asyncLocalStack = new AsyncLocal<ImmutableStack<T>>();
		private readonly Func<T> _getDefaultValue;

		public AsyncLocalStackContainer(Func<T> getDefaultValue)
		{
			_getDefaultValue = getDefaultValue;
		}

		public AsyncLocalStackContainer(T defaultValue)
			: this(() => defaultValue)
		{
		}

		public T Current => GetOrCreateStack(_asyncLocalStack, _getDefaultValue).Peek();

		public IDisposable Use(Func<T, T> getValue)
		{
			return Use(getValue(Current), _asyncLocalStack, _getDefaultValue);
		}

		protected static IDisposable Use(T provider, AsyncLocal<ImmutableStack<T>> asyncLocalStack, Func<T> getDefaultValue)
		{
			ImmutableStack<T> stack = GetOrCreateStack(asyncLocalStack, getDefaultValue);
			var bookmark = new StackBookmark(asyncLocalStack, stack);
			asyncLocalStack.Value = stack.Push(provider);
			return bookmark;
		}


		protected static ImmutableStack<T> GetOrCreateStack(AsyncLocal<ImmutableStack<T>> asyncLocalStack, Func<T> getDefaultValue)
		{
			ImmutableStack<T> stack = asyncLocalStack.Value;
			if (stack == null)
				asyncLocalStack.Value = stack = ImmutableStack<T>.Empty.Push(getDefaultValue());
			return stack;
		}
	}
}
