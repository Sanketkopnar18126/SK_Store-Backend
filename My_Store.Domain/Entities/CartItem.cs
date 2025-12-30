using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; private set; }
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;
        public int ProductId { get; private set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; } // price snapshot
        public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

        public CartItem() { }

        public CartItem(int productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0", nameof(quantity));
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            AddedAt = DateTime.UtcNow;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0", nameof(quantity));
            Quantity = quantity;
        }

        public void SetUnitPrice(decimal unitPrice)
        {
            UnitPrice = unitPrice;
        }
    }
}
