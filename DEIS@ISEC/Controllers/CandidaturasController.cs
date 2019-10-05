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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DEIS_ISEC.Controllers
{
    [Authorize(Roles="Docentes,Empresas,Alunos,Admin,Comissao")]
    public class CandidaturasController : Controller
    {

        private Deis_EstagiosContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public CandidaturasController()
        {
            db = new Deis_EstagiosContext();
        }

        public CandidaturasController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: Candidaturas
        public ActionResult Index()
        {
            var candidaturas = db.Candidaturas.Include(c => c.Aluno).Include(c => c.Proposta);
            return View(candidaturas.ToList());
        }

        // GET: Candidaturas/Details/5
        public ActionResult Details(int? id)
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
            ViewBag.CandidaturaId = candidatura.CandidaturaId;
            return View(candidatura);
        }

        // GET: Candidaturas/Create
        [Authorize(Roles = "Alunos,Admin")]
        public ActionResult Create(int id)
        {
            var user = User.Identity.GetUserId();
            Aluno a = db.Alunos.SingleOrDefault(x => x.UserId == user);
            if (a.NumeroEstágios <= 0)
            {
                TempData["alertMessage"] = "Não é-lhe possível candidatar a mais propostas.\n Atingiu o limite imposto pela comissão de estágios.";
                return View();
            }
            else
            {
                Candidatura c = new Candidatura();
                c.PropostaId = id;
                return View(c);
            }
        }

        // POST: Candidaturas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Alunos,Admin")]
        public ActionResult Create([Bind(Include = "CandidaturaId,PropostaId,AlunoId,NivelPreferencia,DisciplinasNaoConcluidas,DisciplinasConcluidas")] Candidatura candidatura)
        {
            if (ModelState.IsValid)
            {
                var user = User.Identity.GetUserId();
                candidatura.Aluno = db.Alunos.SingleOrDefault(a => a.UserId == user);
                candidatura.AlunoId = candidatura.Aluno.NumeroAluno;
                candidatura.Proposta = db.Propostas.SingleOrDefault(x=>x.PropostaId == candidatura.PropostaId);
                Aluno al = db.Alunos.SingleOrDefault(x => x.NumeroAluno == candidatura.AlunoId);
                al.NumeroEstágios--;
                Empresa e = db.Empresas.SingleOrDefault(x => x.EmpresaId == db.Propostas.FirstOrDefault(y => y.PropostaId == candidatura.PropostaId).EmpresaId);
                e.Candidaturas.Add(candidatura);
                db.Candidaturas.Add(candidatura);
                db.SaveChanges();
                return RedirectToAction("MinhasCandidaturas");
            }

            return View(candidatura);
        }

        // GET: Candidaturas/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.AlunoId = new SelectList(db.Alunos, "NumeroAluno", "Nome", candidatura.AlunoId);
            ViewBag.PropostaId = new SelectList(db.Propostas, "PropostaId", "Enquadramento", candidatura.PropostaId);
            return View(candidatura);
        }

        // POST: Candidaturas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidaturaId,PropostaId,AlunoId,DisciplinasNaoConcluidas,DisciplinasConcluidas")] Candidatura candidatura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidatura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AlunoId = new SelectList(db.Alunos, "NumeroAluno", "Nome", candidatura.AlunoId);
            ViewBag.PropostaId = new SelectList(db.Propostas, "PropostaId", "Enquadramento", candidatura.PropostaId);
            return View(candidatura);
        }

        // GET: Candidaturas/Delete/5
        public ActionResult Delete(int? id)
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
            return View(candidatura);
        }

        // POST: Candidaturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Candidatura candidatura = db.Candidaturas.Find(id);
            db.Candidaturas.Remove(candidatura);
            db.SaveChanges();
            return RedirectToAction("MinhasCandidaturas");
        }

        public ActionResult CandidaturasPorProposta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Alunos = db.Alunos.ToList();
            Proposta p = db.Propostas.SingleOrDefault(u => u.PropostaId == id);
            return View(db.Candidaturas.Where(u=>u.PropostaId == p.PropostaId));
        }

        public ActionResult CandidaturasRecebidas()
        {
            ViewBag.Alunos = db.Alunos.ToList();
            var user = User.Identity.GetUserId();


            if (User.IsInRole("Comissao") || User.IsInRole(Role.Docentes.ToString()))
            {
                Docente t = db.Docentes.SingleOrDefault(u => u.UserId == user);
                if (t != null)
                {
                    return View(db.Candidaturas.Where(u=>u.Proposta.Docentes.Any(x=>x.DocenteId == t.DocenteId) != false && u.Proposta.CandidaturaId == null));
                }
            }
            else if (User.IsInRole(Role.Empresas.ToString()))
            {
                Empresa c = db.Empresas.SingleOrDefault(u => u.UserId == user);
                if (c != null)
                {
                    return View(db.Candidaturas.Where(u => u.Proposta.EmpresaId == c.EmpresaId && u.Proposta.CandidaturaId == null));
                }
            }

            return View();
        }

        public ActionResult MinhasCandidaturas()
        {
            var user = User.Identity.GetUserId();

            Aluno a = db.Alunos.SingleOrDefault(u => u.UserId == user);
            if (a != null)
            {
                return View(db.Candidaturas.Where(u => u.Aluno.UserId == user).ToList());
            }

            return View();
        }

        public ActionResult CandidaturasAprovadas()
        {
            var user = User.Identity.GetUserId();
            var c = db.Candidaturas.Where(u => u.Aluno.UserId == user && u.CandidaturaAceite == true && u.CandidaturaRejeitada == false);
            if (c != null)
            {
                return View(c);
            }
            return View();
        }

        public ActionResult CandidaturasReprovadas()
        {
            var user = User.Identity.GetUserId();
            var c = db.Candidaturas.Where(u => u.Aluno.UserId == user && u.CandidaturaAceite == false && u.CandidaturaRejeitada == true);
            if (c != null)
            {
                return View(c);
            }
            return View();
        }

        public ActionResult CandidaturasAluno(int id)
        {
            Aluno a = db.Alunos.SingleOrDefault(x => x.NumeroAluno == id);
            ViewBag.Propostas = db.Propostas.ToList();
            return View(db.Candidaturas.Where(x=>x.AlunoId == a.AlunoId).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
