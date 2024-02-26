using EmailToTelegram;
using EmailToTelegram.Services.MimeFormatter;
using EmailToTelegram.Services.SenderService;

using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

using System.Runtime.InteropServices;

GlobalMethods.SetDefaultThreadCurrentCulture(GlobalMethods.GetRuCulture());

var builder = Host.CreateApplicationBuilder(args);

#if DEBUG
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.local.json", true, true);
#endif

builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IMimeFormatter, MimeFormatter>();
builder.Services.AddSingleton<ISenderService, SenderService>();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
	builder.Services.AddWindowsService();

	LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
}
else
{
	builder.Services.AddSystemd();
}

var host = builder.Build();

host.Run();