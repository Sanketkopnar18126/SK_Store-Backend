using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // 🔹 RAZORPAY DATA
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;

        // 🔹 AMOUNT DETAILS
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        // 🔹 STATUS
        public bool IsPaid { get; set; } = false;
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
