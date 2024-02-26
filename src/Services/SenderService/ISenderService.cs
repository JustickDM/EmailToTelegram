using EmailToTelegram.Models;
using EmailToTelegram.Services.TelegramBot;

using MimeKit;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Services.SenderService
{
	internal interface ISenderService
	{
		Task TelegramBotSendMessagesAsync(
			[NotNull] ITelegramBot telegramBot,
			[NotNull] IReadOnlyList<MessageFilter> messageFilters,
			[NotNull] IReadOnlyList<MimeMessage> messages,
			[NotNull] string commonTelegramChatId,
			int? commonTelegramMessageThreadId,
			CancellationToken cancelToken = default);
	}
}