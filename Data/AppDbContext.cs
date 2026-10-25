using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Proyecto.Models;

namespace Proyecto.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets de todos los modelos
    public DbSet<Adopciones> Adopcion { get; set; }
    public DbSet<Ciudadanos> Ciudadadano { get; set; }
    public DbSet<Comentarios> Comentario { get; set; }
    public DbSet<DenunciaAbandonos> denunciaAbandono { get; set; }
    public DbSet<Distritos> Distrito { get; set; }
    public DbSet<Establecimientos> Establecimiento { get; set; }
    public DbSet<Mascotas> Mascota { get; set; }
    public DbSet<Publicaciones> publicacion { get; set; }
    public DbSet<Reportes> Reporte { get; set; }
    public DbSet<Rescatistas> Rescatista { get; set; }
    public DbSet<Roles> Rol { get; set; }
    public DbSet<SolicitudesAdopcion> SolicitudAdopcion { get; set; }
    public DbSet<TipoEstableciminetos> TipoEstablecimineto { get; set; }
    public DbSet<TipoReporte> TipoReportes { get; set; }
    public DbSet<Tramites> Tramites { get; set; }
    
    // ESTA LÍNEA FALTABA:
    public DbSet<Usuarios> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ciudadanos>()
            .HasOne(c => c.usuarios)
            .WithOne(u => u.ciudadanos)
            .HasForeignKey<Ciudadanos>(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mascotas>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.especie).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.HasOne(e => e.Distritos).WithMany(d => d.Mascotas).HasForeignKey(e => e.DistritoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reportes>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.estado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Ubicaion).IsRequired();
            entity.HasOne(e => e.TipoReporte).WithMany(t => t.Reportes).HasForeignKey(e => e.tipoReportesId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Adopciones>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.numeroSerie).IsRequired().HasMaxLength(50);
            entity.Property(e => e.fecha).IsRequired();
            entity.HasOne(e => e.solicitudesAdopcion).WithOne(s => s.Adopciones).HasForeignKey<Adopciones>(e => e.SolicitudId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}