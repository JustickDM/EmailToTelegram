using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Extentions
{
	internal static class GlobalExtentions
	{
		public static bool IsExistFile([NotNull] this string filePath)
			=> !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);
	}
}