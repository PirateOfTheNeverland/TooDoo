using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

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
}