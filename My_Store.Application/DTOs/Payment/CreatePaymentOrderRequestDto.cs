using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Application.DTOs.Payment
{
    public class CreatePaymentOrderRequestDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }   // Amount in rupees
        public string Currency { get; set; } = "INR";
    }
}
