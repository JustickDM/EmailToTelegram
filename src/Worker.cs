using EmailToTelegram.Events;
using EmailToTelegram.Extentions;
using EmailToTelegram.Factories;
using EmailToTelegram.Models;
using EmailToTelegram.Services.ImapService;
using EmailToTelegram.Services.Pop3Service;
using EmailToTelegram.Services.SenderService;
using EmailToTelegram.Services.TelegramBot;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram
{
	internal sealed class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly IConfiguration _configuration;
		private readonly ISenderService _senderService;

		private readonly IReadOnlyList<IPop3Service> _pop3Services;
		private readonly IReadOnlyList<IImapService> _imapServices;

		private readonly ITelegramBot _telegramBot;

		private Settings _settings;

		public Worker(
			[NotNull] ILogger<Worker> logger,
			[NotNull] IConfiguration configuration,
			[NotNull] ISenderService senderService)
		{
			_logger = logger.ThrowIfNull();
			_configuration = configuration.ThrowIfNull();
			_senderService = senderService.ThrowIfNull();

			var settingsFilePath = _configuration
				.GetValue<string>("SettingsFilePath")
				.ThrowIfNullOrWhiteSpace();

			_settings = Settings.Load(settingsFilePath);

			_pop3Services = Pop3ServiceFactory.Get(_settings.Pop3Settings);
			_imapServices = ImapServiceFactory.Get(_settings.ImapSettings);

			_telegramBot = TelegramBotFactory.Get(_settings.TelegramBotAccessToken);
			_telegramBot.ExceptionHandled += TelegramBotHandleException;
		}

		protected override async Task ExecuteAsync(CancellationToken cancelToken)
		{
			var result = await GetMeTelegramBotAsync(cancelToken);

			if (!result)
				return;

			TelegramBotStartReceiving(cancelToken);

			await InitPop3ServiceStartMessageIndexAsync(cancelToken);
			await InitImapServiceStartMessageIndexAsync(cancelToken);

			while (!cancelToken.IsCancellationRequested)
			{
				if (_logger.IsEnabled(LogLevel.Information))
					_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

				await Task.Delay(TimeSpan.FromSeconds(_settings.CheckingNewMessagesDelaySeconds), cancelToken);

				await Pop3ToTelegramAsync(cancelToken);
				await ImapToTelegramAsync(cancelToken);
			}

			_telegramBot.ExceptionHandled -= TelegramBotHandleException;
		}

		#region Private methods

		private void TelegramBotHandleException(object? sender, ItemEventArgs<Exception> args)
		{
			if (_logger.IsEnabled(LogLevel.Critical))
				_logger.LogCritical(
					"Exception: {exception}.\nTelegramBot: {telegramBot}",
					args.Item, sender);
		}

		private async Task<bool> GetMeTelegramBotAsync(CancellationToken cancelToken = default)
		{
			bool result;

			var localResult = await _telegramBot.GetMeAsync(cancelToken);

			if (localResult.IsSuccess)
			{
				result = true;

				if (localResult.IsNotEmpty && _logger.IsEnabled(LogLevel.Warning))
					_logger.LogWarning("TelegramBot: {me}", localResult.Item?.ToString());
			}
			else
			{
				result = false;

				if (_logger.IsEnabled(LogLevel.Critical))
					_logger.LogCritical("TelegramBot Exception: {exception}.", localResult.Exception);
			}

			return result;
		}

		private void TelegramBotStartReceiving(CancellationToken cancelToken = default)
		{
			_telegramBot.StartReceiving(cancelToken);

			if (_logger.IsEnabled(LogLevel.Warning))
				_logger.LogWarning("TelegramBot start receiving running at: {time}", DateTimeOffset.Now);
		}

		#region Pop3

		private async Task InitPop3ServiceStartMessageIndexAsync(CancellationToken cancelToken = default)
		{
			if (!_pop3Services.Any())
				return;

			for (var i = 0; i < _pop3Services.Count; i++)
			{
				var service = _pop3Services[i];
				var serviceSettings = _settings.Pop3Settings[i];

				if (serviceSettings.StartMessageIndex > 0)
					continue;

				var result = await service.GetMessagesCountAsync(cancelToken);

				if (result.IsSuccess)
				{
					if (!result.IsNotEmpty)
						continue;

					serviceSettings.StartMessageIndex = result.Item;

					Settings.Save(_settings.FilePath, _settings);

					if (_logger.IsEnabled(LogLevel.Warning))
						_logger.LogWarning(
							"Pop3Service: {pop3ServiceName} (StartMessageIndex={startMessageIndex})",
							service.ServiceName, result.Item);

				}
				else
					if (_logger.IsEnabled(LogLevel.Error))
					_logger.LogError(
						"Exception: {exception}.\nPop3Service: {serviceName}",
						result.Exception, service.ServiceName);
			}

			_settings = Settings.Load(_settings.FilePath);
		}

		private async Task Pop3ToTelegramAsync(CancellationToken cancelToken = default)
		{
			if (!_pop3Services.Any())
				return;

			for (var i = 0; i < _pop3Services.Count; i++)
			{
				var service = _pop3Services[i];
				var serviceSettings = _settings.Pop3Settings[i];

				if (_logger.IsEnabled(LogLevel.Warning))
					_logger.LogWarning("Pop3Service: {serviceName}", service.ServiceName);

				var result = await service.GetMessagesAsync(serviceSettings.StartMessageIndex, cancelToken);

				if (result.IsSuccess)
				{
					if (!result.IsNotEmpty)
						continue;

					serviceSettings.StartMessageIndex += result.Count;

					await _senderService.TelegramBotSendMessagesAsync(
						_telegramBot,
						serviceSettings.MessageFilters,
						result.Items,
						serviceSettings.CommonTelegramChatId,
						serviceSettings.CommonTelegramMessageThreadId,
						cancelToken);

					Settings.Save(_settings.FilePath, _settings);
				}
				else
					if (_logger.IsEnabled(LogLevel.Error))
					_logger.LogError(
						"Exception: {exception}.\nPop3Service: {serviceName}",
						result.Exception, service.ServiceName);
			}

			_settings = Settings.Load(_settings.FilePath);
		}

		#endregion

		#region Imap

		private async Task InitImapServiceStartMessageIndexAsync(CancellationToken cancelToken = default)
		{
			if (!_imapServices.Any())
				return;

			for (var i = 0; i < _imapServices.Count; i++)
			{
				var service = _imapServices[i];
				var serviceSettings = _settings.ImapSettings[i];

				if (serviceSettings.StartMessageIndex > 0)
					continue;

				var result = await service.GetMessagesCountAsync(cancelToken);

				if (result.IsSuccess)
				{
					if (!result.IsNotEmpty)
						continue;

					serviceSettings.StartMessageIndex = result.Item;

					Settings.Save(_settings.FilePath, _settings);

					if (_logger.IsEnabled(LogLevel.Warning))
						_logger.LogWarning(
							"ImapService: {serviceName} (StartMessageIndex={startMessageIndex})",
							service.ServiceName, result.Item);

				}
				else
					if (_logger.IsEnabled(LogLevel.Error))
					_logger.LogError(
						"Exception: {exception}.\nImapService: {serviceName}",
						result.Exception, service.ServiceName);
			}

			_settings = Settings.Load(_settings.FilePath);
		}

		private async Task ImapToTelegramAsync(CancellationToken cancelToken = default)
		{
			if (!_imapServices.Any())
				return;

			for (var i = 0; i < _imapServices.Count; i++)
			{
				var service = _imapServices[i];
				var serviceSettings = _settings.ImapSettings[i];

				if (_logger.IsEnabled(LogLevel.Warning))
					_logger.LogWarning("ImapService: {serviceName}", service.ServiceName);

				var result = await service.GetMessagesAsync(serviceSettings.StartMessageIndex, cancelToken);

				if (result.IsSuccess)
				{
					if (!result.IsNotEmpty)
						continue;

					serviceSettings.StartMessageIndex += result.Count;

					await _senderService.TelegramBotSendMessagesAsync(
						_telegramBot,
						serviceSettings.MessageFilters,
						result.Items,
						serviceSettings.CommonTelegramChatId,
						serviceSettings.CommonTelegramMessageThreadId,
						cancelToken);

					Settings.Save(_settings.FilePath, _settings);
				}
				else
					if (_logger.IsEnabled(LogLevel.Error))
					_logger.LogError(
						"Exception: {exception}.\nImapService: {serviceName}",
						result.Exception, service.ServiceName);
			}

			_settings = Settings.Load(_settings.FilePath);
		}

		#endregion

		#endregion
	}
}