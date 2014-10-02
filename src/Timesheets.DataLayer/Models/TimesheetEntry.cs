using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    public class TimesheetEntry : Auditable<int>
    {
        public Guid TimesheetEntryId { get; set; }
        public DateTime Date { get; set; }
        [Range(0, 24)]
        public decimal NumberOfHours { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int UserId { get; set; }
    }
}