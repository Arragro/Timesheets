using Arragro.Common.ServiceBase;
using System;
using System.Linq;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Services
{
    public class ProjectInvitationService : AuditableService<IProjectInvitationRepository, ProjectInvitation, Guid, Guid>
    {
        public const string PROJECID_NOT_SET = "ProjectId is not set";
        public const string USER_IS_NOT_NULL_NOT_SET = "UserId is not null and is not set";
        public const string INVITATIONCODE_IS_NOT_SET = "Invitation Code is not set";
        public const string INVITATION_ACCEPTED_BEFORE_SENT = "Invitation has been accepted before it was sent";

        public ProjectInvitationService(
            IProjectInvitationRepository projectInvitationRepository)
            : base(projectInvitationRepository)
        {
        }

        private void EnsureProjectIdIsSet(ProjectInvitation projectInvitation)
        {
            if (projectInvitation.ProjectId == null ||
                projectInvitation.ProjectId == default(Guid))
                RulesException.ErrorFor(x => x.ProjectId, PROJECID_NOT_SET);
        }

        private void EnsureUserIdIsSet(ProjectInvitation projectInvitation)
        {
            if (projectInvitation.UserId.HasValue &&
                projectInvitation.UserId.Value == default(Guid))
                RulesException.ErrorFor(x => x.UserId, USER_IS_NOT_NULL_NOT_SET);
        }

        private void EnsureInvitationCodeIsSet(ProjectInvitation projectInvitation)
        {
            if (projectInvitation.InvitationCode == null ||
                projectInvitation.InvitationCode == default(Guid))
                RulesException.ErrorFor(x => x.InvitationCode, INVITATIONCODE_IS_NOT_SET);
        }

        private void EnsureInvitationAcceptedIsValid(ProjectInvitation projectInvitation)
        {
            if (projectInvitation.InvitationAccepted.HasValue &&
                !projectInvitation.InvitationSent)
                RulesException.ErrorFor(x => x.InvitationAccepted, INVITATION_ACCEPTED_BEFORE_SENT);
        }

        protected override void ValidateModelRules(ProjectInvitation model)
        {
            EnsureProjectIdIsSet(model);
            EnsureUserIdIsSet(model);
            EnsureInvitationCodeIsSet(model);
            EnsureInvitationAcceptedIsValid(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public override ProjectInvitation InsertOrUpdate(ProjectInvitation model, Guid userId)
        {
            var add = default(Guid) == model.ProjectInvitationId;
            AddOrUpdateAudit(model, userId, add);
            return Repository.InsertOrUpdate(model, add);
        }
    }
}