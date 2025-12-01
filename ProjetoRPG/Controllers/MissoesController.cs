using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoRPG.Data;
using ProjetoRPG.Models;

namespace ProjetoRPG.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MissoesController : ControllerBase
{
    private readonly RpgContext _context;

    public MissoesController(RpgContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Missao>>> Listar()
    {
        var missoes = await _context.Missoes
            .Include(m => m.Participantes)
                .ThenInclude(pm => pm.Personagem)
            .AsNoTracking()
            .ToListAsync();

        return Ok(missoes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Missao>> Obter(int id)
    {
        var missao = await _context.Missoes
            .Include(m => m.Participantes)
                .ThenInclude(pm => pm.Personagem)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (missao is null)
        {
            return NotFound();
        }

        return missao;
    }

    [HttpPost]
    public async Task<ActionResult<Missao>> Criar(Missao missao)
    {
        if (!ValidarDataLimite(missao.DataLimite))
        {
            ModelState.AddModelError(nameof(missao.DataLimite), "A data limite precisa ser futura.");
            return ValidationProblem(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        _context.Missoes.Add(missao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = missao.Id }, missao);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, Missao missao)
    {
        if (id != missao.Id)
        {
            ModelState.AddModelError(nameof(missao.Id), "O id do caminho e do corpo precisam ser iguais.");
            return ValidationProblem(ModelState);
        }

        if (!ValidarDataLimite(missao.DataLimite))
        {
            ModelState.AddModelError(nameof(missao.DataLimite), "A data limite precisa ser futura.");
            return ValidationProblem(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        _context.Entry(missao).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await MissaoExiste(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var missao = await _context.Missoes.FindAsync(id);
        if (missao is null)
        {
            return NotFound();
        }

        _context.Missoes.Remove(missao);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/personagens")]
    public async Task<IActionResult> AdicionarPersonagem(int id, AtribuirPersonagemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var missaoExiste = await _context.Missoes.AnyAsync(m => m.Id == id);
        if (!missaoExiste)
        {
            return NotFound($"Missão {id} não encontrada.");
        }

        var personagem = await _context.Personagens.FindAsync(request.PersonagemId);
        if (personagem is null)
        {
            return NotFound($"Personagem {request.PersonagemId} não encontrado.");
        }

        var duplicada = await _context.PersonagensMissoes
            .AnyAsync(pm => pm.MissaoId == id && pm.PersonagemId == request.PersonagemId);

        if (duplicada)
        {
            ModelState.AddModelError(nameof(request.PersonagemId), "Este personagem já está escalado para a missão.");
            return ValidationProblem(ModelState);
        }

        var novaRelacao = new PersonagemMissao
        {
            MissaoId = id,
            PersonagemId = request.PersonagemId,
            FuncaoNoGrupo = request.FuncaoNoGrupo
        };

        _context.PersonagensMissoes.Add(novaRelacao);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}/personagens/{personagemId:int}")]
    public async Task<IActionResult> RemoverPersonagem(int id, int personagemId)
    {
        var relacao = await _context.PersonagensMissoes.FindAsync(personagemId, id);
        if (relacao is null)
        {
            return NotFound();
        }

        _context.PersonagensMissoes.Remove(relacao);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static bool ValidarDataLimite(DateOnly? dataLimite)
    {
        if (dataLimite is null)
        {
            return true;
        }

        return dataLimite.Value >= DateOnly.FromDateTime(DateTime.Today);
    }

    private Task<bool> MissaoExiste(int id) =>
        _context.Missoes.AnyAsync(m => m.Id == id);
}

public class AtribuirPersonagemRequest
{
    [Required]
    public int PersonagemId { get; set; }

    [Required, StringLength(40, MinimumLength = 3)]
    public string FuncaoNoGrupo { get; set; } = string.Empty;
}
