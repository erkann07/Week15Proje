using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneStore.Application.Abstractions;
using TuneStore.Application.DTOs;
using TuneStore.Domain.Entities;

namespace TuneStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _svc;
        public OrdersController(IOrderService svc) => _svc = svc;

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Customer) + "," + nameof(UserRole.Admin))]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            var order = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _svc.GetAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        [HttpGet("by-customer/{customerId:int}")]
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Customer))]
        public async Task<IActionResult> GetByCustomer(int customerId) => Ok(await _svc.GetByCustomerAsync(customerId));
    }
}
