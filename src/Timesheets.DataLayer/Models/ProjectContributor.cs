using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;
using Timesheets.DataLayer.Enums;

namespace Timesheets.DataLayer.Models
{
    public class ProjectContributor : Auditable<Guid>
    {
        public Guid ProjectContributorId { get; private set; }
        public Guid ProjectId { get; private set; }
        public Guid UserId { get; private set; }
        public ContributorRole ContributorRole { get; private set; }
        public decimal WeeklyContributorHoursLimit { get; private set; }
        public bool RequiresApproval { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        [MaxLength(20)]
        public string PurchaseOrderNumber { get; private set; }
        public decimal HourlyRate { get; private set; }

        public Project Project { get; private set; }

        protected ProjectContributor()
        { }

        public ProjectContributor(
            ProjectInvitation projectInvitation)
        {
            if (projectInvitation == null) throw new ArgumentNullException("projectInviation", "The Project Invitation supplied is null");
            if (projectInvitation.Project == null) throw new ArgumentNullException("projectInviation", "The Project Invitation supplied is null");
            ProjectId = projectInvitation.Project.ProjectId;
        }
    }
}