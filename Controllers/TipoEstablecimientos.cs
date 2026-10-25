using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoEstablecimientosController : ControllerBase
{
    private readonly AppDbContext _context;

    public TipoEstablecimientosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoEstableciminetos>>> GetTipos() => 
        await _context.TipoEstablecimineto.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoEstableciminetos>> GetTipo(int id)
    {
        var tipo = await _context.TipoEstablecimineto
            .Include(t => t.Establecimientos)
            .FirstOrDefaultAsync(t => t.Id == id);
        return tipo == null ? NotFound() : tipo;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<TipoEstableciminetos>> PostTipo(TipoEstableciminetos tipo)
    {
        _context.TipoEstablecimineto.Add(tipo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTipo), new { id = tipo.Id }, tipo);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> PutTipo(int id, TipoEstableciminetos tipo)
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
        var tipo = await _context.TipoEstablecimineto.FindAsync(id);
        if (tipo == null) return NotFound();
        _context.TipoEstablecimineto.Remove(tipo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}