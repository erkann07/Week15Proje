using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneStore.Api.Filters;
using TuneStore.Application.Abstractions;
using TuneStore.Application.DTOs;
using TuneStore.Domain.Entities;

namespace TuneStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumService _svc;
        public AlbumsController(IAlbumService svc) => _svc = svc;

        public (object items, object total) GetValue()
        {
            return var (items, total);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get((object items, object total) value, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        {
            value = await _svc.GetPagedAsync(page, pageSize, search, _svc.Get_ctx());
            return Ok(new { total, page, pageSize, items });
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _svc.GetAsync(id);
            return a is null ? NotFound() : Ok(a);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [TimeWindowFilter(0, 24)]
        public async Task<IActionResult> Create([FromBody] AlbumCreateUpdateDto dto) => Ok(await _svc.CreateAsync(dto));

        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Update(int id, [FromBody] AlbumCreateUpdateDto dto) => Ok(await _svc.UpdateAsync(id, dto));

        [HttpPatch("{id:int}/stock")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> PatchStock(int id, [FromQuery] int delta) => Ok(await _svc.PatchStockAsync(id, delta));

        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
