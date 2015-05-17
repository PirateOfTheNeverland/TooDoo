﻿using TooDooSvc.Persistence;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using TooDooWebRole.Models;

namespace TooDooWebRole.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private IFixItTaskRepository fixItRepository = null;
        private IPhotoService photoService = null;
        private IFixItQueueManager queueManager = null;

        public TasksController(IFixItTaskRepository repository, IPhotoService photoStore, IFixItQueueManager queueManager)
        {
            fixItRepository = repository;
            photoService = photoStore;
            this.queueManager = queueManager;
        }

        public TasksController()
        {
            fixItRepository = null;
            photoService = null;
            this.queueManager = null;
        }

        // GET: /FixIt/
        public async Task<ActionResult> Status()
        {
            string currentUser = User.Identity.Name;
            var result = await fixItRepository.FindTasksByCreatorAsync(currentUser);

            return View(result);
        }
        /*
        public ActionResult Status()
        {
            string currentUser = User.Identity.Name;
            //var result = await fixItRepository.FindTasksByCreatorAsync(currentUser);

            //return View(currentUser);
            return View();
        }*/


        //
        // GET: /Tasks/Create
        //
        //public async Task<ActionResult> Create()
        public ActionResult Create()
        {
            string currentUser = User.Identity.Name;
            List<SelectListItem> FriendList = new List<SelectListItem>();

            using (FriendContext db = new FriendContext())
            {
                int i = 0;
                string sql = "SELECT * FROM dbo.FriendEntries WHERE Owner = @p0 AND Name NOT IN "
                             + "(SELECT DISTINCT Owner FROM dbo.FriendEntries WHERE Name=@p0 AND IsBlocked='true')";
                var friends = db.FriendEntries.SqlQuery(sql, currentUser);
                //list = friends;

                //.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                // Check if user already exists
                foreach (FriendEntry entry in friends)
                {
                    FriendList.Add(new SelectListItem { Text = entry.Name, Value = i.ToString() });
                    i++;
                }

                ViewData["CreatedBy"] = currentUser;
                ViewData["FriendList"] = FriendList;
                return View();

            }
        }

        //
        // POST: /Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FixItTaskId,CreatedBy,Owner,Title,Notes,PhotoUrl,IsDone")]FixItTask fixittask, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
//                using (FriendContext db = new FriendContext())
//                {
//                    var IsBlocked = db.FriendEntries.SqlQuery("SELECT * FROM dbo.FriendEntries WHERE Name=@p0 AND Owner=@p1",
//                        new object[] { new SqlParameter("p0", fixittask.CreatedBy), new SqlParameter("p1", fixittask.Owner) });
                    fixittask.CreatedBy = User.Identity.Name;
                    fixittask.PhotoUrl = await photoService.UploadPhotoAsync(photo);

                    await fixItRepository.CreateAsync(fixittask);

                    return RedirectToAction("Success");
//                }
            }

            return View(fixittask);
        }

        //
        // GET: /Tasks/Success
        public ActionResult Success()
        {
            return View();
        }
    }
}