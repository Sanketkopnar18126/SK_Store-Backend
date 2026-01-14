using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Entities
{
    public class Banner
    {
        public int Id { get; private set; }
        public string ImageUrl { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public Banner(string imageUrl, string? title = null, string? description = null)
        {
            ImageUrl = imageUrl;
            CreatedAt = DateTime.UtcNow;
        }
        protected Banner() { }
    }
}
