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

    // GET: api/skydiver
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Skydiver>>> GetAll()
    {
        var skydivers = await _service.GetAllAsync();
        return Ok(skydivers);
    }

    // GET: api/skydiver/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Skydiver>> GetById(int id)
    {
        var skydiver = await _service.GetByIdAsync(id);
        if (skydiver is null)
            return NotFound();

        return Ok(skydiver);
    }

    // POST: api/skydiver
    [HttpPost]
    public async Task<ActionResult<Skydiver>> Create([FromBody] Skydiver newSkydiver)
    {
        var created = await _service.CreateAsync(newSkydiver);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE: api/skydiver/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
