using DEIS_ESTAGIOS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DEIS_ISEC.Models
{
    [Table("Mensagens")]
    public class Mensagem
    {
        public int MensagemId { get; set; }

        public string RemetenteId { get; set; }

        public string DestinarioId { get; set; }

        [Required]
        public string Assunto { get; set; }

        [Required]
        [Display(Name = "Mensagem")]
        [DataType(DataType.MultilineText)]
        public string Conteudo { get; set; }

        public statusMensagem StatusMensagem { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;
    }
}