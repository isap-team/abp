using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Isap.Abp.Extensions.Data.Specifications
{
	[RemoteService]
	[Route("api/isap/specifications")]
	public class SpecificationController: IsapControllerBase, ISpecificationAppService
	{
		protected ISpecificationAppService AppService => LazyServiceProvider.LazyGetRequiredService<ISpecificationAppService>();

		[HttpGet]
		[Route("all")]
		public Task<List<SpecificationMetadataDto>> GetAll()
		{
			return AppService.GetAll();
		}
	}
}
