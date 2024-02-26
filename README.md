# EmailToTelegram

## Configuring the file Settings.json

**FilePath** is set automatically from appsettings.json, [SettingsFilePath](https://github.com/JustickDM/EmailToTelegram/blob/master/src/appsettings.json#L8)

```json
{
	"FilePath": "<Path to Settings.json>",
	"TelegramBotAccessToken": "<TOKEN>",
	"Pop3Settings": [
		{
			"Host": "",
			"NoSslPort": 0,
			"SslPort": 0,
			"UseSsl": true,
			"Login": "",
			"Password": "",
			"StartMessageIndex": 0,
			"CommonTelegramChatId": "<CHAT_ID>",
			"CommonTelegramMessageThreadId": null,
			"MessageFilters": [
				{
					"TelegramChatId": "<CHAT_ID>",
					"TelegramMessageThreadId": null,
					"MessageSubjectRegexPattern": "<SUBJECT_REGEX_PATTERN>"
				}
			]
		}
  	],
	"ImapSettings": [
		{
			"Host": "",
			"NoSslPort": 0,
			"SslPort": 0,
			"UseSsl": true,
			"Login": "",
			"Password": "",
			"StartMessageIndex": 0,
			"CommonTelegramChatId": "<CHAT_ID>",
			"CommonTelegramMessageThreadId": null,
			"MessageFilters": [
				{
					"TelegramChatId": "<CHAT_ID>",
					"TelegramMessageThreadId": null,
					"MessageSubjectRegexPattern": "<SUBJECT_REGEX_PATTERN>"
				}
			]
		}
	],
  	"CheckingNewMessagesDelaySeconds": 60
}
```
