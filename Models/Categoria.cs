using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = "";
        public List<Livro> Livros { get; set; } = new();
    }
}
