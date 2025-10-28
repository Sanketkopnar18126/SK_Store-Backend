using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Store.Application.DTOs.Product;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;

namespace My_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;


        public ProductController(IProductService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool groupByCategory = false)
        {
            var result = await _service.GetAllAsync(groupByCategory);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            int adminId=1;
            var created = await _service.CreateAsync(dto,adminId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            int adminId = 1;
            var updated = await _service.UpdateAsync(id, dto, adminId);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }


        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImages( [FromForm] IFormFileCollection files, [FromQuery] int adminId)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded");

            var uploadDtos = files.Select(f => new ProductImageUploadDto
            {
                FileName = f.FileName,
                Content = ReadFully(f.OpenReadStream())
            }).ToList();

            var urls = await _service.UploadImagesAsync(uploadDtos);

            return Ok(new { imageUrls = urls });
        }

        private static byte[] ReadFully(Stream input)
        {
            using var ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }


    }
}
