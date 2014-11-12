using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    [Serializable]
    public class ProjectInvitation : Auditable<Guid>
    {
        public Guid ProjectInvitationId { get; private set; }
        public Guid ProjectId { get; private set; }
        public Guid? UserId { get; private set; }
        [MaxLength(254)]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; private set; }
        public Guid InvitationCode { get; private set; }
        public bool InvitationSent { get; private set; }
        public bool? InvitationAccepted { get; private set; }

        public Project Project { get; private set; }

        protected ProjectInvitation()
        {
        }

        public ProjectInvitation(
            Project project,
            string emailAddress)
        {
            if (project == null) throw new ArgumentNullException("project", "The Project supplied is null.");
            ProjectId = project.ProjectId;
            EmailAddress = emailAddress;
            InvitationCode = Guid.NewGuid();
            Project = project;
        }

        public void SetProjectInvitationId()
        {
            if (ProjectInvitationId != default(Guid)) throw new Exception("The ProjectId is already set.");
            ProjectInvitationId = Guid.NewGuid();
        }

        public void SetProject(Project project)
        {
            if (project == null) throw new ArgumentNullException("project");
            Project = project;
        }

        public void SetProjectInvitationSent(bool invitationSent)
        {
            InvitationSent = invitationSent;
        }

        public void SetProjectInvitationAccepted(bool invitationAccepted)
        {
            if (!InvitationSent) throw new Exception("You cannot accept an invitation if it hasn't been sent.");
            InvitationAccepted = invitationAccepted;
        }

        public void SetUserId(Guid userId)
        {
            if (UserId.HasValue) throw new Exception("The UserId is already set.");
            if (userId == default(Guid)) throw new ArgumentException("You have supplied an empty UserId.", "userId");
            UserId = userId;
        }
    }
}