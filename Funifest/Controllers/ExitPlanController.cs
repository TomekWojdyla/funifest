using Microsoft.AspNetCore.Mvc;
using Funifest.Application.DTO.ExitPlan;
using Funifest.Application.Services.Interfaces;
using Funifest.Application.Exceptions;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExitPlan>>> GetAll()
    {
        var plans = await _service.GetAllAsync();
        return Ok(plans);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExitPlan>> GetById(int id)
    {
        var plan = await _service.GetByIdAsync(id);
        if (plan is null)
            return NotFound();

        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<ExitPlan>> Create([FromBody] CreateExitPlanRequest request)
    {
        try
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ExitPlan>> Update(int id, [FromBody] UpdateExitPlanRequest request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated is null)
                return NotFound();

            return Ok(updated);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:int}/dispatch")]
    public async Task<ActionResult<ExitPlan>> Dispatch(int id)
    {
        try
        {
            var updated = await _service.DispatchAsync(id);
            if (updated is null)
                return NotFound();

            return Ok(updated);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:int}/undo-dispatch")]
    public async Task<IActionResult> UndoDispatch(int id)
    {
        try
        {
            var ok = await _service.UndoDispatchAsync(id);
            return ok ? NoContent() : NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
