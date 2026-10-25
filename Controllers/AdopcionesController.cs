using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdopcionesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdopcionesController(AppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin,CEMZOO,Albergue")]
    public async Task<ActionResult<IEnumerable<Adopciones>>> GetAdopciones() => 
        await _context.Adopcion.Include(a => a.solicitudesAdopcion).ThenInclude(s => s.Mascotas).ToListAsync();

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Adopciones>> GetAdopcion(int id)
    {
        var adopcion = await _context.Adopcion.Include(a => a.solicitudesAdopcion).FirstOrDefaultAsync(a => a.Id == id);
        return adopcion == null ? NotFound() : adopcion;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO,Albergue")]
    public async Task<ActionResult<Adopciones>> PostAdopcion(Adopciones adopcion)
    {
        adopcion.fecha = DateTime.Now;
        _context.Adopcion.Add(adopcion);
        
        // Actualizar estado de la mascota a "Adoptado"
        var solicitud = await _context.SolicitudAdopcion.Include(s => s.Mascotas).FirstOrDefaultAsync(s => s.Id == adopcion.SolicitudId);
        if (solicitud?.Mascotas != null)
        {
            solicitud.Mascotas.Estado = "Adoptado";
        }
        
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAdopcion), new { id = adopcion.Id }, adopcion);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteAdopcion(int id)
    {
        var adopcion = await _context.Adopcion.FindAsync(id);
        if (adopcion == null) return NotFound();
        
        _context.Adopcion.Remove(adopcion);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}