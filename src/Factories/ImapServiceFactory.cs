using EmailToTelegram.Extentions;
using EmailToTelegram.Models;
using EmailToTelegram.Services.ImapService;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Factories
{
	internal static class ImapServiceFactory
	{
		public static IReadOnlyList<IImapService> Get([NotNull] IReadOnlyList<ImapSettings> settings)
		{
			settings.ThrowIfNullOrNotAny();

			var result = new List<IImapService>(settings.Count);

			foreach (var setting in settings)
			{
				var service = new ImapService(
					setting.Host,
					setting.UseSsl ? setting.SslPort : setting.NoSslPort,
					setting.Login,
					setting.Password,
					setting.UseSsl);

				result.Add(service);
			}

			return result;
		}
	}
}