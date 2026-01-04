using Funifest.Application.DTO;
using Funifest.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<IEnumerable<ParachuteDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ParachuteDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ParachuteDto>> Create([FromBody] CreateParachuteDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}

