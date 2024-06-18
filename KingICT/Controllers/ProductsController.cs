using System;
using KingICT.Models;
using KingICT.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingICT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<Products>> GetProducts()
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
            var products = await _productRepository.GetProductsByName(title);

            if (products == null) return NotFound();

            return Ok(products);
        }

        [HttpGet("filter")]
        [Produces("application/json")]
        public async Task<ActionResult<List<Products>>> FilterProducts([FromQuery] string? category, double? price)
        {
            var products = await _productRepository.FilterProducts(category, price);

            if (products == null) return NotFound();

            return Ok(products);
        }
    }
}

