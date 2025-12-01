using System.ComponentModel.DataAnnotations;

namespace ProjetoRPG.Models;

public class PersonagemMissao
{
    public int PersonagemId { get; set; }
    public Personagem Personagem { get; set; } = null!;

    public int MissaoId { get; set; }
    public Missao Missao { get; set; } = null!;

    [Required, StringLength(40, MinimumLength = 3)]
    public string FuncaoNoGrupo { get; set; } = string.Empty;

    public DateTime DataAtribuicao { get; set; } = DateTime.UtcNow;
}
