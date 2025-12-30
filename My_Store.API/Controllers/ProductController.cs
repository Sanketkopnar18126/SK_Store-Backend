// My_Store.API/Controllers/ProductController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Store.Application.DTOs.Product;
using My_Store.Application.Interfaces;
using My_Store.Infrastructure.Services;

namespace My_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService service, ILogger<ProductController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        
        public async Task<IActionResult> GetAll([FromQuery] bool groupByCategory = false, CancellationToken ct = default)
        {
            var result = await _service.GetAllAsync(groupByCategory, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var product = await _service.GetByIdAsync(id, ct);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct = default)
        {
         int adminId = 1; 

            var created = await _service.CreateAsync(dto, adminId, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken ct = default)
        {
            int adminId = 1;

            var updated = await _service.UpdateAsync(id, dto, adminId, ct);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var success = await _service.DeleteAsync(id, ct);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("{id}/similar")]
        public async Task<IActionResult> Similar(int id)
        {
            return Ok(await _service.GetSimilarProductsAsync(id));
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> Recommended(CancellationToken ct)
        {
            var products = await _service.GetRecommendedProductsAsync(8, ct);
            return Ok(products);
        }

        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImages([FromForm] IFormFileCollection files, CancellationToken ct = default)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded");

            var uploadDtos = new List<ProductImageUploadDto>();
            foreach (var f in files)
            {
                using var ms = new MemoryStream();
                await f.CopyToAsync(ms, ct);
                uploadDtos.Add(new ProductImageUploadDto { FileName = f.FileName, Content = ms.ToArray() });
            }

            var urls = await _service.UploadImagesAsync(uploadDtos, ct);
            return Ok(new { imageUrls = urls });
        }
    }
}
