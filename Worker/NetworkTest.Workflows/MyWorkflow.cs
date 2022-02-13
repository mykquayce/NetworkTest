using WorkflowCore.Interface;

namespace NetworkTest.Workflows;

public class MyWorkflow : IWorkflow<PersistenceData>
{
	public string Id => nameof(MyWorkflow);

	public int Version => 1;

	public void Build(IWorkflowBuilder<PersistenceData> builder)
	{
		builder
			.StartWith<Steps.PingStep>()
				.Output(data => data.Results, step => step.Results)
			.Then<Steps.SaveStep>()
				.Input(step => step.Results, data => data.Results);
	}
}
