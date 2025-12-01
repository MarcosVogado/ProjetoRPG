using System.ComponentModel.DataAnnotations;

namespace ProjetoRPG.Models;

public enum StatusMissao
{
    Planejada = 1,
    EmAndamento,
    Concluida,
    Falha
}

public class Missao
{
    public int Id { get; set; }

    [Required, StringLength(120, MinimumLength = 3)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(400)]
    public string? Descricao { get; set; }

    [Range(1, 10)]
    public int Dificuldade { get; set; }

    [Range(0, int.MaxValue)]
    public int RecompensaOuro { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? DataLimite { get; set; }

    [Required]
    public StatusMissao Status { get; set; } = StatusMissao.Planejada;

    public ICollection<PersonagemMissao> Participantes { get; set; } = new List<PersonagemMissao>();
}
