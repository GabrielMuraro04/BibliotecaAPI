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
        public async Task<ActionResult<IEnumerable<LivroDTO>>> GetLivros()
        {
            var livros = await _context.Livros
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

            return livros;
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
        public async Task<ActionResult<LivroDTO>> PostLivro([FromBody] LivroDTO dto)
        {
            if (dto.CategoriasIds.Count > 3)
                return BadRequest("Um livro não pode ter mais de 3 categorias.");

            var categorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id))
                .ToListAsync();

            var livro = new Livro
            {
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                Ano = dto.Ano,
                Disponivel = true,
                Categorias = categorias
            };

            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();

            dto.Id = livro.Id;

            return CreatedAtAction(nameof(GetLivro), new { id = livro.Id }, dto);
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
                return BadRequest("Um livro não pode ter mais de 3 categorias.");

            livro.Titulo = dto.Titulo;
            livro.Autor = dto.Autor;
            livro.Ano = dto.Ano;

            livro.Categorias.Clear();

            var novasCategorias = await _context.Categorias
                .Where(c => dto.CategoriasIds.Contains(c.Id))
                .ToListAsync();

            foreach (var cat in novasCategorias)
                livro.Categorias.Add(cat);

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
