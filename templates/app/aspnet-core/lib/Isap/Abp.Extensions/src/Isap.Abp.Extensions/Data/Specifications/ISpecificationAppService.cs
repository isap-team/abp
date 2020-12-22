using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.CommonCore.Services;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public interface ISpecificationAppService: ICommonAppService
	{
		Task<List<SpecificationMetadataDto>> GetAll();
	}
}
