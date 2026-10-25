using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoReporteController : ControllerBase
{
    private readonly AppDbContext _context;

    public TipoReporteController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoReporte>>> GetTipos() => 
        await _context.TipoReportes.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoReporte>> GetTipo(int id)
    {
        var tipo = await _context.TipoReportes
            .Include(t => t.Reportes)
            .FirstOrDefaultAsync(t => t.Id == id);
        return tipo == null ? NotFound() : tipo;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<TipoReporte>> PostTipo(TipoReporte tipo)
    {
        _context.TipoReportes.Add(tipo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTipo), new { id = tipo.Id }, tipo);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> PutTipo(int id, TipoReporte tipo)
    {
        if (id != tipo.Id) return BadRequest();
        _context.Entry(tipo).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteTipo(int id)
    {
        var tipo = await _context.TipoReportes.FindAsync(id);
        if (tipo == null) return NotFound();
        _context.TipoReportes.Remove(tipo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}