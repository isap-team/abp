using System;
using AutoMapper;
using Isap.Abp.Extensions;
using Isap.Abp.FileStorage.Files;

namespace Isap.Abp.FileStorage
{
	public static class FileStorageAutoMapperExtensions
	{
		public static FileDataDto TryLoadFileDataFromCache(this ResolutionContext ctx, Guid? fileDataId)
		{
			return fileDataId.HasValue
					? ctx.LoadInstance<IFileDataCache, FileDataDto>(fileDataId.Value, cache => cache.GetOrNull(fileDataId.Value))
					: null
				;
		}

		public static FileDataDto LoadFileDataFromCache(this ResolutionContext ctx, Guid fileDataId)
		{
			FileDataDto result = TryLoadFileDataFromCache(ctx, fileDataId);
			return result ?? throw new InvalidOperationException($"Can't find file data with id = '{fileDataId}'.");
		}
	}
}
