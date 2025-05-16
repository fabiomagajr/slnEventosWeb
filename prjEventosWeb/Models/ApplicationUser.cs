using Microsoft.AspNetCore.Identity;

namespace prjEventosWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? NomeCompleto { get; set; }
        public DateTime Criacao { get; set; } = DateTime.Now;
        public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
        // Você pode adicionar mais campos se quiser
    }

}
