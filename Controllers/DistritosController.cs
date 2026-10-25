using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistritosController : ControllerBase
{
    private readonly AppDbContext _context;

    public DistritosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Distritos>>> GetDistritos() => await _context.Distrito.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Distritos>> GetDistrito(int id)
    {
        var distrito = await _context.Distrito.Include(d => d.Mascotas).FirstOrDefaultAsync(d => d.Id == id);
        return distrito == null ? NotFound() : distrito;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<ActionResult<Distritos>> PostDistrito(Distritos distrito)
    {
        _context.Distrito.Add(distrito);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDistrito), new { id = distrito.Id }, distrito);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,CEMZOO")]
    public async Task<IActionResult> PutDistrito(int id, Distritos distrito)
    {
        if (id != distrito.Id) return BadRequest();
        _context.Entry(distrito).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}