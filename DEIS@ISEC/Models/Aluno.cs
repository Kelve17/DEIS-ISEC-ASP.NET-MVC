using Deis.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ESTAGIOS.Models
{
    [Table("Alunos")]
    public class Aluno
    {
        public int AlunoId { get; set; }


        [Required]
        [Display(Name = "Número de Aluno")]
        public int NumeroAluno { get; set; }


        [Required]
        public Ramo Ramo { get; set; }

        [Required]
        [MaxPalavras(1)]
        public string Nome { get; set; }

        [Required]
        [MaxPalavras(2)]
        public string Apelido { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Morada { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("([0-9]{9})", ErrorMessage = "Número de Telemóvel inválido.")]
        public int Telemovel { get; set; }


        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Date de Nascimento")]
        [DataType(DataType.Date)]
        [ValidaDataNascimento(ErrorMessage = "Data não pode ser superior a data atual")]
        public DateTime DataNascimento { get; set; }

        public int NumeroEstágios { get; set; }

        public IList<Candidatura> Candidaturas { get; set; } = new List<Candidatura>();

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}