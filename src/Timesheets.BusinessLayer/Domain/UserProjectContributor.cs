using Arragro.Common.BusinessRules;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Enums;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserProjectContributor
    {
        public const string OWNER_ROLE_CANNOT_BE_CHANGED = "You cannot modify the Role of a Project Owner.";

        public IUser<Guid> User { get; private set; }

        private readonly SecurityRules _securityRules;
        private readonly ProjectService _projectService;
        private readonly ProjectContributorService _projectContributorService;

        public UserProjectContributor(
            IUser<Guid> user,
            SecurityRules securityRules,
            ProjectService projectService,
            ProjectContributorService projectContributorService)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (securityRules == null) throw new ArgumentNullException("securityRules");
            if (projectService == null) throw new ArgumentNullException("projectService");
            if (projectContributorService == null) throw new ArgumentNullException("projectContributorService");

            User = user;
            _securityRules = securityRules;
            _projectService = projectService;
            _projectContributorService = projectContributorService;
        }

        private void EnsureUserIdNotOwner(ProjectContributor projectContributor)
        {
            var rulesException = new RulesException();
            var project = _projectService.Find(projectContributor.ProjectId);

            if (project.OwnerUserId == projectContributor.UserId)
                rulesException.ErrorForModel(OWNER_ROLE_CANNOT_BE_CHANGED);

            if (rulesException.Errors.Any())
                throw rulesException;
        }

        public ProjectContributor ChangeProjectContributorsRole(
            ProjectContributor projectContributor, ContributorRole contributorRole)
        {
            _securityRules.IsUserAuthorisedToModifyProjectData(projectContributor.Project, User);
            EnsureUserIdNotOwner(projectContributor);

            projectContributor.SetContributorRole(contributorRole);

            _projectContributorService.ValidateAndInsertOrUpdate(projectContributor, User.Id);
            _projectContributorService.SaveChanges();

            return projectContributor;
        }

        public ProjectContributor GetProjectContributor(Project project, Guid userId)
        {
            _securityRules.IsUserAuthorisedToReadProjectData(project, User);
            return _projectContributorService.GetProjectContributor(project, userId);
        }

        public IEnumerable<ProjectContributor> GetProjectContributors(Project project)
        {
            _securityRules.IsUserAuthorisedToReadProjectData(project, User);
            return _projectContributorService.GetProjectContributors(project);
        }
    }
}