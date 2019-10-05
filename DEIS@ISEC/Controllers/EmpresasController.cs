using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    [Authorize(Roles = "Empresas,Admin")]
    public class EmpresasController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private Deis_EstagiosContext db;

        public EmpresasController()
        {
            db = new Deis_EstagiosContext();
        }

        public EmpresasController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Empresas
        public ActionResult Index()
        {
            return View(db.Empresas.ToList());
        }

        // GET: Empresas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // GET: Empresas/Create
        public ActionResult Create()
        {
            var user = User.Identity.GetUserId();
            Empresa s = new Empresa();
            s.Email = db.Users.SingleOrDefault(u => u.Id == user.ToString()).Email;
            return View(s);
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmpresaId,Nome,Endereco,AreaNegocio,Email,Telefone,Website,UserId")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                db.Empresas.Add(empresa);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(empresa);
        }

        // GET: Empresas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpresaId,Nome,Endereco,AreaNegocio,Email,Telefone,Website")] Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empresa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empresa);
        }

        // GET: Empresas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresas.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresa = db.Empresas.Find(id);
            db.Empresas.Remove(empresa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AceitarCandidatura(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidatura candidatura = db.Candidaturas.Find(id);
            if (candidatura == null)
            {
                return HttpNotFound();
            }
            ViewBag.NomeAluno = db.Alunos.SingleOrDefault(x => x.AlunoId == candidatura.AlunoId).Nome+" "+ db.Alunos.SingleOrDefault(x => x.AlunoId == candidatura.AlunoId).Apelido;
            ViewBag.Ramo = db.Alunos.SingleOrDefault(x => x.AlunoId == candidatura.AlunoId).Ramo;
            return View(candidatura);
        }

        public ActionResult ConfirmarCandidatura(int id)
        {
            Candidatura candidatura = db.Candidaturas.Find(id);
            db.Propostas.SingleOrDefault(x => x.PropostaId == candidatura.PropostaId).CandidaturaAceite = candidatura;
            db.Propostas.SingleOrDefault(x => x.PropostaId == candidatura.PropostaId).CandidaturaId = candidatura.CandidaturaId;
            var candidaturas = db.Candidaturas.Where(x => x.PropostaId == candidatura.PropostaId).ToList();
            foreach(Candidatura c in candidaturas)
            {
                db.Candidaturas.SingleOrDefault(u => u.CandidaturaId == c.CandidaturaId).CandidaturaRejeitada = true;
            }
            db.Candidaturas.Find(id).CandidaturaAceite = true;
            db.Candidaturas.Find(id).CandidaturaRejeitada = false;
            db.SaveChanges();
            return RedirectToAction("CandidaturasRecebidas","Candidaturas");
        }

        [AllowAnonymous]
        public ActionResult AvaliaEmpresa()
        {
            var user = User.Identity.GetUserId();
            Aluno a = db.Alunos.SingleOrDefault(x => x.UserId == user);

            var vm = new ViewModels
            {
                Propostas = db.Propostas.ToList(),
                Empresas = db.Empresas.ToList(),
                Candidaturas = db.Candidaturas.Where(x=>x.AlunoId == a.AlunoId && x.CandidaturaAceite == true).ToList()
            };
            return View(vm);
        }

        [AllowAnonymous]
        public ActionResult ConfirmaAvaliacao(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidatura c = db.Candidaturas.SingleOrDefault(x => x.CandidaturaId == id);
            if (c == null)
            {
                return HttpNotFound();
            }
            Proposta p = db.Propostas.SingleOrDefault(x => x.CandidaturaId == c.CandidaturaId);

            if (p != null)
            {
                ViewBag.NomeEmpresa = db.Empresas.SingleOrDefault(x => x.EmpresaId == p.EmpresaId).Nome;
                return View(p);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ConfirmaAvaliacao([Bind(Include = "PropostaId, Titulo, EmpresaId, CandidaturaId, Ramo, Tipo, Enquadramento, Objectivos, CondicoesAcesso, Localizacao, DataInicio, DataFim, AvaliacaoAlunoEmpresa, AvaliacaoEmpresaALuno, AvaliacaoDocenteAluno")] Proposta proposta)
        {
            Proposta p = db.Propostas.SingleOrDefault(x => x.PropostaId == proposta.PropostaId);
            p.AvaliacaoAlunoEmpresa = proposta.AvaliacaoAlunoEmpresa;

            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();
            return View("AvaliaEmpresa");
        }
    }
}
