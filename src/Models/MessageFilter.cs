namespace EmailToTelegram.Models
{
	internal sealed record MessageFilter
	{
		public required string TelegramChatId { get; set; } = string.Empty;

		public required int? TelegramMessageThreadId { get; set; }

		public required string MessageSubjectRegexPattern { get; set; } = string.Empty;
	}
}