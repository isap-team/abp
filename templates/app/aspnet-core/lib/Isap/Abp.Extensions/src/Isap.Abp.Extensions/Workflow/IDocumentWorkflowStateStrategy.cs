using System.Threading.Tasks;

namespace Isap.Abp.Extensions.Workflow
{
	public interface IDocumentWorkflowStateStrategy
	{
		Task<bool> CanSave(IDocumentWorkflow workflow);

		Task OnEnter(IDocumentWorkflow workflow, object args);
		Task OnReEnter(IDocumentWorkflow workflow, object args);
		Task OnLeave(IDocumentWorkflow workflow);
	}

	public interface IDocumentWorkflowStateStrategy<in TDocumentWorkflow>: IDocumentWorkflowStateStrategy
		where TDocumentWorkflow: IDocumentWorkflow
	{
		Task<bool> CanSave(TDocumentWorkflow workflow);

		Task OnEnter(TDocumentWorkflow workflow, object args);
		Task OnReEnter(TDocumentWorkflow workflow, object args);
		Task OnLeave(TDocumentWorkflow workflow);
	}
}
