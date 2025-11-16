using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;
using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly BibliotecaContext _context;

        public LivrosController(BibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LivroDTO>>> GetLivros()
        {
            return await _context.Livros
                .Include(l => l.Categorias)
                .Select(l => new LivroDTO
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Autor = l.Autor,
                    Ano = l.Ano,
                    Disponivel = l.Disponivel,
                    CategoriasIds = l.Categorias.Select(c => c.Id).ToList()
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LivroDTO>> GetLivro(int id)
        {
            var livro = await _context.Livros
                .Include(l => l.Categorias)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livro == null)
                return NotFound();

            return new LivroDTO
            {
                Id = livro.Id,
                Titulo = livro.Titulo,
                Autor = livro.Autor,
                Ano = livro.Ano,
                Disponivel = livro.Disponivel,
                CategoriasIds = livro.Categorias.Select(c => c.Id).ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult<Livro>> PostLivro(LivroDTO dto)
        {
            if (dto.CategoriasIds.Count > 3)
                return BadRequest("O livro não pode ter mais que 3 categorias.");

            var livro = new Livro
            {
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                Ano = dto.Ano,
                Disponivel = dto.Disponivel
            };

            var categorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id))
                .ToListAsync();

            livro.Categorias = categorias;

            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLivro), new { id = livro.Id }, livro);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivro(int id, LivroDTO dto)
        {
            var livro = await _context.Livros
                .Include(l => l.Categorias)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livro == null)
                return NotFound();

            if (dto.CategoriasIds.Count > 3)
                return BadRequest("O livro não pode ter mais que 3 categorias.");

            livro.Titulo = dto.Titulo;
            livro.Autor = dto.Autor;
            livro.Ano = dto.Ano;
            livro.Disponivel = dto.Disponivel;

            livro.Categorias.Clear();

            var categorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id))
                .ToListAsync();

            livro.Categorias = categorias;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            var livro = await _context.Livros
                .Include(l => l.Categorias)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livro == null)
                return NotFound();

            livro.Categorias.Clear();
            _context.Livros.Remove(livro);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
