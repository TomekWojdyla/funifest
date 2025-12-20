using Microsoft.AspNetCore.Mvc;
using Funifest.Domain.Models;
using Funifest.Application.Services.Interfaces;

namespace Funifest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParachuteController : ControllerBase
{
    private readonly IParachuteService _service;

    public ParachuteController(IParachuteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Parachute>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Parachute>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Parachute>> Create([FromBody] Parachute parachute)
    {
        var created = await _service.CreateAsync(parachute);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
