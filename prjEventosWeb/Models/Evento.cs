namespace prjEventosWeb.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Local { get; set; }
        public decimal Preco { get; set; }
        public int Vagas { get; set; }
        public string ImagemUrl { get; set; }
        // Relacionamento com a tabela de Inscrições
        public int CategoriaId { get; set; }
        public List<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
    }
}
