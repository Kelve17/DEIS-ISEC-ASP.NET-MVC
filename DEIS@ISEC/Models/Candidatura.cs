using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ESTAGIOS.Models
{
    [Table("Candidaturas")]
    public class Candidatura
    {
        public int CandidaturaId { get; set; }

        public int PropostaId { get; set; }
        [ForeignKey("PropostaId")]
        public Proposta Proposta { get; set; }


        public int AlunoId { get; set; }
        [ForeignKey("AlunoId")]
        public Aluno Aluno { get; set; }


        [Required]
        [Range(0,10,ErrorMessage = "Número inteiro de 0 a 10")]
        [Display(Name = "Nível de Preferência")]
        public int NivelPreferencia { get; set; }


        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Disciplinas não concluídas")]
        public string DisciplinasNaoConcluidas { get; set; }


        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Disciplinas concluídas")]
        public string DisciplinasConcluidas{ get; set; }

        public bool CandidaturaAceite { get; set; }

        public bool CandidaturaRejeitada { get; set; }

    }
}