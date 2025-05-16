namespace prjEventosWeb.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string ImagemUrl { get; set; }
        // Relacionamento com a tabela de Eventos
        public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}
