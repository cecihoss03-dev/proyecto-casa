using Microsoft.AspNetCore.Identity;

namespace Proyecto.Models;

public class ApplicationUser : IdentityUser
{
    // Propiedades adicionales según el documento
    public string? NombreCompleto { get; set; }
    public string? Ci { get; set; }
    public string? Celular { get; set; }
    public string? FotoPerfil { get; set; }
    public int? DistritoId { get; set; }
    public Distritos? Distrito { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}