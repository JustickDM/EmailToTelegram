using NUglify;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Helpers
{
	internal static class HtmlHelpers
	{
		public static string? GetTextFromHtml([MaybeNull] this string? html)
		{
			if (string.IsNullOrWhiteSpace(html))
				return null;

			var text = Uglify.HtmlToText(html).Code;

			return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
		}
	}
}