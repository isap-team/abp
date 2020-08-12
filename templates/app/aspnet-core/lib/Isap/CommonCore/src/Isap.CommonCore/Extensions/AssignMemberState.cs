using System.Reflection;

namespace Isap.CommonCore.Extensions
{
	public class AssignMemberState
	{
		public AssignMemberState(MemberInfo member, string name, object value, bool ignore)
		{
			Member = member;
			Name = name;
			Value = value;
			Ignore = ignore;
		}

		public MemberInfo Member { get; }
		public string Name { get; set; }
		public object Value { get; set; }
		public bool Ignore { get; set; }
	}
}
