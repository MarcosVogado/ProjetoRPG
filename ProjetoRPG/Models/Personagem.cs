using System.ComponentModel.DataAnnotations;

namespace ProjetoRPG.Models;

public enum ClassePersonagem
{
    Guerreiro = 1,
    Mago,
    Arqueiro,
    Ladino,
    Clerigo,
    Bardo,
    Paladino
}

public class Personagem
{
    public int Id { get; set; }

    [Required, StringLength(80, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public ClassePersonagem Classe { get; set; }

    [Range(1, 100)]
    public int Nivel { get; set; } = 1;

    [Range(0, 500)]
    public int Vida { get; set; }

    [Range(0, 500)]
    public int Mana { get; set; }

    [Range(0, 100)]
    public int Moral { get; set; } = 50;

    [StringLength(40)]
    public string? PatenteGuilda { get; set; }

    public ICollection<PersonagemMissao> Missoes { get; set; } = new List<PersonagemMissao>();
}
