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
    public class MensagensController : Controller
    {
        private Deis_EstagiosContext db;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public MensagensController()
        {
            db = new Deis_EstagiosContext();
        }

        public MensagensController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: Mensagens
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult TodasAsMensagens()
        {
            var vm = new ViewModels
            {
                Mensagens = db.Mensagens.ToList(),
                Alunos = db.Alunos.ToList(),
                Empresas = db.Empresas.ToList(),
                Docentes = db.Docentes.ToList()
            };
            return View(vm);
        }

        // GET: Mensagens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagens.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            return View(mensagem);
        }

        // GET: Mensagens/Create
        public ActionResult Create(int id)
        {
            Mensagem m = new Mensagem();
            if (User.IsInRole("Alunos"))
            {
                Proposta p = db.Propostas.SingleOrDefault(x => x.PropostaId == id);
                m.RemetenteId = User.Identity.GetUserId();
                if (p.EmpresaId != null)
                {
                    m.DestinarioId = db.Empresas.SingleOrDefault(x => x.EmpresaId == p.EmpresaId).UserId;
                }
                else
                {
                    m.DestinarioId = db.Docentes.SingleOrDefault(x => x.Propostas.Any(y => y.PropostaId == p.PropostaId) == true).UserId;
                }
            }

            if(User.IsInRole("Docentes") || User.IsInRole("Comissao")|| User.IsInRole("Empresas"))
            {
                Mensagem msg = db.Mensagens.Find(id);
                m.DestinarioId = msg.RemetenteId;
                m.RemetenteId = msg.DestinarioId;
            }
            return View(m);
        }

        // POST: Mensagens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MensagemId,RemetenteId,DestinarioId,Assunto,Conteudo,StatusMensagem,Data")] Mensagem mensagem)
        {
            if (ModelState.IsValid)
            {
                db.Mensagens.Add(mensagem);
                db.SaveChanges();
                return RedirectToAction("MensagensEnviadas");
            }

            return View(mensagem);
        }

        // GET: Mensagens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagens.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            return View(mensagem);
        }

        // POST: Mensagens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MensagemId,Remetente,Destinario,Assunto,Conteudo,StatusMensagem,Data")] Mensagem mensagem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mensagem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mensagem);
        }

        // GET: Mensagens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagens.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            return View(mensagem);
        }

        // POST: Mensagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mensagem mensagem = db.Mensagens.Find(id);
            db.Mensagens.Remove(mensagem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult MensagensRecebidas()
        {
            var user = User.Identity.GetUserId();

            ViewModels vm = new ViewModels
            {   Alunos = db.Alunos.ToList(),
                Docentes = db.Docentes.ToList(),
                Empresas = db.Empresas.ToList(),
                Mensagens = db.Mensagens.Where(x => x.DestinarioId == user).OrderByDescending(x => x.Data).ToList()

            };

            return View(vm);
        }

        public ActionResult MensagensEnviadas()
        {
            var user = User.Identity.GetUserId();

            ViewModels vm = new ViewModels
            {
                Alunos = db.Alunos.ToList(),
                Docentes = db.Docentes.ToList(),
                Empresas = db.Empresas.ToList(),
                Mensagens = db.Mensagens.Where(x => x.RemetenteId == user).OrderByDescending(x=>x.Data).ToList()

            };

            return View(vm);
        }

        public ActionResult LerMensagem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagens.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Alunos"))
            {
                if(db.Docentes.Any(x=>x.UserId == mensagem.RemetenteId))
                {
                    ViewBag.Remetente = db.Docentes.SingleOrDefault(x => x.UserId == mensagem.RemetenteId).Nome+" "+ db.Docentes.SingleOrDefault(x => x.UserId == mensagem.RemetenteId).Apelido;
                }
                if(db.Empresas.Any(x=>x.UserId == mensagem.RemetenteId))
                {
                    ViewBag.Remetente = db.Empresas.SingleOrDefault(x => x.UserId == mensagem.RemetenteId).Nome;
                }
            }

            if(User.IsInRole("Comissao") || User.IsInRole("Docentes") || User.IsInRole("Empresas"))
            {

                ViewBag.Remetente = db.Alunos.SingleOrDefault(x => x.UserId == mensagem.RemetenteId).Nome+" "+ db.Alunos.SingleOrDefault(x => x.UserId == mensagem.RemetenteId).Apelido;
            }
            mensagem.StatusMensagem = statusMensagem.Entregue;
            db.SaveChanges();
            return View(mensagem);
        }


        public ActionResult LerMensagem2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mensagem mensagem = db.Mensagens.Find(id);
            if (mensagem == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Alunos"))
            {
                if (db.Docentes.Any(x => x.UserId == mensagem.DestinarioId))
                {
                    ViewBag.Remetente = db.Docentes.SingleOrDefault(x => x.UserId == mensagem.DestinarioId).Nome + " " + db.Docentes.SingleOrDefault(x => x.UserId == mensagem.DestinarioId).Apelido;
                }
                if (db.Empresas.Any(x => x.UserId == mensagem.DestinarioId))
                {
                    ViewBag.Remetente = db.Empresas.SingleOrDefault(x => x.UserId == mensagem.DestinarioId).Nome;
                }
            }

            if (User.IsInRole("Comissao") || User.IsInRole("Docentes") || User.IsInRole("Empresas"))
            {

                ViewBag.Remetente = db.Alunos.SingleOrDefault(x => x.UserId == mensagem.DestinarioId).Nome + " " + db.Alunos.SingleOrDefault(x => x.UserId == mensagem.DestinarioId).Apelido;
            }
            mensagem.StatusMensagem = statusMensagem.Entregue;
            db.SaveChanges();
            return View(mensagem);
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
