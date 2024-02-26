using EmailToTelegram.Events;
using EmailToTelegram.Extentions;
using EmailToTelegram.Models.Generics;

using MailKit.Net.Pop3;

using MimeKit;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Services.Pop3Service
{
	internal sealed class Pop3Service : IPop3Service
	{
		private readonly string _host;
		private readonly int _port;
		private readonly string _login;
		private readonly string _password;
		private readonly bool _useSsl;

		[NotNull]
		public string ServiceName { get; }

		public event EventHandler<ItemsEventArgs<MimeMessage>>? MimeMessagesDownloaded;

		public Pop3Service(
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
			var client = new Pop3Client();

			try
			{
				await client.ConnectAsync(_host, _port, _useSsl, cancelToken);
				await client.AuthenticateAsync(_login, _password, cancelToken);

				result.Item = client.Count;
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
			var client = new Pop3Client();

			try
			{
				await client.ConnectAsync(_host, _port, _useSsl, cancelToken);
				await client.AuthenticateAsync(_login, _password, cancelToken);

				var currentMessagesCount = client.Count;
				var lastMessageIndex = currentMessagesCount == 0 ? 0 : currentMessagesCount - 1;

				if (currentMessagesCount > 0 && lastMessageIndex >= startMessageIndex)
				{
					var count = lastMessageIndex - startMessageIndex + 1;
					var messages = await client.GetMessagesAsync(startMessageIndex, count, cancelToken);
					var localResult = new List<MimeMessage>(count);

					localResult.AddRange(messages);

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