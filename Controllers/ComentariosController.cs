using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ComentariosController(AppDbContext context) => _context = context;

    [HttpGet("publicacion/{publicacionId}")]
    public async Task<ActionResult<IEnumerable<Comentarios>>> GetComentariosByPublicacion(int publicacionId) => 
        await _context.Comentario.Where(c => c.PublicacionId == publicacionId).Include(c => c.usuarios).ToListAsync();

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Comentarios>> PostComentario(Comentarios comentario)
    {
        comentario.Fecha = DateTime.Now;
        _context.Comentario.Add(comentario);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetComentariosByPublicacion), new { publicacionId = comentario.PublicacionId }, comentario);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")] // O el dueño del comentario (requeriría lógica extra)
    public async Task<IActionResult> DeleteComentario(int id)
    {
        var comentario = await _context.Comentario.FindAsync(id);
        if (comentario == null) return NotFound();
        _context.Comentario.Remove(comentario);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}