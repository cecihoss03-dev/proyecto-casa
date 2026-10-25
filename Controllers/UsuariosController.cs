using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios() => 
        await _context.Usuario
            .Include(u => u.Roles)
            .Include(u => u.Distritos)
            .ToListAsync();

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Usuarios>> GetUsuario(int id)
    {
        var usuario = await _context.Usuario
            .Include(u => u.Roles)
            .Include(u => u.Distritos)
            .FirstOrDefaultAsync(u => u.Id == id);
        return usuario == null ? NotFound() : usuario;
    }

    [HttpGet("distrito/{distritoId}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuariosByDistrito(int distritoId) => 
        await _context.Usuario
            .Include(u => u.Roles)
            .Where(u => u.DistritoId == distritoId)
            .ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<Usuarios>> PostUsuario(Usuarios usuario)
    {
        usuario.Created_at = DateOnly.FromDateTime(DateTime.Now);
        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutUsuario(int id, Usuarios usuario)
    {
        if (id != usuario.Id) return BadRequest();
        _context.Entry(usuario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        var usuario = await _context.Usuario.FindAsync(id);
        if (usuario == null) return NotFound();
        _context.Usuario.Remove(usuario);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}