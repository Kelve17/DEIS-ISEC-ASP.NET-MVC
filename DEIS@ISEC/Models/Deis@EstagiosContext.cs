using DEIS_ISEC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DEIS_ESTAGIOS.Models
{
    public class Deis_EstagiosContext : ApplicationDbContext
    {
        public Deis_EstagiosContext()
        {

        }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Candidatura> Candidaturas { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Proposta> Propostas { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Docente>().HasMany(p => p.Propostas).WithMany(d => d.Docentes).Map(m => m.ToTable("DocentesPropostas"));
        }
    }
}