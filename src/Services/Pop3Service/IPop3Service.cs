using EmailToTelegram.Events;
using EmailToTelegram.Models.Generics;

using MimeKit;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Services.Pop3Service
{
	internal interface IPop3Service
	{
		[NotNull]
		string ServiceName { get; }

		event EventHandler<ItemsEventArgs<MimeMessage>>? MimeMessagesDownloaded;

		Task<SingleResult<int>> GetMessagesCountAsync(CancellationToken cancelToken = default);

		Task<ListResult<MimeMessage>> GetMessagesAsync(int startMessageIndex = 0, CancellationToken cancelToken = default);
	}
}