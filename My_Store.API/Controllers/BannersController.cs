using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Store.Application.Interfaces;

namespace My_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public BannersController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var banners = await _uow.Banners.GetAllAsync(ct);

            var response = banners.Select(b => new
            {
                b.Id,
                b.ImageUrl
            });

            return Ok(response);
        }
    }
}
