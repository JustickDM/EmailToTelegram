using EmailToTelegram.Events;
using EmailToTelegram.Extentions;
using EmailToTelegram.Models.Generics;

using MailKit;
using MailKit.Net.Imap;

using MimeKit;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Services.ImapService
{
	internal sealed class ImapService : IImapService
	{
		private readonly string _host;
		private readonly int _port;
		private readonly string _login;
		private readonly string _password;
		private readonly bool _useSsl;

		[NotNull]
		public string ServiceName { get; }

		public event EventHandler<ItemEventArgs<MimeMessage>>? MimeMessageDownloaded;
		public event EventHandler<ItemsEventArgs<MimeMessage>>? MimeMessagesDownloaded;

		public ImapService(
			[NotNull] string host,
			int port,
			[NotNull] string login,
			[NotNull] string password,
			bool useSsl = true)
		{
			_host = host.ThrowIfNullOrWhiteSpace();
			_port = port.ThrowIfLessThanOrEqualZero();
			_login = login.ThrowIfNullOrWhiteSpace();
			_password = password.ThrowIfNullOrWhiteSpace();
			_useSsl = useSsl;

			ServiceName = string.Format("{0}:{1} (UseSsl={2})", _host, _port, _useSsl);
		}

		public async Task<SingleResult<int>> GetMessagesCountAsync(CancellationToken cancelToken = default)
		{
			var result = new SingleResult<int>();
			var client = new ImapClient();

			try
			{
				await client.ConnectAsync(_host, _port, _useSsl, cancelToken);
				await client.AuthenticateAsync(_login, _password, cancelToken);

				var inbox = client.Inbox;
				inbox.Open(FolderAccess.ReadOnly, cancelToken);

				result.Item = inbox.Count;
			}
			catch (Exception ex)
			{
				result.Exception = ex;
			}
			finally
			{
				await client.DisconnectAsync(true, cancelToken);

				client?.Dispose();
			}

			return result;
		}

		public async Task<ListResult<MimeMessage>> GetMessagesAsync(
			int startMessageIndex = 0,
			CancellationToken cancelToken = default)
		{
			var result = new ListResult<MimeMessage>();
			var client = new ImapClient();

			try
			{
				await client.ConnectAsync(_host, _port, _useSsl, cancelToken);
				await client.AuthenticateAsync(_login, _password, cancelToken);

				var inbox = client.Inbox;
				inbox.Open(FolderAccess.ReadOnly, cancelToken);

				var currentMessagesCount = inbox.Count;
				var lastMessageIndex = currentMessagesCount == 0 ? 0 : currentMessagesCount - 1;

				if (currentMessagesCount > 0 && lastMessageIndex >= startMessageIndex)
				{
					var count = lastMessageIndex - startMessageIndex + 1;
					var localResult = new List<MimeMessage>(count);

					for (var i = 0; i < count; i++)
					{
						var message = await inbox.GetMessageAsync(startMessageIndex + i, cancelToken);

						localResult.Add(message);

						MimeMessageDownloaded?.Invoke(this, new ItemEventArgs<MimeMessage>
						{
							Item = message
						});
					}

					result.Items = localResult;

					MimeMessagesDownloaded?.Invoke(this, new ItemsEventArgs<MimeMessage>
					{
						Items = localResult
					});
				}
			}
			catch (Exception ex)
			{
				result.Exception = ex;
			}
			finally
			{
				await client.DisconnectAsync(true, cancelToken);

				client?.Dispose();
			}

			return result;
		}
	}
}