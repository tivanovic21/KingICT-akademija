using System.Text.Json;
using System.Text.Json.Serialization;
using KingICT.Data;
using KingICT.DTO;
using KingICT.Models;
using Microsoft.EntityFrameworkCore;

namespace KingICT.Services
{
    public class ProductRepository : IProductRepository
	{
        private readonly ApplicationContext _context;
        private readonly HttpClient _httpClient;

        public ProductRepository(ApplicationContext context, HttpClient httpClient)
		{
            _context = context;
            _httpClient = httpClient;
		}

        public async Task<List<Products>> FilterProducts(string? category, double? price)
        {
            if (string.IsNullOrEmpty(category) && !price.HasValue) return null;

            var products = await GetProducts();
            if(string.IsNullOrEmpty(category) && price.HasValue)
            {
                return products.Where(p => p.Price <= price)
                .ToList();
            } else if(!string.IsNullOrEmpty(category) && !price.HasValue)
            {
                return products.Where(p => p.Category.Contains(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
            } else
            {
                return products.Where(p => p.Category.Contains(category, StringComparison.OrdinalIgnoreCase) && p.Price <= price).ToList();
            }
        }


        public async Task<Products> GetProductById(int id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true,
            };

            var productsResponse = JsonSerializer.Deserialize<Products>(jsonResponse, options);
            return productsResponse;
        }

        public async Task<List<Products>> GetProducts()
        {
            var response = await _httpClient.GetAsync("products");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true,
            };

            var productsResponse = JsonSerializer.Deserialize<ProductsDTO>(jsonResponse, options);
            return productsResponse.Products;
        }


        public async Task<List<Products>> GetProductsByName(string title)
        {
            var products = await GetProducts();
            var filteredProducts = products
                .Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return filteredProducts;
        }
    }
}

