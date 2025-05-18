namespace prjEventosWeb.Models
{
    public class Inscricao
    {
        
        public int Id { get; set; }

        public int EventoId { get; set; }
        public virtual Evento Evento { get; set; } // Propriedade de navegação

        public string ApplicationUserId { get; set; } // Chave estrangeira para o AppUser
        public virtual ApplicationUser AppUser { get; set; } // Propriedade de navegação
        //public DateTime DataInscricao { get; set; }
        // Você pode adicionar outras propriedades, como DataInscricao
        public DateTime DataInscricao { get; set; } = DateTime.UtcNow;

    }
}
