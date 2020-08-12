using System;
using System.Collections.Generic;

namespace Isap.Abp.Extensions.DataFilters
{
	public interface IDataFilterDefinitionBuilder
	{
		IDataFilterDefinitionBuilder Register(Action<DataFilterDefinition> buildAction);
		DataFilterDefinition Build();
	}

	public class DataFilterDefinitionBuilder: IDataFilterDefinitionBuilder
	{
		private readonly List<Action<DataFilterDefinition>> _buildActions = new List<Action<DataFilterDefinition>>();

		private readonly Guid _dataFilterId;
		private readonly DataFilterType _dataFilterType;

		private DataFilterDefinitionBuilder(Guid dataFilterId, DataFilterType dataFilterType)
		{
			_dataFilterId = dataFilterId;
			_dataFilterType = dataFilterType;
		}

		public IDataFilterDefinitionBuilder Register(Action<DataFilterDefinition> buildAction)
		{
			_buildActions.Add(buildAction);
			return this;
		}

		public DataFilterDefinition Build()
		{
			var definition = new DataFilterDefinition
				{
					Id = _dataFilterId,
					Type = _dataFilterType,
					Title = $"Data Filter (id = '{_dataFilterId:D}')",
				};
			foreach (Action<DataFilterDefinition> buildAction in _buildActions)
				buildAction(definition);
			return definition;
		}

		public static IDataFilterDefinitionBuilder Create(Guid dataFilterId, DataFilterType dataFilterType)
		{
			return new DataFilterDefinitionBuilder(dataFilterId, dataFilterType);
		}

		public static IDataFilterDefinitionBuilder Create(string dataFilterId, DataFilterType dataFilterType)
		{
			return Create(new Guid(dataFilterId), dataFilterType);
		}
	}
}
