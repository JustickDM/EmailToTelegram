using EmailToTelegram.Extentions;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace EmailToTelegram.Models
{
	internal sealed class Settings(string filePath)
	{
		private readonly static JsonSerializerOptions m_jsonSerializerOptions = new() { WriteIndented = true };

		[NotNull]
		public string FilePath { get; set; } = filePath.ThrowIfNullOrWhiteSpace();

		#region Telegram

		[NotNull]
		public string TelegramBotAccessToken { get; set; } = string.Empty;

		#endregion

		#region Pop3

		[NotNull]
		public IReadOnlyList<Pop3Settings> Pop3Settings { get; set; } = [];

		#endregion

		#region Imap

		[NotNull]
		public IReadOnlyList<ImapSettings> ImapSettings { get; set; } = [];

		#endregion

		/// <summary>
		/// Seconds
		/// </summary>
		public int CheckingNewMessagesDelaySeconds { get; set; } = 60;

		public static Settings Load([NotNull] string filePath)
		{
			if (!filePath.IsExistFile())
			{
				var newSettings = new Settings(filePath);
				var newSettingsJson = JsonSerializer.Serialize(newSettings);

				File.WriteAllText(filePath, newSettingsJson);
			}

			var fileText = File.ReadAllText(filePath);
			var settings = JsonSerializer.Deserialize<Settings>(fileText);

			if (settings is not null)
				settings.FilePath = filePath;
			else
				throw new FileNotFoundException(filePath);

			settings.TelegramBotAccessToken.ThrowIfNullOrWhiteSpace();

			settings.Pop3Settings.ThrowIfNull();
			settings.ImapSettings.ThrowIfNull();

			return settings;
		}

		public static void Save([NotNull] string filePath, [NotNull] Settings settings)
		{
			if (!filePath.IsExistFile())
				throw new FileNotFoundException(string.Format("The settings file was not found in the directory: {0}", filePath));

			settings.ThrowIfNull();

			var updatedSettings = JsonSerializer.Serialize(settings, m_jsonSerializerOptions);

			File.WriteAllText(filePath, updatedSettings);
		}
	}
}