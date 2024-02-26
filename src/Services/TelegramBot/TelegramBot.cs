using EmailToTelegram.Events;
using EmailToTelegram.Extentions;
using EmailToTelegram.Models.Generics;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EmailToTelegram.Services.TelegramBot
{
	internal sealed class TelegramBot : ITelegramBot
	{
		private readonly string _botAccessToken;

		private readonly TelegramBotClient _botClient;

		public event EventHandler<ItemEventArgs<Exception>>? ExceptionHandled;
		public event EventHandler<ItemEventArgs<Message>>? MessageSended;
		public event EventHandler<ItemsEventArgs<Message>>? MessagesSended;

		public TelegramBot([NotNull] string botAccessToken)
		{
			_botAccessToken = botAccessToken.ThrowIfNullOrWhiteSpace();

			_botClient = new TelegramBotClient(_botAccessToken);
		}

		public void StartReceiving(CancellationToken cancelToken = default)
		{
			var receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = []
			};

			_botClient.StartReceiving(
				updateHandler: HandleUpdateAsync,
				pollingErrorHandler: HandlePollingErrorAsync,
				receiverOptions: receiverOptions,
				cancellationToken: cancelToken
			);
		}

		public async Task<SingleResult<User?>> GetMeAsync(CancellationToken cancelToken = default)
		{
			var result = new SingleResult<User?>();

			try
			{
				var me = await _botClient.GetMeAsync(cancelToken);

				result.Item = me;
			}
			catch (Exception ex)
			{
				result.Exception = ex;
			}

			return result;
		}

		public async Task<ListResult<Message>> SendMessagesAsync(
			[NotNull] string chatId,
			[NotNull] IReadOnlyList<string> messages,
			int? messageThreadId = null,
			CancellationToken cancelToken = default)
		{
			chatId.ThrowIfNullOrWhiteSpace();
			messages.ThrowIfNull();

			var result = new ListResult<Message>();
			var localResult = new List<Message>(messages.Count);

			try
			{
				foreach (var message in messages)
				{
					var sendMessage = await _botClient.SendTextMessageAsync(
						chatId: chatId,
						message,
						parseMode: ParseMode.Markdown,
						allowSendingWithoutReply: true,
						replyToMessageId: messageThreadId,
						cancellationToken: cancelToken);

					localResult.Add(sendMessage);

					MessageSended?.Invoke(this, new ItemEventArgs<Message>
					{
						Item = sendMessage
					});
				}

				result.Items = localResult;

				MessagesSended?.Invoke(this, new ItemsEventArgs<Message>
				{
					Items = localResult
				});
			}
			catch (Exception ex)
			{
				result.Exception = ex;
			}

			return result;
		}

		#region Private methods

		private async Task HandleUpdateAsync(
			ITelegramBotClient botClient,
			Update update,
			CancellationToken cancelToken = default)
		{
			if (update.Message is not { } message)
				return;
			if (message.Text is not { } messageText)
				return;

			var chatId = message.Chat.Id;
			var user = $"{message.Chat.FirstName} {message.Chat.LastName} (@{message.Chat.Username})";

			Debug.WriteLine($"Received a '{messageText}' message from {user}, chat {chatId}.");

			var result = await ExecuteCommandAsync(chatId, messageText);

			Debug.WriteLine($"Answer: '{result}'. Message to {user}, chat {chatId}.");

			var sentMessage = await botClient.SendTextMessageAsync(
				chatId: chatId,
				result,
				replyToMessageId: update.Message.MessageId,
				allowSendingWithoutReply: true,
				cancellationToken: cancelToken);
		}

		private Task HandlePollingErrorAsync(
			ITelegramBotClient botClient,
			Exception exception,
			CancellationToken cancelToken = default)
		{
			var errorMessage = exception switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Debug.WriteLine(errorMessage);

			ExceptionHandled?.Invoke(this, new ItemEventArgs<Exception>
			{
				Item = exception
			});

			return Task.CompletedTask;
		}

		private async Task<string> ExecuteCommandAsync(long chatId, string messageText)
		{
			var tcs = new TaskCompletionSource<string>();

			if (messageText.Equals("/start"))
				tcs.TrySetResult($"Hello:) Your ChatId is {chatId}");

			if (messageText.Equals("/help"))
				tcs.TrySetResult($"Comming soon:)");
			else
				tcs.TrySetResult("There is no such command:(");

			var result = await tcs.Task;

			return result;
		}

		#endregion
	}
}