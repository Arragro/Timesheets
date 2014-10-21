using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
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
            string emailAddress,
            Guid? userId = null)
        {
            ProjectId = project == null ? new Guid() : project.ProjectId;
            UserId = userId;
            EmailAddress = emailAddress;
            InvitationCode = Guid.NewGuid();
        }

        public void SetProjectInvitationId()
        {
            if (ProjectInvitationId != default(Guid)) throw new Exception("The ProjectId is already set.");
            ProjectInvitationId = Guid.NewGuid();
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
    }
}