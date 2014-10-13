using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    public class TimesheetEntry : Auditable<Guid>
    {
        public Guid TimesheetEntryId { get; set; }
        public DateTime Date { get; set; }
        [Range(0, 24)]
        public decimal NumberOfHours { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }

        public Project Project { get; set; }
    }
}