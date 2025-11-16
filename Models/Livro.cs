using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Models
{
    public class Livro
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = "";

        [Required]
        public string Autor { get; set; } = "";

        public int Ano { get; set; }

        public bool Disponivel { get; set; } = true;
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
