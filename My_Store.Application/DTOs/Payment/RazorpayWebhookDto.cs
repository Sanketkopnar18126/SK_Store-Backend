using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Application.DTOs.Payment
{
    public class RazorpayWebhookDto
    {
        public string Event { get; set; } = string.Empty;
        public RazorpayPayload Payload { get; set; } = default!;
    }

    public class RazorpayPayload
    {
        public RazorpayPaymentEntity Payment { get; set; } = default!;
    }

    public class RazorpayPaymentEntity
    {
        public RazorpayPaymentData Entity { get; set; } = default!;
    }

    public class RazorpayPaymentData
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
