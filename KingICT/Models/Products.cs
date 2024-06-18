using System;

namespace KingICT.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public string Thumbnail { get; set; }
    }
}