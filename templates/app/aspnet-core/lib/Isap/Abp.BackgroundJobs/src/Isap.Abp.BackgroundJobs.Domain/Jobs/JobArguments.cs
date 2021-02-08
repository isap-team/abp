using System;
using System.Text.Json;

namespace Isap.Abp.BackgroundJobs.Jobs
{
	public interface IJobArguments
	{
		string Key { get; }
		string ArgumentsTypeName { get; }
		Type ArgumentsType { get; }
		JsonDocument Arguments { get; }
	}

	public class JobArguments: IJobArguments
	{
		public string Key { get; set; }

		public string ArgumentsType { get; set; }

		string IJobArguments.ArgumentsTypeName => ArgumentsType;
		Type IJobArguments.ArgumentsType => Type.GetType(ArgumentsType);

		public JsonDocument Arguments { get; set; }
	}
}
