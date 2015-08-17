using TooDooSvc.Persistence;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using TooDooWebRole.Models;
using Microsoft.Practices.Unity;

namespace TooDooWebRole.Controllers
{
     [Authorize]
    public class DashboardController : Controller
    {
        private readonly TooDooManagement manager = null;
        private readonly AccountManagement accmanager = null;

        private IPhotoService photoService = null;

        
        public DashboardController(TooDooManagement toodooManager, IPhotoService photoStore)
        {
            manager = new TooDooManagement();
            accmanager = new AccountManagement();
            photoService = photoStore;
        }

        public DashboardController(IPhotoService photoStore)
        {
            manager = new TooDooManagement();
            accmanager = new AccountManagement();
            photoService = photoStore;
        }

        public DashboardController() 
        { 
            manager = new TooDooManagement();
            accmanager = new AccountManagement();
            photoService = new TooDooSvc.Persistence.PhotoService(new TooDooSvc.Logging.Logger()); 
        }

        //
        // GET: /Dashboard/
        public async Task<ActionResult> Index()
        {
            string currentUser = User.Identity.Name;
            Session["HasNewFriend"] = await accmanager.HasNewFriend(currentUser);

            var result = await manager.FindOpenToodoosByOwnerAsync(currentUser);

            // Notification about new TooDoos goes down here
            UserProfile up = await accmanager.FindUserByNameAsync(currentUser);
            up.HasNewTooDoo = false;
            await accmanager.UpdateAsync(up);
            if (Session["HasNewTooDoo"] != null) Session["HasNewTooDoo"] = null;

            return View(result);
        }

        //
        // GET: /Dashboard/Details/5
        public async Task<ActionResult> Details(int id)
        {
            TooDooEntry toodoo = await manager.FindTooDooByIdAsync(id);

            if (toodoo == null)
            {
                return HttpNotFound();
            }

            return View(toodoo);
        }

        //
        // GET: /Dashboard/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            TooDooEntry toodoo = await manager.FindTooDooByIdAsync(id);
            if (toodoo == null)
            {
                return HttpNotFound();
            }

            // Verify logged in user owns this FixIt task.
            if (User.Identity.Name != toodoo.Owner)
            {
               return HttpNotFound();
            }

            return View(toodoo);
        }

        //
        // POST: /Dashboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind(Include = "CreatedBy,Owner,Title,Notes,PhotoUrl,IsDone,CreatedDate,LastModifiedDate,Report,ReportPhotoUrl")]FormCollection form, HttpPostedFileBase photo)
        {
            TooDooEntry toodoo = await manager.FindTooDooByIdAsync(id);

            // Verify logged in user owns this FixIt task.
            if (User.Identity.Name != toodoo.Owner)
            {
               return HttpNotFound();
            }

            toodoo.ReportPhotoUrl = await photoService.UploadPhotoAsync(photo);

            if (TryUpdateModel(toodoo, form))
            {
                await manager.UpdateAsync(toodoo);
                return RedirectToAction("Index");
            }

            return View(toodoo);
        }
    }
}
