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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emprestimo>>> GetEmprestimos()
        {
            return await _context.Emprestimos
                .Include(e => e.Livro)
                .Include(e => e.Usuario)
                .ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult<Emprestimo>> PostEmprestimo(EmprestimoDTO dto)
        {
            var livro = await _context.Livros.FindAsync(dto.LivroId);
            var usuario = await _context.Usuarios.FindAsync(dto.UsuarioId);

            if (livro == null) return NotFound($"Livro com Id {dto.LivroId} não encontrado.");
            if (usuario == null) return NotFound($"Usuário com Id {dto.UsuarioId} não encontrado.");

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

            return CreatedAtAction(nameof(GetEmprestimos), new { id = emprestimo.Id }, emprestimo); 
        } 

        [HttpPut("devolver/{id}")]
        public async Task<IActionResult> DevolverEmprestimo(int id)
        {
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprestimo == null) return NotFound();

            if (emprestimo.DataDevolucao != null)
                return BadRequest("Este empréstimo já foi devolvido.");

            emprestimo.DataDevolucao = DateTime.Now;
            emprestimo.Livro.Disponivel = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }  
    }
}