using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TramitesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TramitesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tramites>>> GetTramites() => 
        await _context.Tramites.Include(t => t.Establecimientos).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Tramites>> GetTramite(int id)
    {
        var tramite = await _context.Tramites
            .Include(t => t.Establecimientos)
            .FirstOrDefaultAsync(t => t.Id == id);
        return tramite == null ? NotFound() : tramite;
    }

    [HttpGet("establecimiento/{establecimientoId}")]
    public async Task<ActionResult<IEnumerable<Tramites>>> GetTramitesByEstablecimiento(int establecimientoId) => 
        await _context.Tramites
            .Include(t => t.Establecimientos)
            .Where(t => t.EstableciminetosId == establecimientoId)
            .ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO,Veterinaria,Albergue")]
    public async Task<ActionResult<Tramites>> PostTramite(Tramites tramite)
    {
        _context.Tramites.Add(tramite);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTramite), new { id = tramite.Id }, tramite);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CEMZOO,Veterinaria,Albergue")]
    public async Task<IActionResult> PutTramite(int id, Tramites tramite)
    {
        if (id != tramite.Id) return BadRequest();
        _context.Entry(tramite).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteTramite(int id)
    {
        var tramite = await _context.Tramites.FindAsync(id);
        if (tramite == null) return NotFound();
        _context.Tramites.Remove(tramite);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}