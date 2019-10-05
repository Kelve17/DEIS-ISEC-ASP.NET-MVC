namespace DEIS_ISEC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModeloInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alunos",
                c => new
                    {
                        AlunoId = c.Int(nullable: false, identity: true),
                        NumeroAluno = c.Int(nullable: false),
                        Ramo = c.Int(nullable: false),
                        Nome = c.String(nullable: false),
                        Apelido = c.String(nullable: false),
                        Morada = c.String(nullable: false),
                        Telemovel = c.Int(nullable: false),
                        Email = c.String(),
                        DataNascimento = c.DateTime(nullable: false),
                        NumeroEstágios = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AlunoId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Candidaturas",
                c => new
                    {
                        CandidaturaId = c.Int(nullable: false, identity: true),
                        PropostaId = c.Int(nullable: false),
                        AlunoId = c.Int(nullable: false),
                        NivelPreferencia = c.Int(nullable: false),
                        DisciplinasNaoConcluidas = c.String(nullable: false),
                        DisciplinasConcluidas = c.String(nullable: false),
                        CandidaturaAceite = c.Boolean(nullable: false),
                        CandidaturaRejeitada = c.Boolean(nullable: false),
                        Proposta_PropostaId = c.Int(),
                        Empresa_EmpresaId = c.Int(),
                    })
                .PrimaryKey(t => t.CandidaturaId)
                .ForeignKey("dbo.Alunos", t => t.AlunoId, cascadeDelete: true)
                .ForeignKey("dbo.Propostas", t => t.Proposta_PropostaId)
                .ForeignKey("dbo.Empresas", t => t.Empresa_EmpresaId)
                .ForeignKey("dbo.Propostas", t => t.PropostaId, cascadeDelete: true)
                .Index(t => t.PropostaId)
                .Index(t => t.AlunoId)
                .Index(t => t.Proposta_PropostaId)
                .Index(t => t.Empresa_EmpresaId);
            
            CreateTable(
                "dbo.Propostas",
                c => new
                    {
                        PropostaId = c.Int(nullable: false, identity: true),
                        Titulo = c.String(nullable: false),
                        EmpresaId = c.Int(),
                        CandidaturaId = c.Int(),
                        Ramo = c.Int(nullable: false),
                        Tipo = c.Int(nullable: false),
                        Enquadramento = c.String(nullable: false),
                        Objectivos = c.String(nullable: false),
                        CondicoesAcesso = c.String(nullable: false),
                        Localizacao = c.String(),
                        DocenteId = c.Int(),
                        DataInicio = c.DateTime(nullable: false),
                        DataFim = c.DateTime(nullable: false),
                        DataDefesa = c.DateTime(),
                        AvaliacaoAlunoEmpresa = c.Int(),
                        AvaliacaoEmpresaALuno = c.Int(),
                        AvaliacaoDocenteAluno = c.Int(),
                    })
                .PrimaryKey(t => t.PropostaId)
                .ForeignKey("dbo.Candidaturas", t => t.CandidaturaId)
                .ForeignKey("dbo.Docentes", t => t.DocenteId)
                .ForeignKey("dbo.Empresas", t => t.EmpresaId)
                .Index(t => t.EmpresaId)
                .Index(t => t.CandidaturaId)
                .Index(t => t.DocenteId);
            
            CreateTable(
                "dbo.Docentes",
                c => new
                    {
                        DocenteId = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Apelido = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Telefone = c.Int(nullable: false),
                        PertenceComissao = c.Boolean(nullable: false),
                        Morada = c.String(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.DocenteId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Empresas",
                c => new
                    {
                        EmpresaId = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Endereco = c.String(nullable: false),
                        AreaNegocio = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Telefone = c.Int(nullable: false),
                        Website = c.String(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.EmpresaId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Mensagens",
                c => new
                    {
                        MensagemId = c.Int(nullable: false, identity: true),
                        RemetenteId = c.String(),
                        DestinarioId = c.String(),
                        Assunto = c.String(nullable: false),
                        Conteudo = c.String(nullable: false),
                        StatusMensagem = c.Int(nullable: false),
                        Data = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MensagemId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.DocentesPropostas",
                c => new
                    {
                        Docente_DocenteId = c.Int(nullable: false),
                        Proposta_PropostaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Docente_DocenteId, t.Proposta_PropostaId })
                .ForeignKey("dbo.Docentes", t => t.Docente_DocenteId, cascadeDelete: true)
                .ForeignKey("dbo.Propostas", t => t.Proposta_PropostaId, cascadeDelete: true)
                .Index(t => t.Docente_DocenteId)
                .Index(t => t.Proposta_PropostaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Candidaturas", "PropostaId", "dbo.Propostas");
            DropForeignKey("dbo.Propostas", "EmpresaId", "dbo.Empresas");
            DropForeignKey("dbo.Candidaturas", "Empresa_EmpresaId", "dbo.Empresas");
            DropForeignKey("dbo.Empresas", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Propostas", "DocenteId", "dbo.Docentes");
            DropForeignKey("dbo.DocentesPropostas", "Proposta_PropostaId", "dbo.Propostas");
            DropForeignKey("dbo.DocentesPropostas", "Docente_DocenteId", "dbo.Docentes");
            DropForeignKey("dbo.Docentes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Candidaturas", "Proposta_PropostaId", "dbo.Propostas");
            DropForeignKey("dbo.Propostas", "CandidaturaId", "dbo.Candidaturas");
            DropForeignKey("dbo.Candidaturas", "AlunoId", "dbo.Alunos");
            DropForeignKey("dbo.Alunos", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.DocentesPropostas", new[] { "Proposta_PropostaId" });
            DropIndex("dbo.DocentesPropostas", new[] { "Docente_DocenteId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Empresas", new[] { "UserId" });
            DropIndex("dbo.Docentes", new[] { "UserId" });
            DropIndex("dbo.Propostas", new[] { "DocenteId" });
            DropIndex("dbo.Propostas", new[] { "CandidaturaId" });
            DropIndex("dbo.Propostas", new[] { "EmpresaId" });
            DropIndex("dbo.Candidaturas", new[] { "Empresa_EmpresaId" });
            DropIndex("dbo.Candidaturas", new[] { "Proposta_PropostaId" });
            DropIndex("dbo.Candidaturas", new[] { "AlunoId" });
            DropIndex("dbo.Candidaturas", new[] { "PropostaId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Alunos", new[] { "UserId" });
            DropTable("dbo.DocentesPropostas");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Mensagens");
            DropTable("dbo.Empresas");
            DropTable("dbo.Docentes");
            DropTable("dbo.Propostas");
            DropTable("dbo.Candidaturas");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Alunos");
        }
    }
}
