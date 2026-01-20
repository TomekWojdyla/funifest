using Microsoft.AspNetCore.Mvc;
using Funifest.Domain.Models;
using Funifest.Application.Services.Interfaces;

namespace Funifest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkydiverController : ControllerBase
{
    private readonly ISkydiverService _service;

    public SkydiverController(ISkydiverService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Skydiver>>> GetAll()
    {
        var skydivers = await _service.GetAllAsync();
        return Ok(skydivers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Skydiver>> GetById(int id)
    {
        var skydiver = await _service.GetByIdAsync(id);
        if (skydiver is null)
            return NotFound();

        return Ok(skydiver);
    }

    [HttpPost]
    public async Task<ActionResult<Skydiver>> Create([FromBody] Skydiver newSkydiver)
    {
        var created = await _service.CreateAsync(newSkydiver);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/block")]
    public async Task<ActionResult<Skydiver>> Block(int id)
    {
        var updated = await _service.BlockAsync(id);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    [HttpPut("{id:int}/unblock")]
    public async Task<ActionResult<Skydiver>> Unblock(int id)
    {
        var updated = await _service.UnblockAsync(id);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
