using EmailToTelegram.Extentions;
using EmailToTelegram.Models;
using EmailToTelegram.Services.Pop3Service;

using System.Diagnostics.CodeAnalysis;

namespace EmailToTelegram.Factories
{
	internal static class Pop3ServiceFactory
	{
		public static IReadOnlyList<IPop3Service> Get([NotNull] IReadOnlyList<Pop3Settings> settings)
		{
			settings.ThrowIfNullOrNotAny();

			var result = new List<IPop3Service>(settings.Count);

			foreach (var setting in settings)
			{
				var service = new Pop3Service(
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