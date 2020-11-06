namespace Isap.CommonCore.Expressions.Evaluation
{
	public interface IPropertyGetAccessor
	{
		string PropertyName { get; }
		object GetValue(object obj);
	}

	public interface IPropertyGetAccessor<in TEntity>: IPropertyGetAccessor
	{
		object GetValue(TEntity obj);
	}

	public interface IPropertyGetAccessor<in TEntity, out TPropertyValue>: IPropertyGetAccessor<TEntity>
	{
		new TPropertyValue GetValue(TEntity obj);
	}
}
