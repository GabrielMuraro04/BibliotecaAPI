using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Data
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
            : base(options)
        {
        }

        // Aqui você adiciona os DbSets para suas entidades
        public DbSet<Livro> Livros { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
    }
}
