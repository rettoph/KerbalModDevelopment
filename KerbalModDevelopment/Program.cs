// See https://aka.ms/new-console-template for more information
using KerbalModDevelopment.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

IConfiguration configuration = new ConfigurationBuilder()
 .AddJsonFile("appsettings.json")
 .Build();

var loggerConfiguration = new LoggerConfiguration();
loggerConfiguration.MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose);
loggerConfiguration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

IServiceProvider provider = new ServiceCollection()
    .AddSingleton(configuration)
    .AddSingleton<SettingService>()
    .AddSingleton<DirectoryService>()
    .AddSingleton<GameInstallationService>()
    .AddSingleton<EnvironmentService>()
    .AddSingleton<ModBuildService>()
    .AddSingleton<LaunchService>()
    .AddSingleton<ILogger>(loggerConfiguration.CreateLogger())
    .BuildServiceProvider();

LaunchService launcher = provider.GetRequiredService<LaunchService>();
launcher.TryLaunch();