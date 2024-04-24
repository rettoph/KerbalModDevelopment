// See https://aka.ms/new-console-template for more information
using KerbalModDevelopment.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfiguration configuration = new ConfigurationBuilder()
 .AddJsonFile("appsettings.json")
 .Build();

IServiceProvider provider = new ServiceCollection()
    .AddSingleton(configuration)
    .AddSingleton<SettingService>()
    .AddSingleton<GameInstallationService>()
    .BuildServiceProvider();

var gameInstallation = provider.GetRequiredService<GameInstallationService>();

gameInstallation.Verify(false);

Console.ReadLine();