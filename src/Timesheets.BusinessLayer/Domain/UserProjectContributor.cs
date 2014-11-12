using Microsoft.AspNet.Identity;
using System;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Enums;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserProjectContributor
    {
        public IUser<Guid> User { get; private set; }

        private readonly SecurityRules _securityRules;
        private readonly ProjectContributorService _projectContributorService;

        public UserProjectContributor(
            IUser<Guid> user,
            SecurityRules securityRules,
            ProjectContributorService projectContributorService)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (securityRules == null) throw new ArgumentNullException("securityRules");
            if (projectContributorService == null) throw new ArgumentNullException("projectContributorService");

            User = user;
            _securityRules = securityRules;
            _projectContributorService = projectContributorService;
        }

        private void EnsureUserIdNotOwner(ProjectContributor projectContributor)
        {
        }

        public ProjectContributor ChangeProjectContributorsRole(
            ProjectContributor projectContributor, ContributorRole contributorRole)
        {
            _securityRules.IsUserAuthorisedToModifyProjectData(projectContributor.Project, User);
            projectContributor.SetContributorRole(contributorRole);

            _projectContributorService.ValidateAndInsertOrUpdate(projectContributor, User.Id);
            _projectContributorService.SaveChanges();

            return projectContributor;
        }
    }
}