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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuração para evitar que um usuário se inscreva múltiplas vezes no mesmo evento
        builder.Entity<Inscricao>()
            .HasIndex(i => new { i.EventoId, i.ApplicationUserId })
            .IsUnique();

        // Relacionamento Evento <-> Categoria (Um-para-Muitos)
        builder.Entity<Evento>()
            .HasOne(e => e.Categoria)
            .WithMany(c => c.Eventos)
            .HasForeignKey(e => e.CategoriaId);

        // Relacionamento Evento <-> Inscricao (Um-para-Muitos)
        builder.Entity<Inscricao>()
            .HasOne(i => i.Evento)
            .WithMany(e => e.Inscricoes)
            .HasForeignKey(i => i.EventoId);

        // Relacionamento AppUser <-> Inscricao (Um-para-Muitos)
        builder.Entity<Inscricao>()
            .HasOne(i => i.AppUser)
            .WithMany(u => u.Inscricoes)
            .HasForeignKey(i => i.ApplicationUserId);
    }
}
