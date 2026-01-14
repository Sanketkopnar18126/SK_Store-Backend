using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Enums
{
    public enum OrderStatus
    {
        Created = 1,
        PaymentPending = 2,
        Paid = 3,
        Processing = 4,
        Shipped = 5,
        Delivered = 6,
        Cancelled = 7,
        PaymentFailed = 8
    }
}
