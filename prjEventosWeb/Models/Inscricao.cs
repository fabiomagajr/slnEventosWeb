namespace prjEventosWeb.Models
{
    public class Inscricao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataInscricao { get; set; }
        public int EventoId { get; set; }
        // Relacionamento com a tabela de Eventos
        public Evento Evento
        {
            get; set;
        }
    }
}
