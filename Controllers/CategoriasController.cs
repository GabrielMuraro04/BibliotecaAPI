using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly BibliotecaContext _context;

        public CategoriasController(BibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Livros)
                .Select(c => new 
                {
                    c.Id,
                    c.Nome,
                    LivrosIds = c.Livros.Select(l => l.Id).ToList()
                })
                .ToListAsync();

            return categorias;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Livros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            return new
            {
                categoria.Id,
                categoria.Nome,
                LivrosIds = categoria.Livros.Select(l => l.Id).ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult<object>> PostCategoria([FromBody] Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nome))
                return BadRequest("O nome da categoria é obrigatório.");

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoriaAtualizada)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(categoriaAtualizada.Nome))
                return BadRequest("O nome da categoria é obrigatório.");

            categoria.Nome = categoriaAtualizada.Nome;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Livros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            categoria.Livros.Clear();

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
