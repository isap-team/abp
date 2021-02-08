using System;

namespace Isap.CommonCore.Services
{
	public interface IDocumentHeader
	{
		string DocNumber { get; }
		DateTime DocDate { get; }
	}
}
