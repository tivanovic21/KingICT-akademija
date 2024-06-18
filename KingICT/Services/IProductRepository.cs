using System;
using KingICT.Models;

namespace KingICT.Services
{
    public interface IProductRepository
    {
        Task<List<Products>> GetProducts();
        Task<Products> GetProductById(int id);
        Task<List<Products>> FilterProducts(string? category, double? price);
        Task<List<Products>> GetProductsByName(string title);
    }
}

