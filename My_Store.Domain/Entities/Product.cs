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

        protected Product() { }
        public Product(string name, string description, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductException("Product name is required.");
            if (price < 0)
                throw new InvalidProductException("Price cannot be negative.");
            if (stock < 0)
                throw new InvalidProductException("Stock cannot be negative.");

            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
        }
    }
}
