using Microsoft.AspNetCore.Mvc;
using Funifest.Application.DTO.ExitPlan;
using Funifest.Application.Services.Interfaces;
using Funifest.Domain.Models;

namespace Funifest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExitPlanController : ControllerBase
{
    private readonly IExitPlanService _service;

    public ExitPlanController(IExitPlanService service)
    {
        _service = service;
    }

    // GET: api/exitplan
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExitPlan>>> GetAll()
    {
        var plans = await _service.GetAllAsync();
        return Ok(plans);
    }

    // GET: api/exitplan/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExitPlan>> GetById(int id)
    {
        var plan = await _service.GetByIdAsync(id);
        if (plan is null)
            return NotFound();

        return Ok(plan);
    }

    // POST: api/exitplan
    [HttpPost]
    public async Task<ActionResult<ExitPlan>> Create([FromBody] CreateExitPlanRequest request)
    {
        try
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: api/exitplan/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
