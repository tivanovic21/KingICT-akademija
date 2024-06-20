﻿using System;
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

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<List<Products>>> GetProducts()
        {
            var products = await _productRepository.GetProducts();

            if (products == null) return NotFound();

            return Ok(products);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<Products>> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product == null) return NotFound();

            return Ok(product);
        }

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

            if (products == null) return NotFound();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set(cacheKey, products, cacheEntryOptions);

            _logger.Log(LogLevel.Information, "Returning data from API!");
            return Ok(products);
        }

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

            if (products == null) return NotFound();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set(cacheKey, products, cacheEntryOptions);

            _logger.Log(LogLevel.Information, "Returning data from API!");
            return Ok(products);
        }
    }
}

