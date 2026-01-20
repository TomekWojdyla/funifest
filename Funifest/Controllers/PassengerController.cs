using Microsoft.AspNetCore.Mvc;
using Funifest.Domain.Models;
using Funifest.Application.Services.Interfaces;

namespace Funifest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PassengerController : ControllerBase
{
    private readonly IPassengerService _service;

    public PassengerController(IPassengerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Passenger>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Passenger>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Passenger>> Create([FromBody] Passenger passenger)
    {
        var created = await _service.CreateAsync(passenger);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/block")]
    public async Task<ActionResult<Passenger>> Block(int id)
    {
        var updated = await _service.BlockAsync(id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:int}/unblock")]
    public async Task<ActionResult<Passenger>> Unblock(int id)
    {
        var updated = await _service.UnblockAsync(id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
