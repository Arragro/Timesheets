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
        private readonly SecurityRules _securityRules;

        public UserProjectInvitations(
            IUser<Guid> user,
            CacheSettings cacheSettings,
            ProjectService projectService,
            ProjectInvitationService projectInvitationService,
            ProjectContributorService projectContributorService,
            SecurityRules securityRules)
        {
            if (cacheSettings == null) throw new ArgumentNullException("cacheSettings");
            if (projectService == null) throw new ArgumentNullException("projectService");
            if (projectInvitationService == null) throw new ArgumentNullException("projectInvitationService");
            if (projectContributorService == null) throw new ArgumentNullException("projectContributorService");
            if (securityRules == null) throw new ArgumentNullException("securityRules");

            User = user;
            _cacheSettings = cacheSettings;
            _projectService = projectService;
            _projectInvitationService = projectInvitationService;
            _projectContributorService = projectContributorService;
            _securityRules = securityRules;
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
            _securityRules.IsUserAuthorisedToReadProjectData(project, User);
            return _projectInvitationService.GetProjectInvitations(project);
        }

        public void DeleteProjectInvitation(ProjectInvitation projectInvitation)
        {
            _securityRules.IsUserAuthorisedToModifyProjectData(projectInvitation.Project, User);
            _projectInvitationService.DeleteProjectInvitation(projectInvitation);
            _projectInvitationService.SaveChanges();
        }

        public ProjectInvitation InviteUserToProject(
            Project project, string emailAddress)
        {
            var projectInvitation = new ProjectInvitation(project, emailAddress);
            _securityRules.IsUserAuthorisedToModifyProjectData(project, User);
            projectInvitation = _projectInvitationService.ValidateAndInsertOrUpdate(projectInvitation, User.Id);
            _projectInvitationService.SaveChanges();
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

            projectInvitation = _projectInvitationService.ValidateAndInsertOrUpdate(projectInvitation, User.Id);
            _projectInvitationService.SaveChanges();

            return projectInvitation;
        }

        public ProjectContributor AcceptInvitation(Guid invitationCode)
        {
            var projectInvitation = SetUserAndAcceptanceId(invitationCode, true);
            var projectContributor = new ProjectContributor(projectInvitation);
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