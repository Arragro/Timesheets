using Arragro.EF6;
using System.Data.Entity;
using Timesheets.DataLayer.Models;

namespace Timesheets.Persistance.EF6
{
    public class TimesheetsContext : BaseContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }
        public DbSet<ProjectInvitation> ProjectInvitations { get; set; }
        public DbSet<ProjectContributor> ProjectContributors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TimesheetEntry>()
                .HasOptional(t => t.Project)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProjectInvitation>()
                .HasRequired(t => t.Project)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProjectContributor>()
                .HasRequired(t => t.Project)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}