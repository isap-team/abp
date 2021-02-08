using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Isap.Abp.Extensions.Domain;
using Isap.Abp.Extensions.Workflow;
using Isap.CommonCore;
using Isap.CommonCore.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace Isap.Abp.Extensions.Services
{
	public abstract class IsapDocumentAppServiceBase<TEntityDto, TCreateDraftInput, TIntf, TImpl, TKey, TDomainManager>
		: DocumentAppServiceBase<TEntityDto, TCreateDraftInput, TIntf, TImpl, TKey, TDomainManager>
		where TEntityDto: DocumentEntityDto<TKey>
		where TIntf: class, IDocumentEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDomainManager: class, IDomainManager<TIntf, TImpl, TKey>
	{
		[HttpGet]
		[Route("get")]
		public override Task<TEntityDto> Get(TKey id)
		{
			return base.Get(id);
		}

		[HttpGet]
		[Route("many")]
		public override Task<Dictionary<TKey, TEntityDto>> GetMany(TKey[] idList)
		{
			return base.GetMany(idList);
		}

		[HttpPost]
		[Route("page")]
		public override Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, QueryOptionsDto queryOptions = null)
		{
			return base.GetPage(pageNumber, pageSize, countTotal, queryOptions);
		}

		[HttpPost]
		[Route("pageBySpecs")]
		public override Task<ResultSet<TEntityDto>> GetPage(int pageNumber, int pageSize, bool countTotal = false, List<SpecificationParameters> specifications = null)
		{
			return base.GetPage(pageNumber, pageSize, countTotal, specifications);
		}

		[HttpPost]
		[Route("create")]
		public override Task<TEntityDto> Create(TEntityDto entry)
		{
			return base.Create(entry);
		}

		[HttpPut]
		[Route("update")]
		public override Task<TEntityDto> Update(TEntityDto entry)
		{
			return base.Update(entry);
		}

		[HttpPost]
		[Route("save")]
		public override Task<TEntityDto> Save(TEntityDto entry)
		{
			return base.Save(entry);
		}

		[HttpDelete]
		[Route("delete")]
		public override Task Delete(TKey id)
		{
			return base.Delete(id);
		}

		[HttpPost]
		[Route("undelete")]
		public override Task<TEntityDto> Undelete(TKey id)
		{
			return base.Undelete(id);
		}

		[HttpPost]
		[Route("getOrCreateDraft")]
		public override Task<TEntityDto> GetOrCreateDraft(TCreateDraftInput input)
		{
			return base.GetOrCreateDraft(input);
		}

		[HttpGet]
		[Route("getDraft")]
		public override Task<TEntityDto> GetDraft(TKey id)
		{
			return base.GetDraft(id);
		}

		[HttpPost]
		[Route("saveDraft")]
		[DisableValidation]
		public override Task<TEntityDto> SaveDraft(TEntityDto entry)
		{
			return base.SaveDraft(entry);
		}

		[HttpDelete]
		[Route("deleteDraft")]
		public override Task DeleteDraft(TKey id)
		{
			return base.DeleteDraft(id);
		}

		[HttpDelete]
		[Route("deleteDraftForEntry")]
		[DisableValidation]
		public override Task DeleteDraft(TEntityDto entry)
		{
			return base.DeleteDraft(entry);
		}
	}

	public abstract class IsapDocumentAppServiceBase<TEntityDto, TCreateDraftInput, TIntf, TImpl, TKey, TDomainManager, TDocumentWorkflow>
		: IsapDocumentAppServiceBase<TEntityDto, TCreateDraftInput, TIntf, TImpl, TKey, TDomainManager>
		where TEntityDto: DocumentEntityDto<TKey>
		where TIntf: class, IDocumentEntity<TKey>
		where TImpl: class, IEntity<TKey>, TIntf
		where TDomainManager: class, IDocumentDomainManager<TIntf, TImpl, TKey, TCreateDraftInput>
		where TDocumentWorkflow: IDocumentWorkflow<TIntf>
	{
		public IDocumentWorkflowFactory WorkflowFactory => LazyGetRequiredService<IDocumentWorkflowFactory>();

		[HttpPost]
		[Route("getOrCreateDraft")]
		public override async Task<TEntityDto> GetOrCreateDraft(TCreateDraftInput input)
		{
			await CheckQueryPermission();
			TIntf result = await DomainManager.GetOrCreateDraft(input);
			return ToDto(result);
		}

		[HttpGet]
		[Route("getDraft")]
		public override Task<TEntityDto> GetDraft(TKey id)
		{
			return Get(id);
		}

		[HttpPost]
		[Route("saveDraft")]
		[DisableValidation]
		public override async Task<TEntityDto> SaveDraft(TEntityDto entry)
		{
			await CheckCreatePermission(entry);
			return await InternalSave(entry);
		}

		[HttpDelete]
		[Route("deleteDraft")]
		public override async Task DeleteDraft(TKey id)
		{
			await CheckQueryPermission();
			TIntf document = await DomainManager.Get(id);
			if (document != null)
			{
				if (!DomainManager.IsDraft(document))
					throw new UserFriendlyException("Ошибка удаления черновика: документ не является черновиком.");
				await DomainManager.Delete(id);
			}
		}

		[HttpDelete]
		[Route("deleteDraftForEntry")]
		[DisableValidation]
		public override async Task DeleteDraft(TEntityDto entry)
		{
			await CheckCreatePermission(entry);
			TIntf document = await DomainManager.Get(entry.Id);
			if (document != null)
			{
				if (!DomainManager.IsDraft(document))
					throw new UserFriendlyException("Ошибка удаления черновика: документ не является черновиком.");
				await DomainManager.Delete(document.Id);
			}
		}

		[HttpPost]
		[Route("save")]
		public override async Task<TEntityDto> Save(TEntityDto entry)
		{
			await CheckUpdatePermission(entry);
			return await InternalSave(entry);
		}

		protected async Task<TEntityDto> InternalSave(TEntityDto entry, Func<TDocumentWorkflow, Task> onSaveAction = null)
		{
			TIntf document = ToDomain(entry);
			IDocumentWorkflowActivator<TDocumentWorkflow> activator = WorkflowFactory.Create<TDocumentWorkflow>();
			document = await activator.Perform(document, async workflow =>
				{
					await workflow.Save();
					if (onSaveAction != null)
						await onSaveAction(workflow);
					return workflow.Document;
				});
			return ToDto(document);
		}

		protected virtual async Task<TEntityDto> Perform(TKey documentId, Func<TDocumentWorkflow, Task<TIntf>> action)
		{
			IDocumentWorkflowActivator<TDocumentWorkflow> activator = WorkflowFactory.Create<TDocumentWorkflow>();
			TIntf document = await DomainManager.Get(documentId);
			if (document == null)
				throw new UserFriendlyException($"Документ с идентификатором id = '{documentId}' не найден в базе данных.");
			document = await activator.Perform(document, action);
			return ToDto(document);
		}
	}
}
