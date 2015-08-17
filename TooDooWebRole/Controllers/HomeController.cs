using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using TooDooWebRole.Models;

namespace TooDooWebRole.Controllers
{
    public class HomeController : Controller
    {
        private readonly AccountManagement accmanager = null;

        public HomeController()
        {
            accmanager = new AccountManagement();
        }

        public async Task<ActionResult> Index()
        {
            string currentUser = User.Identity.Name;

            ViewBag.Message = "Create and manage TooDoos";
            Session["HasNewFriend"] = await accmanager.HasNewFriend(currentUser);
            Session["HasNewTooDoo"] = await accmanager.HasNewTooDoo(currentUser);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
