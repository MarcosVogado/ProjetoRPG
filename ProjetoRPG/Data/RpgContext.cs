using Microsoft.EntityFrameworkCore;
using ProjetoRPG.Models;

namespace ProjetoRPG.Data;

public class RpgContext : DbContext
{
    public RpgContext(DbContextOptions<RpgContext> options) : base(options)
    {
    }

    public DbSet<Personagem> Personagens => Set<Personagem>();
    public DbSet<Missao> Missoes => Set<Missao>();
    public DbSet<PersonagemMissao> PersonagensMissoes => Set<PersonagemMissao>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<PersonagemMissao>()
            .HasKey(pm => new { pm.PersonagemId, pm.MissaoId });

        builder.Entity<PersonagemMissao>()
            .HasOne(pm => pm.Personagem)
            .WithMany(p => p.Missoes)
            .HasForeignKey(pm => pm.PersonagemId);

        builder.Entity<PersonagemMissao>()
            .HasOne(pm => pm.Missao)
            .WithMany(m => m.Participantes)
            .HasForeignKey(pm => pm.MissaoId);

        builder.Entity<PersonagemMissao>()
            .Property(pm => pm.DataAtribuicao)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
