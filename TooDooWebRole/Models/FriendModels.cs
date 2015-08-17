using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Security;

using TooDooSvc.Logging;

namespace TooDooWebRole.Models
{
    public class FriendContext : DbContext
    {
        public FriendContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<FriendEntry> FriendEntries { get; set; }
    }

    [Table("FriendEntries")]
    public class FriendEntry
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int FriendId { get; set; }
        [Required]
        [StringLength(80)]
        public string Owner { get; set; }
        [Required]
        [StringLength(80)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }
    }

    public class FriendManagement
    {
        private FriendContext db = new FriendContext();
        private ILogger log = null;

        public FriendManagement() { }
        public FriendManagement(ILogger logger)
        {
            log = logger;
        }

        public async Task<FriendEntry> FindFriendByIdAsync(int id)
        {
            FriendEntry friend = null;
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                friend = await db.FriendEntries.FindAsync(id);

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindTooDooByIdAsync", timespan.Elapsed, "id={0}", id);
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindToodooByIdAsync(id={0})", id);
                throw;
            }

            return friend;
        }

        public async Task<FriendEntry> FindFriendByNameAndOwnerAsync(string friendName, string owner)
        {
            FriendEntry friend = null;
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.FriendEntries
                    .Where(t => t.Owner == owner)
                    .Where(t => t.Name == friendName)
                    .OrderByDescending(t => t.FriendId).ToListAsync();

                if (result.Count == 1)
                {
                    friend = result[0];
                }
                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindTooDooByIdAsync", timespan.Elapsed, "id={0}", id);
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindToodooByIdAsync(id={0})", id);
                throw;
            }

            return friend;
        }

        public async Task<List<FriendEntry>> FindNotBlockedFriendsByOwnerAsync(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.FriendEntries
                    .Where(t => t.Owner == userName)
                    .Where(t => t.IsBlocked == false)
                    .OrderByDescending(t => t.FriendId).ToListAsync();

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindOpenToodoosByOwnerAsync", timespan.Elapsed, "username={0}", userName);

                return result;
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindOpenToodoosByOwnerAsync(userName={0})", userName);
                throw;
            }
        }

        public async Task<List<FriendEntry>> FindBlockedFriendsByOwnerAsync(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.FriendEntries
                    .Where(t => t.Owner == userName)
                    .Where(t => t.IsBlocked == true)
                    .OrderByDescending(t => t.FriendId).ToListAsync();

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindClosedToodoosByOwnerAsync", timespan.Elapsed, "username={0}", userName);

                return result;
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindClosedToodoosByOwnerAsync(userName={0})", userName);
                throw;
            }
        }


        public async Task<List<FriendEntry>> FindFriendsByOwnerAsync(string creator)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.FriendEntries
                    .Where(t => t.Owner == creator)
                    .OrderByDescending(t => t.FriendId).ToListAsync();

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindToodoosByCreatorAsync", timespan.Elapsed, "creater={0}", creator);

                return result;
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindToodoosByCreatorAsync(creater={0})", creator);
                throw;
            }
        }

        public async Task CreateAsync(FriendEntry friendToAdd)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                db.FriendEntries.Add(friendToAdd);
                await db.SaveChangesAsync();

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.CreateAsync", timespan.Elapsed, "taskToAdd={0}", toodooToAdd);
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.CreateAsync(toodooToAdd={0})", toodooToAdd);
                throw;
            }
        }

        public async Task UpdateAsync(FriendEntry friendToSave)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                db.Entry(friendToSave).State = EntityState.Modified;
                await db.SaveChangesAsync();

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.UpdateAsync", timespan.Elapsed, "taskToSave={0}", toodooToSave);
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.UpdateAsync(toodooToSave={0})", toodooToSave);
                throw;
            }
        }

        public async Task DeleteAsync(Int32 id)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                FriendEntry friend = await db.FriendEntries.FindAsync(id);
                db.FriendEntries.Remove(friend);
                await db.SaveChangesAsync();

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.DeleteAsync", timespan.Elapsed, "id={0}", id);

            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.DeleteAsync(id={0})", id);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free managed resources
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
    }

}