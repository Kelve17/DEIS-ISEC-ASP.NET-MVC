using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ESTAGIOS.Models
{
    [Table("Propostas")]
    public class Proposta
    {
        public int PropostaId { get; set; }

        [Required]
        public string Titulo { get; set; }


        public int? EmpresaId { get; set; }
        [ForeignKey("EmpresaId")]
        public Empresa Empresa { get; set; }


        public int? CandidaturaId { get; set; } // id da candidatura aceite
        [ForeignKey("CandidaturaId")]
        public Candidatura CandidaturaAceite { get; set; }

        [Required]
        public Ramo Ramo { get; set; }

        [Required]
        public TipoProposta Tipo { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Enquadramento { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Objectivos { get; set; }

        [Required]
        [Display(Name = "Condições de Acesso")]
        [DataType(DataType.MultilineText)]
        public string CondicoesAcesso { get; set; }

        [DataType(DataType.MultilineText)]
        public string Localizacao { get; set; }

        public int? DocenteId { get; set; }//orientador do estágio ou quem aceitou a candidatura ao projecto
        [ForeignKey("DocenteId")]
        public Docente Orientador { get; set; }

        public IList<Candidatura> Candidaturas { get; set; } = new List<Candidatura>();

        public IList<Docente> Docentes { get; set; } = new List<Docente>();

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataInicio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataFim { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataDefesa { get; set; }

        [Range(0, 20, ErrorMessage = "Introduza um número até 20 valores")]
        [Display(Name = "Classificação")]
        public int? AvaliacaoAlunoEmpresa { get; set; } // avaliacao do aluno para a empresa

        [Range(0, 20, ErrorMessage = "Introduza um número até 20 valores")]
        [Display(Name = "Classificação")]
        public int? AvaliacaoEmpresaALuno { get; set; } //avaliacao da empresa para o aluno

        [Range(0, 20, ErrorMessage = "Introduza um número até 20 valores")]
        [Display(Name = "Classificação")]
        public int? AvaliacaoDocenteAluno { get; set; } // avaliacao do docente para o aluno
    }
}