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
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool HasNewFriend { get; set; }
        public bool HasNewTooDoo { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool HasNewFriend { get; set; }
        public bool HasNewTooDoo { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }

    public class AccountManagement
    {
        private UsersContext db = new UsersContext();
        private ILogger log = null;

        public AccountManagement() { }
        public AccountManagement(ILogger logger)
        {
            log = logger;
        }

        public async Task<UserProfile> FindUserByIdAsync(int id)
        {
            UserProfile user = null;
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                user = await db.UserProfiles.FindAsync(id);

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindTooDooByIdAsync", timespan.Elapsed, "id={0}", id);
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindToodooByIdAsync(id={0})", id);
                throw;
            }

            return user;
        }

        public async Task<UserProfile> FindUserByNameAsync(string userName)
        {
            UserProfile user = null;
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.UserProfiles
                    .Where(t => t.UserName == userName)
                    //.Where(t => t.Name == friendName)
                    .OrderByDescending(t => t.UserId).ToListAsync();

                if (result.Count == 1)
                {
                    user = result[0];
                }
                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindTooDooByIdAsync", timespan.Elapsed, "id={0}", id);
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindToodooByIdAsync(id={0})", id);
                throw;
            }

            return user;
        }

        public async Task<List<UserProfile>> FindNotBlockedFriendsByOwnerAsync(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.UserProfiles
                    .Where(t => t.UserName == userName)
                    //.Where(t => t.IsBlocked == false)
                    .OrderByDescending(t => t.UserId).ToListAsync();

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

        public async Task<List<UserProfile>> FindBlockedFriendsByOwnerAsync(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.UserProfiles
                    .Where(t => t.UserName == userName)
                    //.Where(t => t.IsBlocked == true)
                    .OrderByDescending(t => t.UserId).ToListAsync();

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

        public async Task<bool> HasNewFriend(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();
            //if (userName == null || userName == "") return null;
            try
            {
                var result = await db.UserProfiles
                    .Where(t => t.UserName == userName).ToListAsync();
                    //.Select(t => t.HasNewFriend);

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindToodoosByCreatorAsync", timespan.Elapsed, "creater={0}", creator);

                return result.First().HasNewFriend;
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindToodoosByCreatorAsync(creater={0})", creator);
                throw;
            }
        }

        public async Task<bool> HasNewTooDoo(string userName)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.UserProfiles
                    .Where(t => t.UserName == userName).ToListAsync();
                //.Select(t => t.HasNewFriend);

                timespan.Stop();
                //log.TraceApi("SQL Database", "TooDooManagement.FindToodoosByCreatorAsync", timespan.Elapsed, "creater={0}", creator);

                return result.First().HasNewTooDoo;
            }
            catch (Exception e)
            {
                //log.Error(e, "Error in TooDooManagement.FindToodoosByCreatorAsync(creater={0})", creator);
                throw;
            }
        }

        public async Task<List<UserProfile>> FindFriendsByOwnerAsync(string creator)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                var result = await db.UserProfiles
                    .Where(t => t.UserName == creator)
                    .OrderByDescending(t => t.UserId).ToListAsync();

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

        public async Task CreateAsync(UserProfile userToAdd)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                db.UserProfiles.Add(userToAdd);
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

        public async Task UpdateAsync(UserProfile userToSave)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                db.Entry(userToSave).State = EntityState.Modified;
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
                UserProfile user = await db.UserProfiles.FindAsync(id);
                db.UserProfiles.Remove(user);
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

        public async Task NotifyHasNewFriendById(UserProfile userToNotify)
        {
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                db.Entry(userToNotify).State = EntityState.Modified;
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
