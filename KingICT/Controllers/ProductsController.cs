using System;
using KingICT.Models;
using KingICT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KingICT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository productRepository, IMemoryCache memoryCache, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of all products.
        /// </summary>
        /// <returns>A list of products if found, otherwise NotFound.</returns>
        /// <response code="200">Returns the list of products.</response>
        /// <response code="404">If no products are found.</response>
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<List<Products>>> GetProducts()
        {
            var products = await _productRepository.GetProducts();

            if (products == null || products.Count == 0) return NotFound();

            return Ok(products);
        }

        /// <summary>
        /// Retrieves a product by its unique identifier (Id).
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The product if found, otherwise NotFound.</returns>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<Products>> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product == null) return NotFound();

            return Ok(product);
        }

        /// <summary>
        /// Retrieves a list of products by their title. The results are cached for 5 minutes.
        /// </summary>
        /// <param name="title">The title of the products to search for.</param>
        /// <returns>A list of products if found, otherwise NotFound.</returns>
        /// <response code="200">Returns the list of products.</response>
        /// <response code="404">If no products are found.</response>
        [HttpGet("search")]
        [Produces("application/json")]
        public async Task<ActionResult<List<Products>>> GetProductsByName([FromQuery] string title)
        {
            var cacheKey = $"FilterProducts-{title}";
            if(_memoryCache.TryGetValue(cacheKey, out List<Products> cacheData))
            {
                _logger.Log(LogLevel.Information, "Returning data from cache!");
                return Ok(cacheData);
            }

            var products = await _productRepository.GetProductsByName(title);

            if (products == null || products.Count == 0) return NotFound();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set(cacheKey, products, cacheEntryOptions);

            _logger.Log(LogLevel.Information, "Returning data from API!");
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a list of products filtered by category and/or price. The results are cached for 5 minutes.
        /// </summary>
        /// <param name="category">The category of the products to filter by. This parameter is optional.</param>
        /// <param name="price">The price of the products to filter by. This parameter is optional.</param>
        /// <returns>A list of products if found, otherwise NotFound.</returns>
        /// <response code="200">Returns the list of products.</response>
        /// <response code="404">If no products are found.</response>
        [HttpGet("filter")]
        [Produces("application/json")]
        public async Task<ActionResult<List<Products>>> FilterProducts([FromQuery] string? category, double? price)
        {
            var cacheKey = $"FilterProducts-{category}-{price}";

            if (_memoryCache.TryGetValue(cacheKey, out List<Products> cacheData))
            {
                _logger.Log(LogLevel.Information, "Returning data from cache!");
                return Ok(cacheData);
            }

            var products = await _productRepository.FilterProducts(category, price);

            if (products == null || products.Count == 0) return NotFound();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set(cacheKey, products, cacheEntryOptions);

            _logger.Log(LogLevel.Information, "Returning data from API!");
            return Ok(products);
        }
    }
}

