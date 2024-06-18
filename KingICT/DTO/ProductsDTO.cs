using System;
using KingICT.Models;
using System.Text.Json.Serialization;

namespace KingICT.DTO
{
	public class ProductsDTO
	{
        [JsonPropertyName("products")]
        public List<Products> Products { get; set; }
    }
}

