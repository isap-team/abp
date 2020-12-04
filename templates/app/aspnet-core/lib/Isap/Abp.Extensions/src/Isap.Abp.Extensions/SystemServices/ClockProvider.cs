using System;
using Isap.Converters;
using Microsoft.Extensions.Options;
using Volo.Abp.Timing;

namespace Isap.Abp.Extensions.SystemServices
{
	public interface IClockProvider
	{
		IClock Clock { get; }
	}

	public sealed class ClockProvider
		: IClockProvider
	{
		private static readonly AsyncLocalStackContainer<IClockProvider> _convertersStackContainer =
			new AsyncLocalStackContainer<IClockProvider>(() => Default);

		public static readonly IClockProvider Default = new ClockProvider(CreateClock());

		public ClockProvider(IClock clock)
		{
			Clock = clock;
		}

		public static IClockProvider Current => _convertersStackContainer.Current;

		public IClock Clock { get; }

		private static Clock CreateClock() =>
			new Clock(new OptionsWrapper<AbpClockOptions>(new AbpClockOptions
				{
					Kind = DateTimeKind.Utc,
				}));

		public static IDisposable Use(IClock clock)
		{
			return _convertersStackContainer.Use(baseProvider => new ClockProvider(clock));
		}
	}
}
