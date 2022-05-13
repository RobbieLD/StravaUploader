// Service - https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service

using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .Build();