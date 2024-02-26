using MimeKit;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace EmailToTelegram.Services.MimeFormatter
{
	internal interface IMimeFormatter
	{
		string Format([NotNull] MimeMessage mimeMessage, [MaybeNull] CultureInfo? cultureInfo);

		IReadOnlyList<string> Format([NotNull] IReadOnlyList<MimeMessage> mimeMessages, [MaybeNull] CultureInfo? cultureInfo);
	}
}