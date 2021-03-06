﻿using Arragro.Common.Helpers;
using Arragro.Common.ServiceBase;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Services
{
    public class ProjectInvitationService : AuditableService<IProjectInvitationRepository, ProjectInvitation, Guid, Guid>
    {
        public const string PROJECTID_NOT_SET = "ProjectId is not set.";
        public const string USER_IS_NOT_NULL_NOT_SET = "Project Invitation is set and UserId is not null and is not set.";
        public const string INVITATIONCODE_IS_NOT_SET = "Invitation Code is not set.";
        public const string INVITATION_ACCEPTED_BEFORE_SENT = "Invitation has been accepted before it was sent.";
        public const string INVITATION_ALREADY_EXISTS_FOR_THIS_USERID = "For this Project there is already an invitation for the User.";
        public const string INVITATION_ALREADY_EXISTS_FOR_THIS_EMAILADDRESS = "For this Project there is already an invitation for the Email Address.";
        public const string INVITATIONCODE_DOES_NOT_EXIST = "There is no Project Invitation for that invitation code.";

        public ProjectInvitationService(
            IProjectInvitationRepository projectInvitationRepository)
            : base(projectInvitationRepository)
        {
        }

        private void EnsureProjectIdIsSet(ProjectInvitation projectInvitation)
        {
            if (projectInvitation.ProjectId == null ||
                projectInvitation.ProjectId == default(Guid))
                RulesException.ErrorFor(x => x.ProjectId, PROJECTID_NOT_SET);
        }

        private void EnsureUserIdIsSet(ProjectInvitation projectInvitation)
        {
            if (projectInvitation.InvitationAccepted.HasValue)
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

        private void EnsureInvitationIsUniqueForUserOrEmail(ProjectInvitation projectInvitation)
        {
            if (projectInvitation.ProjectInvitationId == default(Guid))
            {
                var query = (
                    from pi in Repository.All()
                    where pi.ProjectId == projectInvitation.ProjectId
                    select pi);

                if (projectInvitation.UserId.HasValue)
                {
                    query = query.Where(pi => pi.UserId.HasValue && pi.UserId.Value == projectInvitation.UserId.Value);
                    if (query.Any()) RulesException.ErrorForModel(INVITATION_ALREADY_EXISTS_FOR_THIS_USERID);
                }
                else if (!string.IsNullOrEmpty(projectInvitation.EmailAddress))
                {
                    query = query.Where(pi => pi.EmailAddress.Trim() == projectInvitation.EmailAddress.Trim());
                    if (query.Any()) RulesException.ErrorForModel(INVITATION_ALREADY_EXISTS_FOR_THIS_EMAILADDRESS);
                }
            }
        }

        protected override void ValidateModelRules(ProjectInvitation model)
        {
            EnsureProjectIdIsSet(model);
            EnsureUserIdIsSet(model);
            EnsureInvitationCodeIsSet(model);
            EnsureInvitationAcceptedIsValid(model);
            EnsureInvitationIsUniqueForUserOrEmail(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public override ProjectInvitation InsertOrUpdate(ProjectInvitation model, Guid userId)
        {
            var add = default(Guid) == model.ProjectInvitationId;
            if (add)
            {
                model.SetProjectInvitationId();
            }
            AddOrUpdateAudit(model, userId, add);

            Repository.InsertOrUpdate(model, add);
            Repository.SaveChanges();

            return model;
        }

        public void DeleteProjectInvitation(ProjectInvitation projectInvitation)
        {
            Repository.Delete(projectInvitation.ProjectInvitationId);
        }

        public IEnumerable<ProjectInvitation> GetProjectInvitations(Project project)
        {
            var projectInvitations = Repository.All().Where(i => i.ProjectId == project.ProjectId).ToList();
            foreach (var projectInvitation in projectInvitations) projectInvitation.SetProject(project);
            return projectInvitations;
        }

        public IEnumerable<ProjectInvitation> GetProjectInvitations(string emailAddress)
        {
            return Repository.All().Where(u => u.EmailAddress == emailAddress).ToList();
        }

        public ProjectInvitation GetProjectInvitationViaInvitationCode(Guid invitationCode)
        {
            var projectInvitation = Repository.All().SingleOrDefault(i => i.InvitationCode == invitationCode);
            if (projectInvitation == null)
                throw new ApplicationException(INVITATIONCODE_DOES_NOT_EXIST);
            return projectInvitation;
        }
    }
}