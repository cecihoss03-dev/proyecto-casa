using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Proyecto.Data;
using Proyecto.Models;
using Nj.EntityFrameworkCore.LibSql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TursoConnection");
    options.UseLibSql(connectionString);
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "casa.sucre",
        ValidAudience = "casa.sucre",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EstaEsUnaClaveSecretaMuyLargaParaCASA2026!"))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "C.A.S.A. API", 
        Version = "v1",
        Description = "Sistema de Centro de Alerta, Salvamento y Adopción - Municipio de Sucre"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://tudominio.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "C.A.S.A. API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedDataAsync(serviceProvider);
}

app.Run();

async Task SeedDataAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var context = serviceProvider.GetRequiredService<AppDbContext>();

    // 1. Crear roles
    string[] roleNames = { "Admin", "CEMZOO", "Veterinaria", "Albergue", "Ciudadano", "Rescatista" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // 2. Crear usuarios de prueba con diferentes roles
    var usuariosPrueba = new[]
    {
        (email: "admin@casa.sucre", password: "Admin123!", role: "Admin", nombre: "Administrador Sistema"),
        (email: "cemzoo@casa.sucre", password: "Cemzoo123!", role: "CEMZOO", nombre: "Personal CEMZOO"),
        (email: "albergue@casa.sucre", password: "Albergue123!", role: "Albergue", nombre: "Albergue Patitas"),
        (email: "veterinaria@casa.sucre", password: "Vet123!", role: "Veterinaria", nombre: "Dr. Juan Pérez"),
        (email: "ciudadano@casa.sucre", password: "Ciudadano123!", role: "Ciudadano", nombre: "María López"),
        (email: "rescatista@casa.sucre", password: "Rescatista123!", role: "Rescatista", nombre: "Carlos Rescate")
    };

    foreach (var (email, password, role, nombre) in usuariosPrueba)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                NombreCompleto = nombre,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                Console.WriteLine($"✅ Usuario {role} creado: {email} / {password}");
            }
        }
    }

    // 3. Crear distritos (si no existen)
    if (!context.Distrito.Any())
    {
        var distritos = new List<Distritos>
        {
            new Distritos { Id = 1, nombre = "Distrito 1", ColorCinta = "Rojo", Poligono = "POLYGON((-65.26 -19.04, -65.25 -19.04, -65.25 -19.03, -65.26 -19.03, -65.26 -19.04))" },
            new Distritos { Id = 2, nombre = "Distrito 2", ColorCinta = "Azul", Poligono = "POLYGON((-65.27 -19.05, -65.26 -19.05, -65.26 -19.04, -65.27 -19.04, -65.27 -19.05))" },
            new Distritos { Id = 3, nombre = "Distrito 3", ColorCinta = "Verde", Poligono = "POLYGON((-65.28 -19.06, -65.27 -19.06, -65.27 -19.05, -65.28 -19.05, -65.28 -19.06))" },
            new Distritos { Id = 4, nombre = "Distrito 4", ColorCinta = "Amarillo", Poligono = "POLYGON((-65.29 -19.07, -65.28 -19.07, -65.28 -19.06, -65.29 -19.06, -65.29 -19.07))" },
            new Distritos { Id = 5, nombre = "Distrito 5", ColorCinta = "Naranja", Poligono = "POLYGON((-65.30 -19.08, -65.29 -19.08, -65.29 -19.07, -65.30 -19.07, -65.30 -19.08))" }
        };
        context.Distrito.AddRange(distritos);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Distritos creados");
    }

    // 4. Crear tipos de establecimientos
    if (!context.TipoEstablecimineto.Any())
    {
        var tipos = new List<TipoEstableciminetos>
        {
            new TipoEstableciminetos { Id = 1, nombre = "Veterinaria" },
            new TipoEstableciminetos { Id = 2, nombre = "Albergue" },
            new TipoEstableciminetos { Id = 3, nombre = "Peluquería" },
            new TipoEstableciminetos { Id = 4, nombre = "Tienda de Mascotas" }
        };
        context.TipoEstablecimineto.AddRange(tipos);
        await context.SaveChangesAsync();
    }

    // 5. Crear tipos de reportes
    if (!context.TipoReportes.Any())
    {
        var tipos = new List<TipoReporte>
        {
            new TipoReporte { Id = 1, nombre = "Animal Abandonado" },
            new TipoReporte { Id = 2, nombre = "Animal Perdido" },
            new TipoReporte { Id = 3, nombre = "Animal Encontrado" },
            new TipoReporte { Id = 4, nombre = "Maltrato Animal" },
            new TipoReporte { Id = 5, nombre = "Mordedura" }
        };
        context.TipoReportes.AddRange(tipos);
        await context.SaveChangesAsync();
    }

    // 6. CREAR MASCOTAS DE PRUEBA (esto es lo que tu compañero necesita)
    if (!context.Mascota.Any())
    {
        var mascotas = new List<Mascotas>
        {
            new Mascotas { 
                nombre = "Firulais", 
                especie = "Perro", 
                edad = "2 años", 
                tamaño = "Mediano", 
                color = "Café", 
                caracteristica = "Muy juguetón y cariñoso", 
                foto = "", 
                Estado = "Disponible", 
                DistritoId = 1 
            },
            new Mascotas { 
                nombre = "Michi", 
                especie = "Gato", 
                edad = "1 año", 
                tamaño = "Pequeño", 
                color = "Negro", 
                caracteristica = "Tranquilo, ideal para departamentos", 
                foto = "", 
                Estado = "Disponible", 
                DistritoId = 2 
            },
            new Mascotas { 
                nombre = "Rocky", 
                especie = "Perro", 
                edad = "3 años", 
                tamaño = "Grande", 
                color = "Dorado", 
                caracteristica = "Protector, necesita espacio", 
                foto = "", 
                Estado = "En proceso", 
                DistritoId = 3 
            },
            new Mascotas { 
                nombre = "Luna", 
                especie = "Gato", 
                edad = "6 meses", 
                tamaño = "Pequeño", 
                color = "Blanco", 
                caracteristica = "Muy activa y curiosa", 
                foto = "", 
                Estado = "Disponible", 
                DistritoId = 1 
            },
            new Mascotas { 
                nombre = "Max", 
                especie = "Perro", 
                edad = "5 años", 
                tamaño = "Mediano", 
                color = "Negro y blanco", 
                caracteristica = "Entrenado, obediente", 
                foto = "", 
                Estado = "Adoptado", 
                DistritoId = 4 
            }
        };
        context.Mascota.AddRange(mascotas);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ 5 mascotas de prueba creadas");
    }

    if (!context.Establecimiento.Any())
    {
        var establecimientos = new List<Establecimientos>
        {
            new Establecimientos { 
                nombre = "Veterinaria San Francisco", 
                direccion = "Av. Independencia #123", 
                telefono = "4-661234", 
                ubicacion = "-19.0414, -65.2589", 
                horario = "Lun-Vie 8:00-18:00", 
                capacidad = 50, 
                calificacion = 4.5m, 
                estado = "Activo", 
                tipoEstableciminetoId = 1, 
                DistritoId = 1 
            },
            new Establecimientos { 
                nombre = "Albergue Patitas Felices", 
                direccion = "Calle Junín #456", 
                telefono = "4-665678", 
                ubicacion = "-19.0450, -65.2600", 
                horario = "Lun-Dom 9:00-17:00", 
                capacidad = 100, 
                calificacion = 4.8m, 
                estado = "Activo", 
                tipoEstableciminetoId = 2, 
                DistritoId = 2 
            }
        };
        context.Establecimiento.AddRange(establecimientos);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ 2 establecimientos de prueba creados");
    }

    Console.WriteLine("✅ Seed data completado");
}