using System;
using System.Net;
using Abp.Web.Models;
using Newtonsoft.Json.Linq;

namespace Isap.Abp.Extensions.Api
{
	public static class AbpIoResponseExtensions
	{
		public static AbpIoResponse ToAbpIoResponse(this AjaxResponseBase abpResponse)
		{
			ErrorInfo error = abpResponse.Error;
			if (error == null)
				return new AbpIoResponse();

			if (error.Code == (int) HttpStatusCode.InternalServerError)
			{
				if (!string.IsNullOrEmpty(error.Details))
				{
					AbpIoResponse TryParseAbpIoResponse(string json)
					{
						try
						{
							JToken jToken = JToken.Parse(json);
							return jToken.ToObject<AbpIoResponse>();
						}
						catch (Exception)
						{
							return null;
						}
					}

					AbpIoResponse abpIoResponse = TryParseAbpIoResponse(error.Details);
					if (abpIoResponse != null)
						return abpIoResponse;
				}
			}

			return new AbpIoResponse
				{
					Error = new AbpIoErrorInfo
						{
							Code = error.Code.ToString(),
							Message = error.Message,
							Details = error.Details,
						},
				};
		}
	}
}
