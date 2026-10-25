using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CiudadanosController : ControllerBase
{
    private readonly AppDbContext _context;

    public CiudadanosController(AppDbContext context) => _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<IEnumerable<Ciudadanos>>> GetCiudadanos() => 
        await _context.Ciudadadano.Include(c => c.usuarios).ToListAsync();

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Ciudadanos>> GetCiudadano(int id)
    {
        var ciudadano = await _context.Ciudadadano.Include(c => c.usuarios).FirstOrDefaultAsync(c => c.Id == id);
        return ciudadano == null ? NotFound() : ciudadano;
    }

    [HttpPost]
    [Authorize(Roles = "Ciudadano")]
    public async Task<ActionResult<Ciudadanos>> PostCiudadano(Ciudadanos ciudadano)
    {
        _context.Ciudadadano.Add(ciudadano);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCiudadano), new { id = ciudadano.Id }, ciudadano);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutCiudadano(int id, Ciudadanos ciudadano)
    {
        if (id != ciudadano.Id) return BadRequest();
        _context.Entry(ciudadano).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}