using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace Isap.Abp.Extensions.Data
{
	[Obsolete]
	public interface IDbContextAccessor
	{
		DbContext Current { get; }
		IDisposable Use(DbContext context);
	}

	[Obsolete]
	public class DbContextAccessor: IDbContextAccessor, ISingletonDependency
	{
		private class StackBookmark: IDisposable
		{
			private readonly DbContextAccessor _accessor;
			private readonly ImmutableStack<DbContext> _stack;

			public StackBookmark(DbContextAccessor accessor, ImmutableStack<DbContext> stack)
			{
				_accessor = accessor;
				_stack = stack;
			}

			public void Dispose()
			{
				_accessor._dbContextStack.Value = _stack;
			}
		}

		private readonly AsyncLocal<ImmutableStack<DbContext>> _dbContextStack = new AsyncLocal<ImmutableStack<DbContext>>();

		public DbContext Current => GetOrCreateStack().Peek();

		public IDisposable Use(DbContext context)
		{
			ImmutableStack<DbContext> stack = GetOrCreateStack();
			StackBookmark bookmark = new StackBookmark(this, stack);
			_dbContextStack.Value = stack.Push(context);
			return bookmark;
		}

		private ImmutableStack<DbContext> GetOrCreateStack()
		{
			ImmutableStack<DbContext> stack = _dbContextStack.Value;
			if (stack == null)
				_dbContextStack.Value = stack = ImmutableStack<DbContext>.Empty.Push(null);
			return stack;
		}
	}
}
