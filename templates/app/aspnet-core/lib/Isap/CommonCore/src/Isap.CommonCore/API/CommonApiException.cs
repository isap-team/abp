using System;
using System.Net;

namespace Isap.CommonCore.API
{
	public class CommonApiException: Exception
	{
		public CommonApiException(string errorCode, string errorMessage)
			: base($"Unexpected error occured while requesting API with error code = '{errorCode}' and message = '{errorMessage}'.")
		{
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
		}

		public CommonApiException(string errorCode, string errorMessage, HttpStatusCode statusCode, string reasonPhrase)
			: this(errorCode, errorMessage)
		{
			StatusCode = statusCode;
			ReasonPhrase = reasonPhrase;
		}

		public CommonApiException(HttpStatusCode statusCode, string reasonPhrase)
			: base($"Unexpected HTTP response with status = '{statusCode}' and reason '{reasonPhrase}'.")
		{
			ErrorCode = $"HTTP:{statusCode}";
			StatusCode = statusCode;
			ReasonPhrase = reasonPhrase;
		}

		public HttpStatusCode StatusCode { get; }
		public string ReasonPhrase { get; }

		public string ErrorCode { get; }
		public string ErrorMessage { get; }
	}
}
