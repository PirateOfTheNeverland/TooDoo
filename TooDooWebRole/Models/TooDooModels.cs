using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

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
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(80)]
        public string Owner { get; set; }
        [StringLength(80)]
        public string Title { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
        [StringLength(200)]
        public string PhotoUrl { get; set; }
        public bool IsDone { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}