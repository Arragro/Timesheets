using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;
using Timesheets.DataLayer.Enums;

namespace Timesheets.DataLayer.Models.Bases
{
    public class ContributorDetails : Auditable<Guid>
    {
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