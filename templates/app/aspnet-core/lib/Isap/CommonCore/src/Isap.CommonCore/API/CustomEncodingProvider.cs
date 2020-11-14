using System;
using System.Collections.Generic;
using System.Text;

#if !DEBUG
using Castle.Core.Logging;
using Isap.CommonCore.Logging;
#endif

namespace Isap.CommonCore.API
{
	internal class CustomEncodingProvider: EncodingProvider
	{
		private static readonly Dictionary<string, Encoding> _encodings = new Dictionary<string, Encoding>(StringComparer.OrdinalIgnoreCase)
			{
				{ "ascii", Encoding.ASCII },
				{ "utf-8", Encoding.UTF8 },
#pragma warning disable 618
				{ "utf-7", Encoding.UTF7 },
#pragma warning restore 618
				{ "utf-16", GetWindowsEncoding(1200) },
				{ "utf-16le", GetWindowsEncoding(1200) },
				{ "utf-16be", GetWindowsEncoding(1201) },
				{ "utf-32", Encoding.UTF32 },
				{ "utf-32le", Encoding.UTF32 },
				{ "utf-32be", GetWindowsEncoding(12001) },
				{ "windows-1251", GetWindowsEncoding(1251) },
				{ "iso-8859-1", GetWindowsEncoding(28591) },
			};

		public override Encoding GetEncoding(int codepage)
		{
			return GetWindowsEncoding(codepage);
		}

		private static Encoding GetWindowsEncoding(int codepage)
		{
			return CodePagesEncodingProvider.Instance.GetEncoding(codepage);
		}

		public override Encoding GetEncoding(string name)
		{
			if (!_encodings.TryGetValue(name.Trim('"'), out Encoding result))
			{
#if DEBUG
				throw new InvalidOperationException($"Can't find encoding for name '{name}'.");
#else
				ILogger logger = ApplicationLogging.GetScopedLogger();
				logger.Warn($"Can't find encoding for name '{name}'.");
				return Encoding.ASCII;
#endif
			}

			return result;
		}
	}
}
