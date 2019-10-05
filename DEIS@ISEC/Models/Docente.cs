using Deis.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ESTAGIOS.Models
{
    [Table("Docentes")]
    public class Docente
    {
        public int DocenteId { get; set; }

        [Required]
        [MaxPalavras(1)]
        public string Nome { get; set; }

        [Required]
        [MaxPalavras(2)]
        public string Apelido { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.PhoneNumber)]
        public int Telefone { get; set; }

        public bool PertenceComissao { get; set; }

        [Required]
        public string Morada { get; set; }

        public IList<Proposta> Propostas { get; set; } = new List<Proposta>();

        public IList<Proposta> PropostasOrientadas { get; set; } = new List<Proposta>();

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}