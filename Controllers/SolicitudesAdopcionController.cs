using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesAdopcionController : ControllerBase
{
    private readonly AppDbContext _context;

    public SolicitudesAdopcionController(AppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin,CEMZOO,Albergue")]
    public async Task<ActionResult<IEnumerable<SolicitudesAdopcion>>> GetSolicitudes() => 
        await _context.SolicitudAdopcion
            .Include(s => s.Mascotas)
            .Include(s => s.Ciudadanos)
            .ToListAsync();

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<SolicitudesAdopcion>> GetSolicitud(int id)
    {
        var solicitud = await _context.SolicitudAdopcion
            .Include(s => s.Mascotas)
            .Include(s => s.Ciudadanos)
            .FirstOrDefaultAsync(s => s.Id == id);
        return solicitud == null ? NotFound() : solicitud;
    }

    [HttpGet("ciudadano/{ciudadanoId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SolicitudesAdopcion>>> GetSolicitudesByCiudadano(int ciudadanoId) => 
        await _context.SolicitudAdopcion
            .Include(s => s.Mascotas)
            .Where(s => s.CiudadanoId == ciudadanoId)
            .ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Ciudadano")]
    public async Task<ActionResult<SolicitudesAdopcion>> PostSolicitud(SolicitudesAdopcion solicitud)
    {
        solicitud.fecha = DateTime.Now;
        solicitud.Estado = "Pendiente";
        _context.SolicitudAdopcion.Add(solicitud);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.Id }, solicitud);
    }

    [HttpPut("{id}/estado")]
    [Authorize(Roles = "Admin,CEMZOO,Albergue")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] string nuevoEstado)
    {
        var solicitud = await _context.SolicitudAdopcion.FindAsync(id);
        if (solicitud == null) return NotFound();
        
        solicitud.Estado = nuevoEstado; // "Aprobada", "Rechazada", "En revisión"
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteSolicitud(int id)
    {
        var solicitud = await _context.SolicitudAdopcion.FindAsync(id);
        if (solicitud == null) return NotFound();
        _context.SolicitudAdopcion.Remove(solicitud);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}