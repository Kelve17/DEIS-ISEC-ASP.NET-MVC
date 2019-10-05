using DEIS_ESTAGIOS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ISEC.Models
{
    [NotMapped]
    public class ViewModels
    {
        public int Id { get; set; }
        public IList<Aluno> Alunos { get; set; } = new List<Aluno>();
        public IList<Candidatura> Candidaturas { get; set; } = new List<Candidatura>();
        public IList<Docente> Docentes { get; set; } = new List<Docente>();
        public IList<Empresa> Empresas { get; set; } = new List<Empresa>();
        public IList<Proposta> Propostas { get; set; } = new List<Proposta>();
        public IList<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
    }
}