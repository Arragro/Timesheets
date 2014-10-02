using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    public class Project : Auditable<int>
    {
        public Guid ProjectId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string Code { get; set; }
        [MaxLength(20)]
        public string PurchaseOrderNumber { get; set; }
        public decimal Budget { get; set; }
        public int WeeklyUserHoursLimit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
    }
}