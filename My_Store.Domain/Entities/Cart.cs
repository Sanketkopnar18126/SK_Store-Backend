using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Entities
{
    public class Cart
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }              // owner
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; private set; }

        public readonly List<CartItem> _items = new();
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public Cart(int userId)
        {
            UserId = userId;
        }

        // EF parameterless ctor
        protected Cart() { }

        public void AddOrUpdateItem(int productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0", nameof(quantity));
            var existing = _items.Find(i => i.ProductId == productId);
            if (existing == null)
            {
                _items.Add(new CartItem(productId, quantity, unitPrice));
            }
            else
            {
                existing.SetQuantity(existing.Quantity + quantity);
                existing.SetUnitPrice(unitPrice); // refresh snapshot price if you want
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetItemQuantity(int productId, int quantity)
        {
            var existing = _items.Find(i => i.ProductId == productId);
            if (existing == null) return;
            if (quantity <= 0)
                _items.Remove(existing);
            else
                existing.SetQuantity(quantity);

            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveItem(int productId)
        {
            var existing = _items.Find(i => i.ProductId == productId);
            if (existing != null) _items.Remove(existing);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Clear()
        {
            _items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public decimal GetTotal()
        {
            decimal total = 0;
            foreach (var it in _items)
                total += it.Quantity * it.UnitPrice;
            return total;
        }
    }
}

