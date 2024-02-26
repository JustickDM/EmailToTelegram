using EmailToTelegram.Extentions;
using EmailToTelegram.Services.TelegramBot;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Factories
{
	internal static class TelegramBotFactory
	{
		public static ITelegramBot Get([NotNull] string botAccessToken)
		{
			botAccessToken.ThrowIfNullOrWhiteSpace();

			var result = new TelegramBot(botAccessToken);

			return result;
		}
	}
}