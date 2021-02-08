using System;
using System.Collections.Generic;
using System.IO;

namespace Isap.CommonCore.Extensions
{
	public static class PathUtility
	{
		public static string MakeRelativePath(string baseDir, string path)
		{
			string currentPathPlaceholder = String.Concat(".", Path.DirectorySeparatorChar);
			string parentPathPlaceholder = String.Concat("..", Path.DirectorySeparatorChar);

			if (String.Compare(Path.GetPathRoot(baseDir), Path.GetPathRoot(path), StringComparison.OrdinalIgnoreCase) != 0)
				return path;
			var baseDirSegments =
				new List<string>(baseDir.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
			var pathSegments =
				new List<string>(path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
			string result = currentPathPlaceholder;
			while (baseDirSegments.Count > 0
				&& String.Compare(baseDirSegments[0], pathSegments[0], StringComparison.OrdinalIgnoreCase) == 0)
			{
				baseDirSegments.RemoveAt(0);
				pathSegments.RemoveAt(0);
			}

			while (baseDirSegments.Count > 0)
			{
				result = String.Concat(result, parentPathPlaceholder);
				baseDirSegments.RemoveAt(0);
			}

			while (pathSegments.Count > 1)
			{
				result = String.Concat(result, pathSegments[0], Path.DirectorySeparatorChar);
				pathSegments.RemoveAt(0);
			}

			result = String.Concat(result, pathSegments[0]);
			result =
				result.Replace(String.Concat(currentPathPlaceholder, parentPathPlaceholder), parentPathPlaceholder);
			return result;
		}

		public static string Resolve(string absoluteOrRelativePath, Func<string> getBaseDir)
		{
			if (Path.IsPathRooted(absoluteOrRelativePath))
				return absoluteOrRelativePath;
			string baseDir = getBaseDir();
			return Path.GetFullPath(Path.Combine(baseDir, absoluteOrRelativePath));
		}

		public static string Resolve(string absoluteOrRelativePath, string baseDir)
		{
			return Resolve(absoluteOrRelativePath, () => baseDir);
		}
	}
}
