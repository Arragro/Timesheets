using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    public class TimesheetEntry : Auditable<Guid>
    {
        public Guid TimesheetEntryId { get; private set; }
        public DateTime Date { get; private set; }
        [Range(0, 24)]
        public decimal NumberOfHours { get; private set; }
        [MaxLength(50)]
        public string Description { get; private set; }
        public Guid? ProjectId { get; private set; }
        public Guid UserId { get; private set; }

        public Project Project { get; private set; }

        protected TimesheetEntry()
        {
        }

        public TimesheetEntry(
            Guid userId, DateTime date, decimal numberOfHours, string description = null, Project project = null)
        {
            UserId = userId;
            ProjectId = project != null ? project.ProjectId : (Guid?)null;
            Date = date.Date;
            NumberOfHours = numberOfHours;
            Description = description;
        }

        public void ChangeNumberOfHours(decimal newNumberOfHours)
        {
            NumberOfHours = newNumberOfHours;
        }

        public void SetTimesheetEntryId()
        {
            if (TimesheetEntryId != default(Guid)) throw new Exception("The TimesheetEntryId is already set.");
            TimesheetEntryId = Guid.NewGuid();
        }
    }
}