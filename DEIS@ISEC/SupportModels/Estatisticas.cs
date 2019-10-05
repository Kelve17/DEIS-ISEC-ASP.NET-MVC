using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ISEC.SupportModels
{
    [NotMapped]
    public class Estatisticas
    {
        public int EstatisticasId { get; set; }
        public int TotalEstagios { get; set; }
        public int TotalProjetos { get; set; }
        public int TotalCandidaturas { get; set; }
        public int TotalEmpresas { get; set; }
        public int TotalAlunos { get; set; }
        public int TotalDocentes { get; set; }
        public string EmpresaComMaisPropostas { get; set; }
        public string AlunoComMaisCandidaturas { get; set; }
        public string DocentesComMaisPropostas { get; set; }
        public List<string> EmpresasMaisProcuradas { get; set; } = new List<string>();
    }
}