using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Models
{
    public class Livro
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Autor { get; set; }

        public int Ano { get; set; }

        public bool Disponivel { get; set; } = true;

        public List<Categoria> Categorias { get; set; } = new();
    }
}
