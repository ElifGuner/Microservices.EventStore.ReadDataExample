using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProductApplication.Models.ViewModels;
using Shared.Events;
using Shared.Services;
using Shared.Services.Abstractions;
using Shared.Models;
using Microsoft.CodeAnalysis;
using Shared;

namespace ProductApplication.Controllers
{
    public class ProductsController : Controller
    {
        private IEventStoreService _eventStoreService;
        private IMongoDBService _mongoDBService;

        public ProductsController(IEventStoreService eventStoreService, IMongoDBService mongoDBService)
        {
            _eventStoreService = eventStoreService;
            _mongoDBService = mongoDBService;
        }

        public async Task<IActionResult> Index()
        {
            var productCollection = _mongoDBService.GetCollection<Product>("Products");
            var products = await (await productCollection.FindAsync(_ => true)).ToListAsync(); // get all products
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM model)
        {
            NewProductAddedEvent newProductAddedEvent = new()
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = model.ProductName,
                InitialCount = model.Count,
                IsAvailable = model.IsAvailable,
                InitialPrice = model.Price
            };

            await _eventStoreService.AppendToStreamAsync("products-stream", new[]
               { _eventStoreService.GenerateEventData(newProductAddedEvent)});

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string productId)
        {
            var productCollection = _mongoDBService.GetCollection<Product>("Products");
            var product = await(await productCollection.FindAsync(p => p.Id == productId)).FirstOrDefaultAsync(); 
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> CountUpdate(Product model, int durum)
        {
            var productCollection = _mongoDBService.GetCollection<Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            // if (product.Count > model.Count)
            if (durum == 1)
            {
                CountDecreasedEvent countDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementCount = model.Count
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[]
                { _eventStoreService.GenerateEventData(countDecreasedEvent)});
            }
            //else if (product.Count < model.Count)
            else if (durum == 0)
            {
                CountIncreasedEvent countIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementCount = model.Count
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[]
                { _eventStoreService.GenerateEventData(countIncreasedEvent)});
            }
             return null;
        }

        [HttpPost]
        public async Task<IActionResult> PriceUpdate(Product model, int durum)
        {
            var productCollection = _mongoDBService.GetCollection<Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            //if (product.Price > model.Price)
            if (durum == 1)
            {
                PriceDecreasedEvent priceDecreasedEvent = new()
                {
                    ProductId = model.Id,
                    DecrementAmount = model.Price
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[]
                { _eventStoreService.GenerateEventData(priceDecreasedEvent)});
            }
            //else if (product.Price < model.Price)
            else if (durum == 0)
            {
                PriceIncreasedEvent priceIncreasedEvent = new()
                {
                    ProductId = model.Id,
                    IncrementAmount = model.Price
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[]
                { _eventStoreService.GenerateEventData(priceIncreasedEvent)});
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> AvailableUpdate(Product model)
        {
            var productCollection = _mongoDBService.GetCollection<Product>("Products");
            var product = await (await productCollection.FindAsync(p => p.Id == model.Id)).FirstOrDefaultAsync();

            if (product.IsAvailable != model.IsAvailable)
            {
                AvailabilityChangedEvent availabilityChangedEvent = new()
                {
                    ProductId = model.Id,
                    IsAvailable = model.IsAvailable
                };
                await _eventStoreService.AppendToStreamAsync("products-stream", new[]
                { _eventStoreService.GenerateEventData(availabilityChangedEvent)});
            }

            return null;
        }
    }
}
