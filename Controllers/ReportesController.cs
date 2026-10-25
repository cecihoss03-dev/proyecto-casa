using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportesController(AppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin,CEMZOO,Rescatista")]
    public async Task<ActionResult<IEnumerable<Reportes>>> GetReportes() => 
        await _context.Reporte.Include(r => r.TipoReporte).Include(r => r.Ciudadanos).ToListAsync();

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Reportes>> GetReporte(int id)
    {
        var reporte = await _context.Reporte.Include(r => r.TipoReporte).FirstOrDefaultAsync(r => r.Id == id);
        return reporte == null ? NotFound() : reporte;
    }

    [HttpPost]
    [Authorize(Roles = "Ciudadano,Rescatista,CEMZOO")]
    public async Task<ActionResult<Reportes>> PostReporte(Reportes reporte)
    {
        reporte.Fecha = DateTime.Now;
        reporte.estado = "Pendiente";
        _context.Reporte.Add(reporte);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReporte), new { id = reporte.Id }, reporte);
    }

    [HttpPut("{id}/estado")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] string nuevoEstado)
    {
        var reporte = await _context.Reporte.FindAsync(id);
        if (reporte == null) return NotFound();
        
        reporte.estado = nuevoEstado;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteReporte(int id)
    {
        var reporte = await _context.Reporte.FindAsync(id);
        if (reporte == null) return NotFound();
        
        _context.Reporte.Remove(reporte);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}