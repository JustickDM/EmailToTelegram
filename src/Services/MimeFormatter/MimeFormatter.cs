using EmailToTelegram.Extentions;
using EmailToTelegram.Helpers;

using MimeKit;

using NUglify.Helpers;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace EmailToTelegram.Services.MimeFormatter
{
	internal sealed class MimeFormatter : IMimeFormatter
	{
		public string Format([NotNull] MimeMessage mimeMessage, [MaybeNull] CultureInfo? cultureInfo)
		{
			mimeMessage.ThrowIfNull();

			var stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"*{mimeMessage.Subject}*");

			try
			{
				stringBuilder.AppendLine(((TextPart)mimeMessage.Body).Text);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				// Unable to cast object of type 'MimeKit.MultipartAlternative' to type 'MimeKit.TextPart'.
				stringBuilder.AppendLine(mimeMessage.HtmlBody.GetTextFromHtml());
			}

			var result = stringBuilder.ToString();

			return result;
		}

		public IReadOnlyList<string> Format([NotNull] IReadOnlyList<MimeMessage> mimeMessages, [MaybeNull] CultureInfo? cultureInfo)
		{
			mimeMessages.ThrowIfNull();

			var result = new List<string>(mimeMessages.Count);

			mimeMessages.ForEach(it => result.Add(Format(it, cultureInfo)));

			return result;
		}
	}
}