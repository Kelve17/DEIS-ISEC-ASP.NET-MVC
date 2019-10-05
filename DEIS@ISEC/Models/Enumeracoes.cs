using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DEIS_ESTAGIOS.Models
{
    public enum Ramo
    {
        [Display(Name = "Sistemas de Informação")]
        SistemasDeInformação = 1,

        [Display(Name = "Desenvolvimento de Aplicações")]
        DesenvolvimentoDeAplicação = 2,

        [Display(Name = "Administração de Redes")]
        AdministraçãoDeRedes = 3
    };

    public enum TipoProposta
    {
        Estágio = 1,
        Projecto = 2
    };

    public enum TipoAvaliacao
    {
        AlunoEmpresa = 1,
        EmpresaAluno = 2,
        DocenteAluno =3
    };

    public enum Estado
    {
        Rejeitada = 1,
        Aprovada = 2
    }

    public enum Role
    {
        Docentes = 1,
        Alunos = 2,
        Empresas = 3
    }

    public enum statusMensagem
    {
        Enviada,
        Entregue
    }

}