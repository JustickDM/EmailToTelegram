using EmailToTelegram.Extentions;
using EmailToTelegram.Models;
using EmailToTelegram.Services.MimeFormatter;
using EmailToTelegram.Services.TelegramBot;

using MimeKit;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EmailToTelegram.Services.SenderService
{
	internal sealed class SenderService(IMimeFormatter mimeFormatter) : ISenderService
	{
		private readonly IMimeFormatter _mimeFormatter = mimeFormatter.ThrowIfNull();

		public async Task TelegramBotSendMessagesAsync(
			[NotNull] ITelegramBot telegramBot,
			[NotNull] IReadOnlyList<MessageFilter> messageFilters,
			[NotNull] IReadOnlyList<MimeMessage> messages,
			[NotNull] string commonTelegramChatId,
			int? commonTelegramMessageThreadId,
			CancellationToken cancelToken = default)
		{
			telegramBot.ThrowIfNull();
			messageFilters.ThrowIfNull();
			messages.ThrowIfNull();
			commonTelegramChatId.ThrowIfNullOrWhiteSpace();

			if (messageFilters.Any())
				foreach (var messageFilter in messageFilters)
				{
					var targetMessages = new List<MimeMessage>(messages.Count);

					foreach (var message in messages)
					{
						var isMatchResult = Regex.IsMatch(message.Subject, @$"{messageFilter.MessageSubjectRegexPattern}");

						if (isMatchResult)
							targetMessages.Add(message);
					}

					var formattedMessages = _mimeFormatter.Format(targetMessages, CultureInfo.CurrentCulture);

					await telegramBot.SendMessagesAsync(
							messageFilter.TelegramChatId,
							formattedMessages,
							messageFilter.TelegramMessageThreadId,
							cancelToken);
				}
			else
			{
				if (!messages.Any())
					return;

				var formattedMessages = _mimeFormatter.Format(messages, CultureInfo.CurrentCulture);

				await telegramBot.SendMessagesAsync(
					commonTelegramChatId,
					formattedMessages,
					commonTelegramMessageThreadId,
					cancelToken);
			}
		}
	}
}