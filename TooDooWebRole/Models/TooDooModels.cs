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
    public class TooDooContext : DbContext
    {
        public TooDooContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<TooDooEntry> TooDooEntries { get; set; }
    }

    [Table("TooDooEntries")]
    public class TooDooEntry
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TooDooId { get; set; }
        [Required]
        [StringLength(80)]
        [Display(Name="Created by")]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(80)]
        [Display(Name="Assigned to")]
        public string Owner { get; set; }
        [Required]
        [StringLength(80)]
        [Display(Name="Title")]
        public string Title { get; set; }
        [StringLength(1000)]
        [Display(Name="Notes")]
        public string Notes { get; set; }
        [StringLength(200)]
        public string PhotoUrl { get; set; }
        public bool IsDone { get; set; }
        [Required]
        [Display(Name="Creation date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }
    }

    public class TooDooManagement
    {
        private TooDooContext db = new TooDooContext();
        private ILogger log = null;

        public TooDooManagement() { }
        public TooDooManagement(ILogger logger)
        {
            log = logger;
        }

        public async Task<TooDooEntry> FindTooDooByIdAsync(int id)
        {
            TooDooEntry toodoo = null;
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                toodoo = await db.TooDooEntries.FindAsync(id);
                
                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindTooDooByIdAsync", timespan.Elapsed, "id={0}", id);
            }
            catch(Exception e)
            {
               //log.Error(e, "Error in TooDooManagement.FindToodooByIdAsync(id={0})", id);
               throw;
            }

            return toodoo;
        }

        public async Task<List<TooDooEntry>> FindOpenToodoosByOwnerAsync(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.TooDooEntries
                    .Where(t => t.Owner == userName)
                    .Where(t => t.IsDone == false)
                    .OrderByDescending(t => t.TooDooId).ToListAsync();

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

        public async Task<List<TooDooEntry>> FindClosedToodoosByOwnerAsync(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.TooDooEntries
                    .Where(t => t.Owner == userName)
                    .Where(t => t.IsDone == true)
                    .OrderByDescending(t => t.TooDooId).ToListAsync();

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

        
        public async Task<List<TooDooEntry>> FindToodoosByCreatorAsync(string creator)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.TooDooEntries
                    .Where(t => t.CreatedBy == creator)
                    .OrderByDescending(t => t.TooDooId).ToListAsync();

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

        public async Task CreateAsync(TooDooEntry toodooToAdd)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                db.TooDooEntries.Add(toodooToAdd);
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

        public async Task UpdateAsync(TooDooEntry toodooToSave)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                db.Entry(toodooToSave).State = EntityState.Modified;
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
                TooDooEntry toodoo = await db.TooDooEntries.FindAsync(id);
                db.TooDooEntries.Remove(toodoo);
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