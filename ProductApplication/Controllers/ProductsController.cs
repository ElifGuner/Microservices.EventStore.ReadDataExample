using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProductApplication.Models.ViewModels;
using Shared.Events;
using Shared.Services;
using Shared.Services.Abstractions;
using Shared.Models;

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
    }
}
