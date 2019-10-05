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
using DEIS_ISEC.SupportModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace DEIS_ISEC.Controllers
{
    [Authorize(Roles= ("Docentes,Admin"))]
    public class DocentesController : Controller
    {
        private Deis_EstagiosContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public DocentesController()
        {
            db = new Deis_EstagiosContext();
        }

        public DocentesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Docentes
        public ActionResult Index()
        {
            return View(db.Docentes.ToList());
        }

        // GET: Docentes/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            return View(docente);
        }

        // GET: Docentes/Create
        [Authorize(Roles = "Admin,Docentes")]
        public ActionResult Create()
        {
            var user = User.Identity.GetUserId();
            Docente s = new Docente();
            s.Email = db.Users.SingleOrDefault(u => u.Id == user.ToString()).Email;
            return View(s);
        }

        // POST: Docentes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Docentes")]
        public ActionResult Create([Bind(Include = "DocenteId,Nome,Apelido,Email,Telefone,PertenceComissao,Morada,UserId")] Docente docente)
        {
            if (ModelState.IsValid)
            {
                db.Docentes.Add(docente);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(docente);
        }

        // GET: Docentes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            return View(docente);
        }

        // POST: Docentes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocenteId,CodigoDescricao,Nome,Apelido,Telefone,PertenceComissao,Morada")] Docente docente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(docente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(docente);
        }

        // GET: Docentes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            return View(docente);
        }

        // POST: Docentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Docente docente = db.Docentes.Find(id);
            db.Docentes.Remove(docente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Docentes,Empresas")]
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
            ViewBag.id = candidatura.CandidaturaId;
            return View(candidatura);
        }

        [Authorize(Roles = "Empresas,Docentes")]
        public ActionResult ConfirmarCandidatura(int id)
        {
            var user = User.Identity.GetUserId();
            Docente docente = db.Docentes.SingleOrDefault(x => x.UserId == user);
            Candidatura candidatura = db.Candidaturas.Find(id);
            db.Propostas.SingleOrDefault(x => x.PropostaId == candidatura.PropostaId).CandidaturaAceite = candidatura;
            db.Propostas.SingleOrDefault(x => x.PropostaId == candidatura.PropostaId).CandidaturaId = candidatura.CandidaturaId;
            db.Propostas.SingleOrDefault(x => x.PropostaId == candidatura.PropostaId).DocenteId = docente.DocenteId;
            db.Propostas.SingleOrDefault(x => x.PropostaId == candidatura.PropostaId).Orientador = docente;
            db.Docentes.SingleOrDefault(x => x.UserId == user).PropostasOrientadas.Add(db.Propostas.SingleOrDefault(x => x.PropostaId == candidatura.PropostaId));
            db.Candidaturas.Find(id).CandidaturaAceite = true;
            var candidaturas = db.Candidaturas.Where(x => x.PropostaId == candidatura.PropostaId).ToList();
            foreach (Candidatura c in candidaturas)
            {
                db.Candidaturas.SingleOrDefault(u => u.CandidaturaId == c.CandidaturaId).CandidaturaRejeitada = true;
            }
            db.SaveChanges();
            return RedirectToAction("CandidaturasRecebidas", "Candidaturas");
        }

        [Authorize(Roles="Admin")]
        public ActionResult DefinirComissao()
        {
            return View(db.Docentes.ToList());
        }


        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmaComissao(int? id)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
             Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            var roleComissao = db.Roles.SingleOrDefault(x => x.Name == "Comissao").Id;
            userManager.AddToRole(docente.UserId, "Comissao");
            docente.PertenceComissao = true;
            db.Entry(docente).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ListaComissao");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ListaComissao()
        {
            return View(db.Docentes.Where(x => x.PertenceComissao == true).ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveComissao(int? id)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docente docente = db.Docentes.Find(id);
            if (docente == null)
            {
                return HttpNotFound();
            }
            var roleComissao = db.Roles.SingleOrDefault(x => x.Name == "Docentes").Id;
            docente.PertenceComissao = false;
            userManager.RemoveFromRole(docente.UserId, "Comissao");
            db.Entry(docente).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ListaComissao");
        }

       [Authorize(Roles ="Admin,Comissao")]
       public ActionResult Estatisticas()
        {
            Estatisticas est = new Estatisticas
            {
                TotalEstagios = db.Propostas.Where(x => x.Tipo == TipoProposta.Estágio /*&& x.DataInicio.Year == 2018*/).Count(),
                TotalProjetos = db.Propostas.Where(x => x.Tipo == TipoProposta.Projecto /*&& x.DataInicio.Year == 2018*/).Count(),
                TotalCandidaturas = db.Candidaturas/*.Where(x => x.Proposta.DataInicio.Year == 2018)*/.Count(),
                TotalEmpresas = db.Empresas.Count(),
                TotalAlunos = db.Alunos.Count(),
                TotalDocentes = db.Docentes.Count(),
                EmpresasMaisProcuradas = db.Empresas.OrderByDescending(x => x.Candidaturas.Count()).Select(y => y.Nome).ToList(),
                EmpresaComMaisPropostas = db.Empresas.OrderByDescending(x => x.Propostas.Count()).First().Nome+"-"+ db.Empresas.OrderByDescending(x => x.Propostas.Count()).First().Propostas.Count(),
                AlunoComMaisCandidaturas = db.Alunos.OrderByDescending(x => x.Candidaturas.Count()).First().Nome + " " + db.Alunos.OrderByDescending(x => x.Candidaturas.Count()).First().Apelido+"-" + db.Alunos.OrderByDescending(x => x.Candidaturas.Count()).First().Candidaturas.Count(),
                DocentesComMaisPropostas = db.Docentes.OrderByDescending(x => x.Propostas.Count()).First().Nome + " " + db.Docentes.OrderByDescending(x => x.Propostas.Count()).First().Apelido + "-" + db.Docentes.OrderByDescending(x => x.Propostas.Count()).First().Propostas.Count
            };

            return View(est);
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
