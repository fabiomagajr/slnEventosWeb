namespace prjEventosWeb.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateUserViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        public string? Nome { get; set; }

        public bool IsAdmin { get; set; }
    }

}
