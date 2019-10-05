using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DEIS_ESTAGIOS;
using DEIS_ESTAGIOS.Models;
using DEIS_ISEC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DEIS_ISEC.Controllers
{
    [Authorize(Roles ="Alunos,Admin,Comissao,Docentes,Empresas")]
    public class AlunosController : Controller
    {
        private Deis_EstagiosContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AlunosController()
        {
            db = new Deis_EstagiosContext();
        }

        public AlunosController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Alunos
        public ActionResult Index()
        {
            return View(db.Alunos.ToList());
        }

        [AllowAnonymous]
        // GET: Alunos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aluno aluno = db.Alunos.SingleOrDefault(x=>x.NumeroAluno == id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            return View(aluno);
        }

        // GET: Alunos/Create
        public ActionResult Create()
        {
            var user = User.Identity.GetUserId();
            Aluno a = new Aluno
            {
                Email = db.Users.SingleOrDefault(u => u.Id == user.ToString()).Email
            };
            return View(a);
        }

        // POST: Alunos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NumeroAluno,Ramo,Nome,Apelido,Morada,Telemovel,Email,DataNascimento,NumeroEstágios,UserId")] Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                db.Alunos.Add(aluno);
                db.SaveChanges();
                return RedirectToAction("ListaPropostas");
            }

            return View(aluno);
        }

        // GET: Alunos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aluno aluno = db.Alunos.SingleOrDefault(x=>x.NumeroAluno == id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            return View(aluno);
        }

        // POST: Alunos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NumeroAluno,Ramo,Nome,Apelido,Morada,Telemovel,Email,NumeroEstágios")] Aluno aluno)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aluno).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aluno);
        }

        // GET: Alunos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aluno aluno = db.Alunos.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            return View(aluno);
        }

        // POST: Alunos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aluno aluno = db.Alunos.Find(id);
            db.Alunos.Remove(aluno);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        public ActionResult AvaliaAluno()
        {
            var user = User.Identity.GetUserId();
            if (User.IsInRole("Docentes") || User.IsInRole("Comissao"))
            {
                Docente d = db.Docentes.SingleOrDefault(x => x.UserId == user);

                var vm = new ViewModels
                {
                    Propostas = db.Propostas.Where(x => x.DocenteId == d.DocenteId && x.AvaliacaoDocenteAluno== null).ToList(),
                    Alunos = db.Alunos.ToList(),
                    Candidaturas = db.Candidaturas.ToList()
                };
                return View(vm);
            }

            if (User.IsInRole("Empresas"))
            {
                Empresa d = db.Empresas.SingleOrDefault(x => x.UserId == user);

                var vm = new ViewModels
                {
                    Propostas = db.Propostas.Where(x => x.EmpresaId == d.EmpresaId && x.AvaliacaoEmpresaALuno == null && x.CandidaturaId != null).ToList(),
                    Alunos = db.Alunos.ToList(),
                    Candidaturas = db.Candidaturas.ToList()
                };
                return View(vm);
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmaAvaliacao(int id)
        {
            Candidatura c = db.Candidaturas.SingleOrDefault(x => x.CandidaturaId == id);
            ViewBag.NomeAluno = db.Alunos.SingleOrDefault(x => x.AlunoId == c.AlunoId).Nome + " " + db.Alunos.SingleOrDefault(x => x.AlunoId == c.AlunoId).Apelido;
            ViewBag.NumeroAluno = db.Alunos.SingleOrDefault(x => x.AlunoId == c.AlunoId).NumeroAluno;
            ViewBag.RamoAluno = db.Alunos.SingleOrDefault(x => x.AlunoId == c.AlunoId).Ramo;
            Proposta p = db.Propostas.SingleOrDefault(x => x.CandidaturaId == c.CandidaturaId);
            if (User.IsInRole("Docentes") || User.IsInRole("Comissao"))
            {
                return View(p);
            }

            if (User.IsInRole("Empresas"))
            {
                return View(p);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ConfirmaAvaliacao([Bind(Include = "PropostaId, Titulo, EmpresaId, CandidaturaId, Ramo, Tipo, Enquadramento, Objectivos, CondicoesAcesso, Localizacao, DataInicio, DataFim, AvaliacaoAlunoEmpresa, AvaliacaoEmpresaALuno, AvaliacaoDocenteAluno")] Proposta proposta)
        {
            Proposta p = db.Propostas.SingleOrDefault(x => x.PropostaId == proposta.PropostaId);
            if (User.IsInRole("Docentes") || User.IsInRole("Comissao"))
            {
                p.AvaliacaoDocenteAluno = proposta.AvaliacaoDocenteAluno;
            }

            if (User.IsInRole("Empresas"))
            {
                p.AvaliacaoEmpresaALuno = proposta.AvaliacaoEmpresaALuno;
            }

            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();
            return View("AvaliaAluno");
        }

        [AllowAnonymous]
        public ActionResult ListaAlunos()
        {
            ViewModels vm = new ViewModels
            {
                Alunos = db.Alunos.ToList(),
                Candidaturas = db.Candidaturas.ToList(),
                Propostas = db.Propostas.ToList()
            };

            return View(vm);
        }

        [Authorize(Roles = "Comissao,Admin")]
        public ActionResult DefinirNumeroEstagios()
        {
            return View(db.Alunos.ToList());
        }


        [Authorize(Roles = "Admin,Comissao")]
        public ActionResult ConfirmaNumeroEstagios(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aluno a = db.Alunos.Find(id);
            if (a == null)
            {
                return HttpNotFound();
            }

            return View(a);
        }

        [Authorize(Roles = "Admin,Comissao")]
        [HttpPost]
        public ActionResult ConfirmaNumeroEstagios(Aluno a)
        {
            Aluno aluno = db.Alunos.SingleOrDefault(x=>x.AlunoId == a.AlunoId);
            aluno.NumeroEstágios = a.NumeroEstágios;
            db.Entry(aluno).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DefinirNumeroEstagios");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ListaPropostas()
        {
            return View(db.Propostas.Where(x=>x.CandidaturaId == null).ToList());
        }

    }
}
