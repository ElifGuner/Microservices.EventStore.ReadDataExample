using ProductEventHandlerService;
using Shared.Services.Abstractions;
using Shared;
using Shared.Services;
using ProductEventHandlerService.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<EventStoreBackgroundService>();
        services.AddSingleton<IEventStoreService, EventStoreService>();
        services.AddSingleton<IMongoDBService, MongoDBService>();
    })
    .Build();

host.Run();
