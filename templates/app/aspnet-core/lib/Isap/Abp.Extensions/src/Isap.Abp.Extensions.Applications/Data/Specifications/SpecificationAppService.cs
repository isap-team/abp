using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Services;

namespace Isap.Abp.Extensions.Data.Specifications
{
	public class SpecificationAppService: AppServiceBase, ISpecificationAppService
	{
		protected ISpecificationBuilderRepository SpecificationBuilderRepository => LazyGetRequiredService<ISpecificationBuilderRepository>();

		public async Task<List<SpecificationMetadataDto>> GetAll()
		{
			await Task.Yield();

			string GetFullName(SpecificationMetadata m) =>
				string.Join('.',
					m.Namespace
						.Concat(m.Types.Select(t => t.Name))
						.Concat(Enumerable.Repeat(m.Name, 1))
				);

			List<SpecificationMetadata> metadata = SpecificationBuilderRepository.GetAllMetadata()
				.OrderBy(GetFullName)
				.ToList();
			return ObjectMapper.Map<List<SpecificationMetadata>, List<SpecificationMetadataDto>>(metadata);
		}
	}
}
