namespace Isap.Abp.Extensions.Api
{
	public class AbpIoResponse
	{
		protected AbpIoResponse(bool success, AbpIoErrorInfo error, bool unAuthorizedRequest)
		{
			Success = success;
			Error = error;
			UnAuthorizedRequest = unAuthorizedRequest;
		}

		public AbpIoResponse(AbpIoErrorInfo error, bool unAuthorizedRequest)
		{
			Success = false;
			Error = error;
			UnAuthorizedRequest = unAuthorizedRequest;
		}

		public AbpIoResponse()
		{
			Success = true;
		}

		/// <summary>
		///     Indicates success status of the result.
		///     Set <see cref="Error" /> if this value is false.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		///     Error details (Must and only set if <see cref="Success" /> is false).
		/// </summary>
		public AbpIoErrorInfo Error { get; set; }

		/// <summary>
		///     This property can be used to indicate that the current user has no privilege to perform this request.
		/// </summary>
		public bool UnAuthorizedRequest { get; set; }
	}

	public class AbpIoResponse<TResult>: AbpIoResponse
	{
		public AbpIoResponse(TResult result)
			: base(true, null, false)
		{
			Result = result;
		}

		public AbpIoResponse(AbpIoErrorInfo error, bool unAuthorizedRequest)
			: base(error, unAuthorizedRequest)
		{
			Result = default;
		}

		/// <summary>
		///     The actual result object.
		///     It is set if <see cref="AbpIoResponse.Success" /> is true.
		/// </summary>
		public TResult Result { get; set; }
	}
}
