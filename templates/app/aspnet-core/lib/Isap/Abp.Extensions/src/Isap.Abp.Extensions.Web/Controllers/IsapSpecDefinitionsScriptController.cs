using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Data.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Volo.Abp.Http;
using Volo.Abp.Minify.Scripts;

namespace Isap.Abp.Extensions.Web.Controllers
{
	[Area("Isap")]
	[Route("Isap/SpecDefinitionsScript")]
	[DisableAuditing]
	[RemoteService(false)]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class IsapSpecDefinitionsScriptController: AbpController
	{
		private readonly AbpAspNetCoreMvcOptions _options;
		private readonly ISpecificationAppService _specificationAppService;
		private readonly IJavascriptMinifier _javascriptMinifier;

		public IsapSpecDefinitionsScriptController(
			ISpecificationAppService specificationAppService,
			IOptions<AbpAspNetCoreMvcOptions> options,
			IJavascriptMinifier javascriptMinifier)
		{
			_options = options.Value;
			_specificationAppService = specificationAppService;
			_javascriptMinifier = javascriptMinifier;
		}

		[HttpGet]
		[Produces(MimeTypes.Application.Javascript, MimeTypes.Text.Plain)]
		public async Task<IActionResult> Index()
		{
			List<SpecificationMetadataDto> metadata = await _specificationAppService.GetAll();

			IEnumerable<string> scriptItems = metadata
				.GroupBy(m => string.Join('.', m.Namespace))
				.OrderBy(g => g.Key)
				.SelectMany(g => GetScriptForNamespace(new string(' ', 8), g.Key, g));

			string script = @$"//****************************************************************************//
//* ISAP Specification Definitions

var isap = isap || {{}};

(function () {{

    isap.spec = isap.spec || {{}};

    (function () {{

        isap.spec.defs = isap.spec.defs || {{}};

        (function () {{
            {string.Join("\r\n", scriptItems)}
        }})();

    }})();

}})();
";
			return Content(
				_options.MinifyGeneratedScript == true
					? _javascriptMinifier.Minify(script)
					: script,
				MimeTypes.Application.Javascript
			);
		}

		private IEnumerable<string> GetScriptForNamespace(string ident, string @namespace, IEnumerable<SpecificationMetadataDto> metadata)
		{
			yield return $"var ns = abp.utils.createNamespace(isap.spec.defs, '{@namespace}');";

			IEnumerable<string> lines =
				metadata
					.GroupBy(m => m.Types.First())
					.OrderBy(g => g.Key)
					.SelectMany(g =>
						GetScriptForType(ident + new string(' ', 4), "ns", g.Key,
							g.Select(m => Tuple.Create(m, m.Types.Skip(1).ToList())).ToList()
						)
					);

			foreach (string line in lines)
				yield return line;
		}

		private IEnumerable<string> GetScriptForType(string ident, string nsRefName, string type, List<Tuple<SpecificationMetadataDto, List<string>>> metadata)
		{
			if (string.IsNullOrEmpty(nsRefName))
				yield return $"{ident}\"{type}\": {{";
			else
				yield return $"{ident}{nsRefName}.{type} = {{";

			List<Tuple<SpecificationMetadataDto, List<string>>> fields = metadata.Where(tuple => tuple.Item2.Count == 0).ToList();

			string innerIdent = ident + new string(' ', 4);

			foreach (Tuple<SpecificationMetadataDto, List<string>> tuple in fields)
			{
				yield return $"{innerIdent}\"{tuple.Item1.Name}\": '{tuple.Item1.SpecId}',";
			}

			IEnumerable<string> lines = metadata.Except(fields)
				.GroupBy(tuple => tuple.Item2.First())
				.OrderBy(g => g.Key)
				.SelectMany(g =>
					GetScriptForType(innerIdent, null, g.Key,
						g.Select(m => Tuple.Create(m.Item1, m.Item2.Skip(1).ToList())).ToList()
					)
				)
				.ToList();

			foreach (string line in lines)
				yield return line;

			yield return $"{innerIdent}\"_\": null";

			if (string.IsNullOrEmpty(nsRefName))
				yield return $"{ident}}},";
			else
				yield return $"{ident}}};";
		}
	}
}
