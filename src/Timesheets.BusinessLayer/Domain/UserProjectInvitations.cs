using Arragro.Common.CacheProvider;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserProjectInvitations
    {
        public IUser<Guid> User { get; private set; }

        private readonly CacheSettings _cacheSettings;
        private readonly ProjectService _projectService;
        private readonly ProjectInvitationService _projectInvitationService;
        private readonly ProjectContributorService _projectContributorService;

        public UserProjectInvitations(
            IUser<Guid> user,
            CacheSettings cacheSettings,
            ProjectService projectService,
            ProjectInvitationService projectInvitationService,
            ProjectContributorService projectContributionService)
        {
            User = user;
            _cacheSettings = cacheSettings;
            _projectService = projectService;
            _projectInvitationService = projectInvitationService;
            _projectContributorService = projectContributionService;
        }

        private IEnumerable<ProjectInvitation> LoadProjects(IEnumerable<ProjectInvitation> projectInvitations)
        {
            foreach (var projectInvitation in projectInvitations)
            {
                projectInvitation.SetProject(_projectService.Find(projectInvitation.ProjectId));
            }
            return projectInvitations;
        }

        public IEnumerable<ProjectInvitation> GetProjectInvitations(Project project)
        {
            return _projectInvitationService.GetProjectInvitations(project);
        }

        public ProjectInvitation InviteUserToProject(
            Project project, string emailAddress)
        {
            var projectInvitation = new ProjectInvitation(project, emailAddress);
            //This also execute SaveChanges as a result of object graph issues in Entity Framework...
            projectInvitation = _projectInvitationService.ValidateAndInsertOrUpdate(projectInvitation, User.Id);
            return projectInvitation;
        }

        private ProjectInvitation GetProjectInvitation(Guid projectInvitationId)
        {
            var projectInvitation = _projectInvitationService.Find(projectInvitationId);
            projectInvitation.SetProject(_projectService.Find(projectInvitation.ProjectId));
            return projectInvitation;
        }

        private ProjectInvitation SetUserAndAcceptanceId(Guid invitationCode, bool accepted)
        {
            var projectInvitation = _projectInvitationService.GetProjectInvitationViaInvitationCode(invitationCode);
            projectInvitation.SetProject(_projectService.GetProject(projectInvitation.ProjectId));

            projectInvitation.SetUserId(User.Id);
            projectInvitation.SetProjectInvitationAccepted(accepted);

            //This also execute SaveChanges as a result of object graph issues in Entity Framework...
            projectInvitation = _projectInvitationService.ValidateAndInsertOrUpdate(projectInvitation, User.Id);
            _projectInvitationService.SaveChanges();
            //projectInvitation.SetProject(_projectService.Find(projectInvitation.ProjectId));
            return projectInvitation;
        }

        public ProjectContributor AcceptInvitation(Guid invitationCode)
        {
            var projectInvitation = SetUserAndAcceptanceId(invitationCode, true);
            var projectContributor = new ProjectContributor(projectInvitation);
            //This also execute SaveChanges as a result of object graph issues in Entity Framework...
            projectContributor = _projectContributorService.ValidateAndInsertOrUpdate(projectContributor, User.Id);
            _projectContributorService.SaveChanges();
            return projectContributor;
        }

        public void RejectInvitation(Guid invitationCode)
        {
            var projectInvitation = SetUserAndAcceptanceId(invitationCode, false);
        }
    }
}