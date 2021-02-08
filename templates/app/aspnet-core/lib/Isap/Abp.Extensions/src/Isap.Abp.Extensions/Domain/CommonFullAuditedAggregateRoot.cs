using System;
using System.Collections.Generic;
using Isap.CommonCore.Services;
using Isap.Converters;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Isap.Abp.Extensions.Domain
{
	// ReSharper disable once PossibleInterfaceMemberAmbiguity
	public interface ICommonFullAuditedAggregateRoot<TKey>: IAggregateRoot<TKey>, IFullAuditedObject, ICommonEntity<TKey>
	{
	}

	[Serializable]
	public abstract class CommonFullAuditedAggregateRoot<TKey>: FullAuditedAggregateRoot<TKey>, ICommonFullAuditedAggregateRoot<TKey>
	{
		protected CommonFullAuditedAggregateRoot()
		{
		}

		protected CommonFullAuditedAggregateRoot(TKey id)
			: base(id)
		{
		}

		//
		protected IValueConverter Converter => ValueConverterProviders.Current.GetConverter();

		object ICommonEntity.GetId()
		{
			return Id;
		}

		protected void SetValue<T>(string name, T value)
		{
			ExtraProperties[name] = value;
		}

		protected T GetValue<T>(string name, Func<T> getDefaultValue)
		{
			return Converter.TryConvertTo<T>(ExtraProperties.GetOrDefault(name)).AsDefaultIfNotSuccess(getDefaultValue);
		}

		protected T GetValue<T>(string name, T defaultValue = default)
		{
			return Converter.TryConvertTo<T>(ExtraProperties.GetOrDefault(name)).AsDefaultIfNotSuccess(defaultValue);
		}
	}
}
