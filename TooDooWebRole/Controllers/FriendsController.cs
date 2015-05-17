using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TooDooSvc.Persistence;
using TooDooWebRole.Models;

namespace TooDooWebRole.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private IFixItTaskRepository fixItRepository = null;

        public FriendsController(IFixItTaskRepository repository)
        {
            fixItRepository = repository;
        }

        //
        // GET: /Friends/

        public ActionResult Index()
        {
            string currentUser = User.Identity.Name;
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
        public ActionResult Create([Bind(Include = "FriendId,Owner,Name,Notes,IsDeleted,IsBlocked")]FriendEntry friend)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    friend.Owner = User.Identity.Name;

                    using (FriendContext db = new FriendContext())
                    using (UsersContext dbUsers = new UsersContext())
                    {
                        string sql = "SELECT * FROM dbo.FriendEntries WHERE Name = @p0 AND Owner = @p1";
                        var UserExists = dbUsers.UserProfiles.SqlQuery("SELECT * FROM dbo.UserProfile WHERE UserName = @p0", friend.Name);
                        var FriendExists = db.FriendEntries.SqlQuery(sql, 
                            new object[] { new SqlParameter("p0", friend.Name), 
                                           new SqlParameter("p1", friend.Owner)});
                        int i = UserExists.Count();
                        int j = FriendExists.Count();

                        if (i > 0) // If User with specified name exists...
                        {
                            if (j > 0) // If there's a friend entry for this user
                            {
                                // update friend entry
                                string sql1 = "UPDATE dbo.FriendEntries SET IsDeleted='false', Notes=@p0 WHERE Name=@p1 AND Owner=@p2";
                                var friends = db.Database.ExecuteSqlCommand(sql1,
                                    new object[] { new SqlParameter("p0", friend.Notes),
                                                   new SqlParameter("p1", friend.Name), 
                                                   new SqlParameter("p2", friend.Owner)});
                            }
                            else // create a new friend entry
                            {
                                var friends = db.FriendEntries.Add(friend);

                                db.SaveChanges();
                            }
                        }
                        else // return. No such user
                        {
                            ViewBag.Owner = friend.Owner;
                            return View();
                        }
                    }

                    return RedirectToAction("Index");
                }
                ViewBag.Owner = friend.Owner;
                return View();
            }
            catch
            {
                ViewBag.Owner = friend.Owner;
                return View();
            }
        }

        //
        // GET: /Friends/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Friends/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
