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
            _projectInvitationService.ValidateAndInsertOrUpdate(projectInvitation, User.Id);
            _projectInvitationService.SaveChanges();
            return projectInvitation;
        }

        private ProjectInvitation GetProjectInvitation(Guid projectInvitationId)
        {
            var projectInvitation = _projectInvitationService.Find(projectInvitationId);
            projectInvitation.SetProject(_projectService.Find(projectInvitation.ProjectId));
            return projectInvitation;
        }

        public ProjectContributor AcceptInvitation(ProjectInvitation projectInvitation)
        {
            projectInvitation.SetUserId(User.Id);
            projectInvitation.SetProjectInvitationAccepted(true);

            var projectContributor = new ProjectContributor(projectInvitation);
            projectContributor = _projectContributorService.ValidateAndInsertOrUpdate(projectContributor, User.Id);
            _projectContributorService.SaveChanges();
            return projectContributor;
        }
    }
}