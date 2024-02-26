using EmailToTelegram.Extentions;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace EmailToTelegram
{
	internal static class GlobalMethods
	{
		public static void SetDefaultThreadCurrentCulture([NotNull] CultureInfo cultureInfo)
		{
			cultureInfo.ThrowIfNull();

			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
		}

		public static CultureInfo GetRuCulture()
			=> CultureInfo.GetCultureInfo("ru-RU");

		public static CultureInfo GetEnCulture()
			=> CultureInfo.GetCultureInfo("en-US");

		public static string GetDateStringFormat()
			=> CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

		public static string GetTimeStringFormat()
			=> CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
	}
}