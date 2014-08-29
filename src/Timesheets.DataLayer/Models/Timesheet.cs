using Arragro.Common.BusinessRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
