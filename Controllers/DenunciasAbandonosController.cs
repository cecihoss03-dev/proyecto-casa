using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DenunciasAbandonosController : ControllerBase
{
    private readonly AppDbContext _context;

    public DenunciasAbandonosController(AppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<IEnumerable<DenunciaAbandonos>>> GetDenuncias() => 
        await _context.denunciaAbandono.Include(d => d.Reportes).ToListAsync();

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<DenunciaAbandonos>> GetDenuncia(int id)
    {
        var denuncia = await _context.denunciaAbandono.Include(d => d.Reportes).FirstOrDefaultAsync(d => d.Id == id);
        return denuncia == null ? NotFound() : denuncia;
    }

    [HttpPost]
    [Authorize(Roles = "Ciudadano,Rescatista,CEMZOO")]
    public async Task<ActionResult<DenunciaAbandonos>> PostDenuncia(DenunciaAbandonos denuncia)
    {
        _context.denunciaAbandono.Add(denuncia);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDenuncia), new { id = denuncia.Id }, denuncia);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> PutDenuncia(int id, DenunciaAbandonos denuncia)
    {
        if (id != denuncia.Id) return BadRequest();
        _context.Entry(denuncia).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> DeleteDenuncia(int id)
    {
        var denuncia = await _context.denunciaAbandono.FindAsync(id);
        if (denuncia == null) return NotFound();
        _context.denunciaAbandono.Remove(denuncia);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}