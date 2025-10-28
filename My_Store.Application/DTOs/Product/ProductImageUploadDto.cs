using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Application.DTOs.Product
{
    public class ProductImageUploadDto
    {
        public string FileName { get; set; } = null!;
        public byte[] Content { get; set; } = null!;

    }
}
