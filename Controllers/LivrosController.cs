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

        // GET: api/livros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LivroDTO>>> GetLivros()
        {
            var livros = await _context.Livros
                .Select(l => new LivroDTO
                {
                    Id = l.Id,
                    Titulo = l.Titulo,
                    Autor = l.Autor,
                    Ano = l.Ano,
                    Disponivel = l.Disponivel
                })
                .ToListAsync();

            return livros;    
        }

        // GET: api/livros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LivroDTO>> GetLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
                return NotFound();
            
            return new LivroDTO
            {
                Id = livro.Id,
                Titulo = livro.Titulo,
                Autor = livro.Autor,
                Ano = livro.Ano,
                Disponivel = livro.Disponivel
            };
        }

        // POST: api/livros
        [HttpPost]
        public async Task<ActionResult<LivroDTO>> PostLivro(LivroDTO dto)
        {
            var livro = new Livro
            {
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                Ano = dto.Ano,
                Disponivel = dto.Disponivel
            };

            _context.Livros.Add(livro);
            await _context.SaveChangesAsync();

            dto.Id = livro.Id;

            return CreatedAtAction(nameof(GetLivro), new { id = livro.Id }, dto);
        }

        // PUT: api/livros/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivro(int id, LivroDTO dto)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
                return NotFound();

            livro.Titulo = dto.Titulo;
            livro.Autor = dto.Autor;
            livro.Ano = dto.Ano;
            livro.Disponivel = dto.Disponivel;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/livros/5
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