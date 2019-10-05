using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ESTAGIOS.Models
{
    [Table("Empresas")]
    public class Empresa
    {
        public int EmpresaId { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }


        [Required]
        [Display(Name = "Área de Negócio")]
        public string AreaNegocio { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("([0-9]{9})",ErrorMessage = "Número de Telefone inválido!")]
        public int Telefone { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string Website { get; set; }

        public IList<Proposta> Propostas { get; set; } = new List<Proposta>();

        public IList<Candidatura> Candidaturas { get; set; } = new List<Candidatura>();

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}