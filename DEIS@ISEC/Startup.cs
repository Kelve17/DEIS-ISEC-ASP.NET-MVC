using System;
using DEIS_ESTAGIOS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DEIS_ESTAGIOS.Startup))]
namespace DEIS_ESTAGIOS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CriarPerfis();
        }

        private void CriarPerfis()
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            //role manager para manipular perfis
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //user manager para manipular users


            if (!roleManager.RoleExists("Empresas"))
            {
                var role = new IdentityRole { Name = "Empresas" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Docentes"))
            {
                var role = new IdentityRole { Name = "Docentes" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Alunos"))
            {
                var role = new IdentityRole { Name = "Alunos" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole { Name = "Admin" };
                roleManager.Create(role);
                var user = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "Admin@hotmail.com"
                };

                string userPWD = @"\correias17";
                var chkUser = UserManager.Create(user, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
                context.Users.Add(user);
            }
        }
    }
}
