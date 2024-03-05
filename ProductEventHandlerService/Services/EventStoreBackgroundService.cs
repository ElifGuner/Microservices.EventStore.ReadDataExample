using EventStore.Client;
using MongoDB.Driver;
using Shared.Models;
using Shared.Events;
using Shared.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace ProductEventHandlerService.Services
{
    public class EventStoreBackgroundService : BackgroundService
    {
        private IEventStoreService _eventStoreService;
        private IMongoDBService _mongoDBService;

        public EventStoreBackgroundService(IEventStoreService eventStoreService, IMongoDBService mongoDBService)
        {
            _eventStoreService = eventStoreService;
            _mongoDBService = mongoDBService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreService.SubscribeToStreamAsync("products-stream",
                async (streamSubscription, resolvedEvent, cancellationToken) =>
                {
                    string eventType = resolvedEvent.Event.EventType;
                    object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(),
                        Assembly.Load("Shared").GetTypes().FirstOrDefault(t => t.Name == eventType));
                    //Shared içindeki eventType'a karşılık olan tür hangisiyse, sen buradaki binary datayı o türden instance'a dönüştür.
                    //Aşağıdaki çalışmıyor, muhtemelen full name'e bakıyor.
                    //Assembly.Load("Shared").GetTypes()
                    var productCollection = _mongoDBService.GetCollection<Product>("Products");

                    switch (@event)
                    {
                        case NewProductAddedEvent e:
                            var hasProduct = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).AnyAsync();
                            if (!hasProduct)
                                await productCollection.InsertOneAsync(new Product()
                                {
                                    Id = e.ProductId,
                                    ProductName = e.ProductName,
                                    Count = e.InitialCount,
                                    IsAvailable = e.IsAvailable,
                                    Price = e.InitialPrice
                                });

                            break;
                    }
                }
                );

        }
    }
}
