using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstablecimientosController : ControllerBase
{
    private readonly AppDbContext _context;

    public EstablecimientosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Establecimientos>>> GetEstablecimientos() => 
        await _context.Establecimiento.Include(e => e.TipoEstableciminetos).Include(e => e.Distritos).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Establecimientos>> GetEstablecimiento(int id)
    {
        var establecimiento = await _context.Establecimiento.Include(e => e.TipoEstableciminetos).FirstOrDefaultAsync(e => e.Id == id);
        return establecimiento == null ? NotFound() : establecimiento;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO,Veterinaria,Albergue")]
    public async Task<ActionResult<Establecimientos>> PostEstablecimiento(Establecimientos establecimiento)
    {
        _context.Establecimiento.Add(establecimiento);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEstablecimiento), new { id = establecimiento.Id }, establecimiento);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CEMZOO,Veterinaria,Albergue")]
    public async Task<IActionResult> PutEstablecimiento(int id, Establecimientos establecimiento)
    {
        if (id != establecimiento.Id) return BadRequest();
        _context.Entry(establecimiento).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!EstablecimientoExists(id)) return NotFound(); else throw; }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteEstablecimiento(int id)
    {
        var establecimiento = await _context.Establecimiento.FindAsync(id);
        if (establecimiento == null) return NotFound();
        _context.Establecimiento.Remove(establecimiento);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool EstablecimientoExists(int id) => _context.Establecimiento.Any(e => e.Id == id);
}