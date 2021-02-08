using Isap.Abp.Extensions.Data;

namespace Isap.Abp.FileStorage.EntityFrameworkCore
{
	public abstract class FileStorageDbModuleBase<TModelBuilderImpl>
		: AbpExtDbModule<IFileStorageModelBuilder, TModelBuilderImpl>
		where TModelBuilderImpl: class, IFileStorageModelBuilder
	{
	}
}
