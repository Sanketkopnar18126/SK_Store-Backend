using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Domain.Exceptions;

namespace My_Store.Domain.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public string Category { get; private set; } = string.Empty;
        public int? CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public int? UpdatedBy { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string Status { get; private set; } = "published";
        public string[]? ImageUrls { get; private set; }



        protected Product() { }
        public Product(string name, string description, decimal price, int stock, string category, int? createdBy = null)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new InvalidProductException("Product name required") : name;
            Price = price < 0 ? throw new InvalidProductException("Price cannot be negative") : price;
            Stock = stock < 0 ? throw new InvalidProductException("Stock cannot be negative") : stock;
            Category = string.IsNullOrWhiteSpace(category) ? throw new InvalidProductException("Category required") : category;
            Description = description;
            CreatedBy = createdBy;
        }

        public void Update(string name, string description, decimal price, int stock, string category, int? updatedBy = null)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new InvalidProductException("Product name required") : name;
            Price = price < 0 ? throw new InvalidProductException("Price cannot be negative") : price;
            Stock = stock < 0 ? throw new InvalidProductException("Stock cannot be negative") : stock;
            Category = string.IsNullOrWhiteSpace(category) ? throw new InvalidProductException("Category required") : category;
            Description = description;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetImages(string[] urls)
        {
            if (urls != null && urls.Length > 0)
                ImageUrls = urls;

            UpdatedAt = DateTime.UtcNow;
        }

        public void AddImage(string url)
        {
            if (ImageUrls == null)
                ImageUrls = new string[] { url };
            else
                ImageUrls = ImageUrls.Concat(new[] { url }).ToArray();

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
