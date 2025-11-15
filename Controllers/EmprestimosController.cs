using Microsoft.AspNetCore.Mvc;
using BibliotecaAPI.Data;
using BibliotecaAPI.Models;
using BibliotecaAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmprestimosController : ControllerBase
    {
        private readonly BibliotecaContext _context;

        public EmprestimosController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: api/emprestimos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emprestimo>>> GetEmprestimos()
        {
            return await _context.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .ToListAsync();
        }

        // GET: api/emprestimos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Emprestimo>> GetEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprestimo == null)
                return NotFound();

            return emprestimo;
        }

        // GET: api/emprestimos/usuario/5
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Emprestimo>>> GetEmprestimosPorUsuario(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return NotFound($"Usuário com Id {usuarioId} não encontrado.");

            var emprestimos = await _context.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .Where(e => e.UsuarioId == usuarioId)
                .OrderByDescending(e => e.DataEmprestimo)
                .ToListAsync();

            return emprestimos;
        }

        // POST: api/emprestimos
        [HttpPost]
        public async Task<ActionResult<Emprestimo>> PostEmprestimo(EmprestimoDTO dto)
        {
            var livro = await _context.Livros.FindAsync(dto.LivroId);
            var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);

            if (livro == null)
                return NotFound($"Livro com Id {dto.LivroId} não encontrado.");

            if (usuario == null)
                return NotFound($"Usuário com Id {dto.UsuarioId} não encontrado.");

            if (!livro.Disponivel)
                return BadRequest($"Livro '{livro.Titulo}' não está disponível no momento.");

            var emprestimo = new Emprestimo
            {
                LivroId = dto.LivroId,
                UsuarioId = dto.UsuarioId,
                DataEmprestimo = DateTime.Now
            };

            livro.Disponivel = false;

            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmprestimo), new { id = emprestimo.Id }, emprestimo);
        }

        // PUT: api/Emprestimos/devolver/{id}
        [HttpPut("devolver/{id}")]
        public async Task<IActionResult> DevolverEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprestimo == null)
                return NotFound(new { message = "Empréstimo não encontrado." });

            if (emprestimo.DataDevolucao != null)
                return BadRequest(new { message = "Este empréstimo já foi devolvido." });

            // Marca a devolução
            emprestimo.DataDevolucao = DateTime.Now;

            // Deixa o livro disponível novamente
            emprestimo.Livro.Disponivel = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Livro devolvido com sucesso!",
                emprestimo.Id,
                emprestimo.UsuarioId,
                emprestimo.LivroId,
                emprestimo.DataEmprestimo,
                emprestimo.DataDevolucao
            });
        }

    }
}
