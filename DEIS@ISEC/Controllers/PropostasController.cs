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
    [Authorize(Roles = "Docentes,Empresas,Admin")]
    public class PropostasController : Controller
    {
        private Deis_EstagiosContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public PropostasController()
        {
            db = new Deis_EstagiosContext();
        }

        public PropostasController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: Propostas
        public ActionResult Index()
        {
            var propostas = db.Propostas.Include(p => p.CandidaturaAceite).Include(p => p.Empresa);
            return View(propostas.ToList());
        }

        // GET: Propostas/Details/5
        //[Authorize(Roles = "Admin,Alunos,Docentes,Comissao,Empresas")]
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }
            return View(proposta);
        }

        // GET: Propostas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Propostas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PropostaId,Titulo,EmpresaId,CandidaturaId,Ramo,Tipo,Enquadramento,Objectivos,CondicoesAcesso,Localizacao,DataInicio,DataFim,AvaliacaoAlunoEmpresa,AvaliacaoEmpresaALuno,AvaliacaoDocenteAluno")] Proposta proposta)
        {
            if (ModelState.IsValid)
            {
                var user = User.Identity.GetUserId();

                if (User.IsInRole("Empresas"))
                {
                    var idEmpresa = db.Empresas.SingleOrDefault(u => u.UserId == user).EmpresaId;
                    proposta.EmpresaId = idEmpresa;
                    db.Empresas.SingleOrDefault(u => u.UserId == user).Propostas.Add(proposta);
                    IList<Proposta> aux = db.Empresas.SingleOrDefault(u => u.UserId == user).Propostas;
                    aux.Add(proposta);
                    aux.Add(proposta);

                }

                if (User.IsInRole("Docentes") || User.IsInRole("Comissao"))
                {
                    Docente d = db.Docentes.SingleOrDefault(u => u.UserId == user);
                    proposta.Docentes.Add(d);
                    db.Docentes.SingleOrDefault(u => u.UserId == user).Propostas.Add(proposta);
                }
                db.Propostas.Add(proposta);
                db.SaveChanges();
                return RedirectToAction("MinhasPropostas");
            }

            return View(proposta);
        }

        // GET: Propostas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }

            return View(proposta);
        }

        // POST: Propostas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PropostaId,Titulo,EmpresaId,CandidaturaId,Ramo,Tipo,Enquadramento,Objectivos,CondicoesAcesso,Localizacao,DataInicio,DataFim,AvaliacaoAlunoEmpresa,AvaliacaoEmpresaALuno,AvaliacaoDocenteAluno")] Proposta proposta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proposta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MinhasPropostas");
            }

            return View(proposta);
        }

        // GET: Propostas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }
            return View(proposta);
        }

        // POST: Propostas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Proposta proposta = db.Propostas.Find(id);
            db.Propostas.Remove(proposta);
            db.SaveChanges();
            return RedirectToAction("MinhasPropostas");
        }

        public ActionResult MinhasPropostas()
        {
            var user = User.Identity.GetUserId();

            if (User.IsInRole("Comissao") || User.IsInRole(Role.Docentes.ToString()))
            {
                Docente t = db.Docentes.SingleOrDefault(u => u.UserId == user);
                if (t != null)
                {
                    return View(db.Propostas.Where(u => u.Docentes.Any(x => x.DocenteId == t.DocenteId) != false && u.CandidaturaId == null));
                }
            }
            else if (User.IsInRole(Role.Empresas.ToString()))
            {
                Empresa c = db.Empresas.SingleOrDefault(u => u.UserId == user);
                if (c != null)
                {
                    return View(db.Propostas.Where(u => u.EmpresaId == c.EmpresaId && u.CandidaturaId == null));
                }
            }

            return View();
        }


        public ActionResult AntigasPropostas()
        {
            var user = User.Identity.GetUserId();

            if (User.IsInRole("Comissao") || User.IsInRole(Role.Docentes.ToString()))
            {
                Docente t = db.Docentes.SingleOrDefault(u => u.UserId == user);
                if (t != null)
                {
                    return View(db.Propostas.Where(u => u.Docentes.Any(x => x.DocenteId == t.DocenteId) != false && u.CandidaturaId != null));
                }
            }
            else if (User.IsInRole(Role.Empresas.ToString()))
            {
                Empresa c = db.Empresas.SingleOrDefault(u => u.UserId == user);
                if (c != null)
                {
                    return View(db.Propostas.Where(u => u.EmpresaId == c.EmpresaId && u.CandidaturaId != null));
                }
            }

            return View();
        }

        public ActionResult MinhasPropostas2()
        {
            var user = User.Identity.GetUserId();

            if (User.IsInRole("Comissao") || User.IsInRole(Role.Docentes.ToString()))
            {
                Docente t = db.Docentes.SingleOrDefault(u => u.UserId == user);
                if (t != null)
                {
                    return View(db.Propostas.Where(u => u.Docentes.Any(x => x.DocenteId == t.DocenteId) != false && u.CandidaturaId == null));
                }
            }
            else if (User.IsInRole(Role.Empresas.ToString()))
            {
                Empresa c = db.Empresas.SingleOrDefault(u => u.UserId == user);
                if (c != null)
                {
                    return View(db.Propostas.Where(u => u.EmpresaId == c.EmpresaId && u.CandidaturaId == null));
                }
            }

            return View();
        }

        public ActionResult PropostasDocentes()
        {
            var user = User.Identity.GetUserId();
            Docente t = db.Docentes.SingleOrDefault(x => x.UserId == user);
            if (t != null)
            {
                return View(db.Propostas.Where(u => u.Docentes.Any(x => x.DocenteId == t.DocenteId) == false && u.CandidaturaId == null && u.Tipo == TipoProposta.Projecto));
            }
            return View();
        }

        public ActionResult AdicionarDocenteProposta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta p = db.Propostas.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }
            return View(p);
        }

        public ActionResult ConfirmarAdicaoProposta(int id)
        {
            var user = User.Identity.GetUserId();
            Proposta p = db.Propostas.Find(id);
            Docente d = db.Docentes.SingleOrDefault(x => x.UserId == user);
            p.Docentes.Add(d);
            d.Propostas.Add(p);
            db.SaveChanges();
            return RedirectToAction("MinhasPropostas");
        }
        [Authorize(Roles = "Docentes")]
        public ActionResult PropostasOrientadas()
        {
            var user = User.Identity.GetUserId();
            Docente d = db.Docentes.SingleOrDefault(x => x.UserId == user);
            var vm = new ViewModels
            {
                Alunos = db.Alunos.ToList(),
                Candidaturas = db.Candidaturas.ToList(),
                Docentes = db.Docentes.ToList(),
                Empresas = db.Empresas.ToList(),
                Propostas = db.Propostas.Where(u => u.DocenteId == d.DocenteId && u.AvaliacaoDocenteAluno == null).ToList()
            };
            return View(vm);
        }

        [AllowAnonymous]
        public ActionResult PropostasRoleGeral(int id)
        {
            if (id == 1)
            {
                return View(db.Propostas.Where(x => x.Tipo == TipoProposta.Estágio && x.DataInicio.Year == DateTime.Now.Year).OrderBy(y => y.Localizacao).ToList());
            }
            else if (id == 2)
            {
                return View(db.Propostas.Where(x => x.Tipo == TipoProposta.Projecto && x.DataInicio.Year == DateTime.Now.Year).OrderBy(y => y.Localizacao).ToList());

            }
            else if (id == 3)
            {
                return View(db.Propostas.Where(x => x.Tipo == TipoProposta.Estágio && x.DataInicio.Year == DateTime.Now.Year).OrderBy(y => y.Ramo).ToList());

            }
            else if (id == 4)
            {
                return View(db.Propostas.Where(x => x.Tipo == TipoProposta.Projecto && x.DataInicio.Year == DateTime.Now.Year).OrderBy(y => y.Ramo).ToList());

            }
            return View();
        }

        [Authorize(Roles = "Admin,Comissao")]
        public ActionResult DefinirOrientadorEstagio()
        {
            var vm = new ViewModels
            {
                Empresas = db.Empresas.ToList(),
                Alunos = db.Alunos.ToList(),
                Candidaturas = db.Candidaturas.ToList(),
                Propostas = db.Propostas.Where(x => x.CandidaturaId != null && x.DocenteId == null && x.Tipo == TipoProposta.Estágio).ToList()
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Comissao")]
        public ActionResult SeleccionaOrientador(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }
            ViewBag.NomeEmpresa = db.Empresas.SingleOrDefault(x => x.EmpresaId == proposta.EmpresaId).Nome;
            ViewBag.Docentes = db.Docentes.Select(x => x.Email).ToList();

            return View(proposta);
        }

        [Authorize(Roles = "Admin,Comissao")]
        [HttpPost]
        public ActionResult SeleccionaOrientador(Proposta p)
        {
            Proposta proposta = db.Propostas.Find(p.PropostaId);
            Docente docente = db.Docentes.SingleOrDefault(x => x.Email == p.Orientador.Email);
            proposta.DocenteId = docente.DocenteId;
            db.Entry(proposta).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DefinirOrientadorEstagio");
        }


        [Authorize(Roles = "Admin,Comissao")]
        public ActionResult DefinirDataDefesa()
        {
            var vm = new ViewModels
            {
                Empresas = db.Empresas.ToList(),
                Alunos = db.Alunos.ToList(),
                Candidaturas = db.Candidaturas.ToList(),
                Propostas = db.Propostas.Where(x => x.CandidaturaId != null && x.DataDefesa == null && x.Tipo == TipoProposta.Estágio).ToList()
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Comissao")]
        public ActionResult SeleccionaDataDefesa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proposta proposta = db.Propostas.Find(id);
            if (proposta == null)
            {
                return HttpNotFound();
            }
            ViewBag.NomeEmpresa = db.Empresas.SingleOrDefault(x => x.EmpresaId == proposta.EmpresaId).Nome;

            return View(proposta);
        }

        [Authorize(Roles = "Admin,Comissao")]
        [HttpPost]
        public ActionResult SeleccionaDataDefesa(Proposta p)
        {
            Proposta proposta = db.Propostas.Find(p.PropostaId);
            proposta.DataDefesa = p.DataDefesa;
            db.Entry(proposta).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("DefinirDataDefesa");
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
