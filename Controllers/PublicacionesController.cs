using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicacionesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PublicacionesController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Publicaciones>>> GetPublicaciones() => 
        await _context.publicacion.Include(p => p.Mascotas).ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO,Albergue,Ciudadano")]
    public async Task<ActionResult<Publicaciones>> PostPublicacion(Publicaciones publicacion)
    {
        publicacion.fecha = DateTime.Now;
        _context.publicacion.Add(publicacion);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPublicaciones), new { id = publicacion.Id }, publicacion);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePublicacion(int id)
    {
        var publicacion = await _context.publicacion.FindAsync(id);
        if (publicacion == null) return NotFound();
        _context.publicacion.Remove(publicacion);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}