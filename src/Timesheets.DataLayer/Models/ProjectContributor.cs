using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;
using Timesheets.DataLayer.Enums;

namespace Timesheets.DataLayer.Models
{
    [Serializable]
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

        private void ConfigureProjectContributor(
            Project project, Guid userId)
        {
            if (project == null) throw new ArgumentNullException("project", "The Project supplied is null.");
            if (userId == default(Guid))
                throw new ArgumentNullException("userId", "The UserId supplied is null.");

            ProjectId = project.ProjectId;
            UserId = userId;
            ContributorRole = ContributorRole.User;
            WeeklyContributorHoursLimit = null;
            RequiresApproval = false;
            StartDate = project.StartDate;
            EndDate = project.EndDate;
            PurchaseOrderNumber = null;
            HourlyRate = null;

            Project = project;
        }

        public ProjectContributor(
            ProjectInvitation projectInvitation)
        {
            if (projectInvitation == null) throw new ArgumentNullException("projectInvitation", "The Project Invitation supplied is null.");
            if (!projectInvitation.UserId.HasValue ||
                (projectInvitation.UserId.HasValue && projectInvitation.UserId.Value == default(Guid)))
                throw new ArgumentNullException("projectInvitation.UserId", "The Project Invitation does not have a UserId.");
            ConfigureProjectContributor(projectInvitation.Project, projectInvitation.UserId.Value);
        }

        public ProjectContributor(
            Project project, Guid userId)
        {
            ConfigureProjectContributor(project, userId);
        }

        public void SetProjectContributorId()
        {
            if (ProjectContributorId != default(Guid)) throw new Exception("The ProjectContributorId is already set");
            ProjectContributorId = Guid.NewGuid();
        }

        public void SetContributorRole(ContributorRole contributorRole)
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

        public void SetProject(Project project)
        {
            Project = project;
        }
    }
}