using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    public class Timesheet : Auditable<Guid>
    {
        public Guid TimesheetId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Range(0, 24)]
        public decimal NumberOfHours { get; set; }

        [MaxLength(50)]
        public string Description { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public Guid UserId { get; set; }
    }
}