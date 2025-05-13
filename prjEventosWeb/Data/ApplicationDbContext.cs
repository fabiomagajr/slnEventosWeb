using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using prjEventosWeb.Models;

namespace prjEventosWeb.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<prjEventosWeb.Models.Categoria> Categoria { get; set; } = default!;

public DbSet<prjEventosWeb.Models.Evento> Evento { get; set; } = default!;

public DbSet<prjEventosWeb.Models.Inscricao> Inscricao { get; set; } = default!;
}
