using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RescatistasController : ControllerBase
{
    private readonly AppDbContext _context;

    public RescatistasController(AppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<IEnumerable<Rescatistas>>> GetRescatistas() => 
        await _context.Rescatista.Include(r => r.usuarios).ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<Rescatistas>> PostRescatista(Rescatistas rescatista)
    {
        _context.Rescatista.Add(rescatista);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRescatistas), new { id = rescatista.Id }, rescatista);
    }

    [HttpPut("{id}/disponibilidad")]
    [Authorize(Roles = "Rescatista,Admin,CEMZOO")]
    public async Task<IActionResult> UpdateDisponibilidad(int id, [FromBody] string estado)
    {
        var rescatista = await _context.Rescatista.FindAsync(id);
        if (rescatista == null) return NotFound();
        
        rescatista.disponible = estado; // Ej: "Disponible", "En misión", "No disponible"
        await _context.SaveChangesAsync();
        return NoContent();
    }
}