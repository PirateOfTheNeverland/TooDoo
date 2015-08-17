using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TooDooSvc.Persistence;
using TooDooWebRole.Models;

namespace TooDooWebRole.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly FriendManagement manager = null;
        private readonly AccountManagement accmanager = null;

        public FriendsController(FriendManagement friendManager)
        {
            manager = friendManager;
        }

        public FriendsController()
        {
            manager = new FriendManagement();
            accmanager = new AccountManagement();
        }

        //
        // GET: /Friends/

        public async Task<ActionResult> Index()
        {
            string currentUser = User.Identity.Name;
            Session["HasNewTooDoo"] = await accmanager.HasNewTooDoo(currentUser);

            List<FriendEntry> items = new List<FriendEntry>();

            using (FriendContext db = new FriendContext())
            {
                int i = 0;
                //object[] parameters = new object[] { new SqlParameter("longitude", longitude), new SqlParameter("longitude", longitude) };
                var friends = db.FriendEntries.SqlQuery("SELECT * FROM dbo.FriendEntries WHERE Owner = @p0 AND IsDeleted = 'false'", currentUser );
                //list = friends;

                //.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                // Check if user already exists
                foreach (FriendEntry entry in friends)
                {
                    items.Add(new FriendEntry { Name = entry.Name, Notes = entry.Notes, 
                                                FriendId = entry.FriendId, 
                                                Owner = entry.Owner, 
                                                IsBlocked = entry.IsBlocked,
                                                IsDeleted = entry.IsDeleted});
                    i++;
                }

                ViewData["FriendList"] = items;
                return View(items);

            }
        }

        //
        // GET: /Friends/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        public async Task<ActionResult> Notification(int id)
        {
            Session["HasNewFriend"] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Notification([Bind(Include = "FriendId,Owner,Name,Notes,IsDeleted,IsBlocked")]FriendEntry form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    form.Owner = User.Identity.Name;

                    using (FriendContext db = new FriendContext())
                    using (UsersContext dbUsers = new UsersContext())
                    {
                        string sql = "SELECT * FROM dbo.FriendEntries WHERE Name = @p0 AND Owner = @p1";
                        var UserExists = dbUsers.UserProfiles.SqlQuery("SELECT * FROM dbo.UserProfile WHERE UserName = @p0", form.Name);
                        var FriendExists = db.FriendEntries.SqlQuery(sql,
                            new object[] { new SqlParameter("p0", form.Name), 
                                           new SqlParameter("p1", form.Owner)});
                        int i = UserExists.Count();
                        int j = FriendExists.Count();

                        if (i > 0) // If User with specified name exists...
                        {
                            if (j > 0) // If there's a form entry for this user
                            {
                                FriendEntry fr = await manager.FindFriendByNameAndOwnerAsync(form.Name, form.Owner);

                                fr.IsDeleted = false;
                                fr.IsBlocked = false;
                                fr.Notes = form.Notes;
                                await manager.UpdateAsync(fr);

                                // update form entry
                                //string sql1 = "UPDATE dbo.FriendEntries SET IsDeleted='false', Notes=@p0 WHERE Name=@p1 AND Owner=@p2";
                                //var friends = db.Database.ExecuteSqlCommand(sql1,
                                //    new object[] { new SqlParameter("p0", form.Notes),
                                //                   new SqlParameter("p1", form.Name), 
                                //                   new SqlParameter("p2", form.Owner)});
                            }
                            else // create a new form entry
                            {
                                //ToDo: add notification for user that he has been added
                                await manager.CreateAsync(form);

                                //Notify user about new friend
                                UserProfile up = await accmanager.FindUserByNameAsync(form.Name);

                                up.HasNewFriend = true;
                                await accmanager.UpdateAsync(up);
                            }
                        }
                        else // return. No such user
                        {
                            ViewBag.Owner = form.Owner;
                            return View();
                        }
                    }

                    return RedirectToAction("Index");
                }
                ViewBag.Owner = form.Owner;
                return View();
            }
            catch
            {
                ViewBag.Owner = form.Owner;
                return View();
            }
        }

        //
        // GET: /Friends/Create

        public ActionResult Create()
        {
            string owner = User.Identity.Name;
            ViewBag.Owner = owner;
            return View();
        }

        //
        // POST: /Friends/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FriendId,Owner,Name,Notes,IsDeleted,IsBlocked")]FriendEntry form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    form.Owner = User.Identity.Name;

                    using (FriendContext db = new FriendContext())
                    using (UsersContext dbUsers = new UsersContext())
                    {
                        string sql = "SELECT * FROM dbo.FriendEntries WHERE Name = @p0 AND Owner = @p1";
                        var UserExists = dbUsers.UserProfiles.SqlQuery("SELECT * FROM dbo.UserProfile WHERE UserName = @p0", form.Name);
                        var FriendExists = db.FriendEntries.SqlQuery(sql, 
                            new object[] { new SqlParameter("p0", form.Name), 
                                           new SqlParameter("p1", form.Owner)});
                        int i = UserExists.Count();
                        int j = FriendExists.Count();

                        if (i > 0) // If User with specified name exists...
                        {
                            if (j > 0) // If there's a form entry for this user
                            {
                                FriendEntry fr = await manager.FindFriendByNameAndOwnerAsync(form.Name, form.Owner);

                                fr.IsDeleted = false;
                                fr.IsBlocked = false;
                                fr.Notes = form.Notes;
                                await manager.UpdateAsync(fr);

                                // update form entry
                                //string sql1 = "UPDATE dbo.FriendEntries SET IsDeleted='false', Notes=@p0 WHERE Name=@p1 AND Owner=@p2";
                                //var friends = db.Database.ExecuteSqlCommand(sql1,
                                //    new object[] { new SqlParameter("p0", form.Notes),
                                //                   new SqlParameter("p1", form.Name), 
                                //                   new SqlParameter("p2", form.Owner)});
                            }
                            else // create a new form entry
                            {
                                //ToDo: add notification for user that he has been added
                                await manager.CreateAsync(form);

                                //Notify user about new friend
                                UserProfile up = await accmanager.FindUserByNameAsync(form.Name);

                                up.HasNewFriend = true;
                                await accmanager.UpdateAsync(up);
                            }
                        }
                        else // return. No such user
                        {
                            ViewBag.Owner = form.Owner;
                            return View();
                        }
                    }

                    return RedirectToAction("Index");
                }
                ViewBag.Owner = form.Owner;
                return View();
            }
            catch
            {
                ViewBag.Owner = form.Owner;
                return View();
            }
        }

        //
        // GET: /Friends/Edit/5

        public async Task<ActionResult> Edit(int id)
        {
            FriendEntry friend = await manager.FindFriendByIdAsync(id);
            if (friend == null)
            {
                return HttpNotFound();
            }

            // Verify logged in user owns this FixIt task.
            if (User.Identity.Name != friend.Owner)
            {
                return HttpNotFound();
            }

            return View(friend);
        }

        //
        // POST: /Friends/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind(Include = "FriendId,Owner,Name,Notes,IsDeleted,IsBlocked")]FormCollection collection)
        {
            FriendEntry friend = await manager.FindFriendByIdAsync(id);

            // Verify logged in user owns this FixIt task.
            if (User.Identity.Name != friend.Owner)
            {
                return HttpNotFound();
            }

            if (TryUpdateModel(friend, collection))
            {
                await manager.UpdateAsync(friend);
                return RedirectToAction("Index");
            }

            return View(friend);
        }

        //
        // GET: /Friends/Delete/5

        public ActionResult Delete(int id)
        {
            string Owner = User.Identity.Name;

            using (FriendContext db = new FriendContext())
            {
                var friends = db.Database.ExecuteSqlCommand("UPDATE dbo.FriendEntries SET IsDeleted='true' WHERE FriendId=@p0", id);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Block(int id)
        {
            string Owner = User.Identity.Name;

            using (FriendContext db = new FriendContext())
            {
                var friends = db.Database.ExecuteSqlCommand("UPDATE dbo.FriendEntries SET IsBlocked='true' WHERE FriendId=@p0", id);

            }
            return RedirectToAction("Index");
        }

        public ActionResult Unblock(int id)
        {
            string Owner = User.Identity.Name;

            using (FriendContext db = new FriendContext())
            {
                var friends = db.Database.ExecuteSqlCommand("UPDATE dbo.FriendEntries SET IsBlocked='false' WHERE FriendId=@p0", id);

            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Friends/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
