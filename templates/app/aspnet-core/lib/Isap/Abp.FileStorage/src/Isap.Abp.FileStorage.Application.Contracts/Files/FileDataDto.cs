using System;
using System.ComponentModel.DataAnnotations;
using Isap.CommonCore.Services;
using Isap.CommonCore.Validation;

namespace Isap.Abp.FileStorage.Files
{
	[Serializable]
	public class FileDataDto: CommonEntityDto<Guid>, ICommonNormalize
	{
		public long Length { get; set; }

		[Required]
		[MaxLength(64)]
		public string ContentType { get; set; }

		[Required]
		[MaxLength(1024)]
		public string FileName { get; set; }

		[Required]
		[MaxLength(128)]
		public string Name { get; set; }

		[Required]
		[MaxLength(1024)]
		public string Path { get; set; }

		[Required]
		[MaxLength(40)]
		public string Hash { get; set; }

		public string Url { get; set; }

		public void Normalize()
		{
			Url = Url?.Trim();
			Path = Path?.Trim();
			ContentType = ContentType?.Trim();
		}
	}
}
