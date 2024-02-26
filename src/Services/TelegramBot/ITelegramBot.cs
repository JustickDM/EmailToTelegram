using EmailToTelegram.Events;
using EmailToTelegram.Models.Generics;

using System.Diagnostics.CodeAnalysis;

using Telegram.Bot.Types;

namespace EmailToTelegram.Services.TelegramBot
{
	internal interface ITelegramBot
	{
		event EventHandler<ItemEventArgs<Exception>>? ExceptionHandled;
		event EventHandler<ItemEventArgs<Message>>? MessageSended;
		event EventHandler<ItemsEventArgs<Message>>? MessagesSended;

		void StartReceiving(CancellationToken cancelToken = default);

		Task<SingleResult<User?>> GetMeAsync(CancellationToken cancelToken = default);

		Task<ListResult<Message>> SendMessagesAsync(
			[NotNull] string chatId,
			[NotNull] IReadOnlyList<string> messages,
			int? messageThreadId = null,
			CancellationToken cancelToken = default);
	}
}