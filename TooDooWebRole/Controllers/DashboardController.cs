using TooDooSvc.Persistence;
using System.Threading.Tasks;
using System.Web.Mvc;

using TooDooWebRole.Models;

namespace TooDooWebRole.Controllers
{
     [Authorize]
    public class DashboardController : Controller
    {
        private readonly TooDooManagement manager = null;

        public DashboardController() { manager = new TooDooManagement(); }
        public DashboardController(TooDooManagement toodooManager)
        {
            manager = toodooManager;
        }

        //
        // GET: /Dashboard/
        public async Task<ActionResult> Index()
        {
            string currentUser = User.Identity.Name;
            var result = await manager.FindOpenToodoosByOwnerAsync(currentUser);

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
        public async Task<ActionResult> Edit(int id, [Bind(Include = "CreatedBy,Owner,Title,Notes,PhotoUrl,IsDone,CreatedDate,LastModifiedDate")]FormCollection form)
        {
            TooDooEntry toodoo = await manager.FindTooDooByIdAsync(id);

            // Verify logged in user owns this FixIt task.
            if (User.Identity.Name != toodoo.Owner)
            {
               return HttpNotFound();
            }

            if (TryUpdateModel(toodoo, form))
            {
                await manager.UpdateAsync(toodoo);
                return RedirectToAction("Index");
            }

            return View(toodoo);
        }
    }
}
