namespace BibliotecaAPI.DTOs
{
    public class LivroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string Autor { get; set; } = "";
        public int Ano { get; set; }
        public bool Disponivel { get; set; }

        // Lista de categorias
        public List<int> CategoriasIds { get; set; } = new();
    }
}
