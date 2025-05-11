using Microsoft.AspNetCore.Identity;

namespace prjEventosWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? NomeCompleto { get; set; }
        public DateTime Criacao { get; set; } = DateTime.Now;
        // Você pode adicionar mais campos se quiser
    }

}
