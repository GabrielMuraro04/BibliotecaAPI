using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Models;

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
        public async Task<ActionResult<IEnumerable<object>>> GetLivros([FromQuery] string? titulo)
        {
            var query = _context.Livros
                .Include(l => l.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                query = query.Where(l => l.Titulo.Contains(titulo));
            }

            var livros = await query
                .Select(l => new
                {
                    l.Id,
                    l.Titulo,
                    l.Autor,
                    l.Ano,
                    l.Disponivel,
                    Categoria = l.Categoria == null ? null : new { l.Categoria.Id, l.Categoria.Nome }
                })
                .ToListAsync();

            return livros;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetLivro(int id)
        {
            var livro = await _context.Livros
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livro == null)
                return NotFound();

            return new
            {
                livro.Id,
                livro.Titulo,
                livro.Autor,
                livro.Ano,
                livro.Disponivel,
                Categoria = livro.Categoria == null ? null : new { livro.Categoria.Id, livro.Categoria.Nome }
            };
        }

        [HttpPost]
        public async Task<ActionResult<object>> PostLivro([FromBody] LivroDTO dto)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Livros)
                .FirstOrDefaultAsync(c => c.Id == dto.CategoriaId);

            if (categoria == null)
                return NotFound($"Categoria com Id {dto.CategoriaId} não encontrada.");

            if (categoria.Livros.Count >= 3)
                return BadRequest($"A categoria '{categoria.Nome}' já possui 3 livros. Não é possível adicionar mais.");

            var livro = new Livro
            {
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                Ano = dto.Ano,
                Disponivel = dto.Disponivel,
                CategoriaId = dto.CategoriaId
            };

            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLivro), new { id = livro.Id }, new { livro.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivro(int id, [FromBody] LivroDTO dto)
        {
            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
                return NotFound();

            var categoria = await _context.Categorias
                .Include(c => c.Livros)
                .FirstOrDefaultAsync(c => c.Id == dto.CategoriaId);

            if (categoria == null)
                return NotFound($"Categoria com Id {dto.CategoriaId} não encontrada.");

            if (livro.CategoriaId != dto.CategoriaId && categoria.Livros.Count >= 3)
                return BadRequest($"A categoria '{categoria.Nome}' já possui 3 livros. Não é possível mover este livro para essa categoria.");

            livro.Titulo = dto.Titulo;
            livro.Autor = dto.Autor;
            livro.Ano = dto.Ano;
            livro.Disponivel = dto.Disponivel;
            livro.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
                return NotFound();

            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
