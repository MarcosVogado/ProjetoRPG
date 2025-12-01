using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoRPG.Data;
using ProjetoRPG.Models;

namespace ProjetoRPG.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonagensController : ControllerBase
{
    private readonly RpgContext _context;

    public PersonagensController(RpgContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Personagem>>> Listar()
    {
        var personagens = await _context.Personagens
            .AsNoTracking()
            .ToListAsync();

        return Ok(personagens);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Personagem>> Obter(int id)
    {
        var personagem = await _context.Personagens
            .Include(p => p.Missoes)
                .ThenInclude(pm => pm.Missao)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (personagem is null)
        {
            return NotFound();
        }

        return personagem;
    }

    [HttpPost]
    public async Task<ActionResult<Personagem>> Criar(Personagem personagem)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        _context.Personagens.Add(personagem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = personagem.Id }, personagem);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, Personagem personagem)
    {
        if (id != personagem.Id)
        {
            ModelState.AddModelError(nameof(personagem.Id), "O id do caminho e do corpo precisam ser iguais.");
            return ValidationProblem(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        _context.Entry(personagem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PersonagemExiste(id))
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
        var personagem = await _context.Personagens.FindAsync(id);
        if (personagem is null)
        {
            return NotFound();
        }

        _context.Personagens.Remove(personagem);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private Task<bool> PersonagemExiste(int id) =>
        _context.Personagens.AnyAsync(p => p.Id == id);
}
