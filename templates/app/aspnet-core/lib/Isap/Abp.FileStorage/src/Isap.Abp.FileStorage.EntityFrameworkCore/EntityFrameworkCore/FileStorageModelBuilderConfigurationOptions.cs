using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	public class FileStorageModelBuilderConfigurationOptions: AbpModelBuilderConfigurationOptions
	{
		public FileStorageModelBuilderConfigurationOptions(
			[NotNull] string tablePrefix = "",
			[CanBeNull] string schema = null)
			: base(
				tablePrefix,
				schema)
		{
		}
	}
}
