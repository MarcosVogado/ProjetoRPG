using Microsoft.EntityFrameworkCore;
using ProjetoRPG.Data;
using ProjetoRPG.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Servicos
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RpgContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Pipeline
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RpgContext>();
    db.Database.Migrate();
    SeedDatabase(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void SeedDatabase(RpgContext db)
{
    // evita semear várias vezes
    if (db.Personagens.Any() || db.Missoes.Any())
    {
        return;
    }

    var personagens = new[]
    {
        new Personagem { Nome = "Arthas", Classe = ClassePersonagem.Guerreiro, Nivel = 8, Vida = 140, Mana = 30, Moral = 85, PatenteGuilda = "Capitao" },
        new Personagem { Nome = "Jaina", Classe = ClassePersonagem.Mago, Nivel = 9, Vida = 90, Mana = 180, Moral = 92, PatenteGuilda = "Arquimaga" },
        new Personagem { Nome = "Valeera", Classe = ClassePersonagem.Ladino, Nivel = 7, Vida = 110, Mana = 40, Moral = 75, PatenteGuilda = "Batedor" }
    };

    var missoes = new[]
    {
        new Missao { Titulo = "Escoltar caravana", Descricao = "Proteja mercadores ate a cidade vizinha", Dificuldade = 3, RecompensaOuro = 150, Status = StatusMissao.Planejada },
        new Missao { Titulo = "Caçar dragao jovem", Descricao = "Eliminar a ameaca nos picos do norte", Dificuldade = 7, RecompensaOuro = 600, Status = StatusMissao.EmAndamento },
        new Missao { Titulo = "Recuperar reliquia", Descricao = "Trazer de volta a relíquia roubada", Dificuldade = 5, RecompensaOuro = 320, Status = StatusMissao.Planejada }
    };

    db.Personagens.AddRange(personagens);
    db.Missoes.AddRange(missoes);
    db.SaveChanges();

    // atribuir personagens às missoes
    var atribuicoes = new[]
    {
        new PersonagemMissao { PersonagemId = personagens[0].Id, MissaoId = missoes[0].Id, FuncaoNoGrupo = "Tank" },
        new PersonagemMissao { PersonagemId = personagens[1].Id, MissaoId = missoes[0].Id, FuncaoNoGrupo = "Mago" },
        new PersonagemMissao { PersonagemId = personagens[2].Id, MissaoId = missoes[1].Id, FuncaoNoGrupo = "Batedor" },
    };

    db.PersonagensMissoes.AddRange(atribuicoes);
    db.SaveChanges();
}
