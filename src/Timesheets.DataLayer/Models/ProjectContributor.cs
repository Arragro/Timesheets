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
        public decimal? WeeklyContributorHoursLimit { get; private set; }
        public bool RequiresApproval { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        [MaxLength(20)]
        public string PurchaseOrderNumber { get; private set; }
        public decimal? HourlyRate { get; private set; }

        public Project Project { get; private set; }

        protected ProjectContributor()
        { }

        public ProjectContributor(
            ProjectInvitation projectInvitation)
        {
            if (projectInvitation == null) throw new ArgumentNullException("projectInvitation", "The Project Invitation supplied is null");
            if (projectInvitation.Project == null) throw new ArgumentNullException("projectInvitation.Project", "The Project supplied is null");
            if (!projectInvitation.UserId.HasValue ||
                (projectInvitation.UserId.HasValue && projectInvitation.UserId.Value == default(Guid)))
                throw new ArgumentNullException("projectInvitation.UserId", "The Project Invitation does not have a UserId");
            ProjectId = projectInvitation.Project.ProjectId;
            UserId = projectInvitation.UserId.Value;
            ContributorRole = ContributorRole.User;
            WeeklyContributorHoursLimit = null;
            RequiresApproval = false;
            StartDate = projectInvitation.Project.StartDate;
            EndDate = projectInvitation.Project.EndDate;
            PurchaseOrderNumber = null;
            HourlyRate = null;

            Project = projectInvitation.Project;
        }

        public void SetProjectContributorId()
        {
            if (ProjectContributorId != default(Guid)) throw new Exception("The ProjectContributorId is already set");
            ProjectContributorId = Guid.NewGuid();
        }

        public void SetContributorRoles(ContributorRole contributorRole)
        {
            ContributorRole = contributorRole;
        }

        public void SetContributorProjectDetails(
            decimal? weeklyContributorHoursLimit = null, bool requiresApproval = false,
            DateTime? startDate = null, DateTime? endDate = null,
            string purchaseOrderNumber = null, decimal? hourlyRate = null)
        {
            WeeklyContributorHoursLimit = weeklyContributorHoursLimit;
            RequiresApproval = requiresApproval;
            StartDate = startDate;
            EndDate = endDate;
            PurchaseOrderNumber = purchaseOrderNumber;
            HourlyRate = hourlyRate;
        }
    }
}