using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace prjEventosWeb.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Apresentador { get; set; }
        public string UrlImgApresentador { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Local { get; set; }
        public double Preco { get; set; }
        public int LimiteVagas { get; set; }
        public string ImagemUrl { get; set; }
        // Relacionamento com a tabela de Inscrições
        public int CategoriaId { get; set; }
        [ValidateNever]
        public virtual Categoria Categoria { get; set; } = null!; // Propriedade de navegação para Categoria
        // public List<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
        [ValidateNever]
        public virtual ICollection<Inscricao> Inscricoes { get; set; } = null!;

        // Propriedade calculada (não mapeada para o banco) para vagas disponíveis
        [NotMapped]
        public int VagasDisponiveis => LimiteVagas - (Inscricoes?.Count() ?? 0);

        // Propriedade calculada (não mapeada para o banco) para saber quantos já se inscreveram
        [NotMapped]
        public int Inscritos => Inscricoes?.Count() ?? 0;

    }
}
