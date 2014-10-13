using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;
using Timesheets.DataLayer.Enums;

namespace Timesheets.DataLayer.Models
{
    public class ProjectContributor : Auditable<Guid>
    {
        public Guid ProjectContributorId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public ContributorRole ContributorRole { get; set; }
        public decimal WeeklyContributorHoursLimit { get; set; }
        public bool RequiresApproval { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(20)]
        public string PurchaseOrderNumber { get; set; }
        public decimal HourlyRate { get; set; }
    }
}