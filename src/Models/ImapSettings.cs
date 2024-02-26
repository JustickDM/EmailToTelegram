namespace EmailToTelegram.Models
{
	internal sealed record ImapSettings
	{
		public required string Host { get; set; } = string.Empty;

		public required int NoSslPort { get; set; }

		public required int SslPort { get; set; }

		public required bool UseSsl { get; set; } = true;

		public required string Login { get; set; } = string.Empty;

		public required string Password { get; set; } = string.Empty;

		public required int StartMessageIndex { get; set; }

		public required string CommonTelegramChatId { get; set; } = string.Empty;

		public required int? CommonTelegramMessageThreadId { get; set; }

		public required IReadOnlyList<MessageFilter> MessageFilters { get; set; } = [];
	}
}