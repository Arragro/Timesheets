using Arragro.Common.BusinessRules;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class SecurityRules
    {
        public const string USER_IS_NOT_AUTHORISED = "User is not authorised to modify the project";

        private readonly ProjectService _projectService;
        private readonly ProjectContributorService _projectContributorService;

        public SecurityRules(
            ProjectService projectService,
            ProjectContributorService projectContributorService)
        {
            if (projectService == null) throw new ArgumentNullException("projectService");
            if (projectContributorService == null) throw new ArgumentNullException("projectContributorService");

            _projectService = projectService;
            _projectContributorService = projectContributorService;
        }

        public void IsUserAuthorisedToModifyProjectData(Project project, IUser<Guid> user)
        {
            var rulesException = new RulesException();
            var existingProject = _projectService.Find(project.ProjectId);
            if (existingProject == null) throw new ArgumentNullException("project", "The supplied project does not exist in the database.");

            if (existingProject != null && existingProject.OwnerUserId != user.Id)
                rulesException.ErrorForModel(USER_IS_NOT_AUTHORISED);
            else
            {
                var projectContributor = _projectContributorService.GetProjectContributor(project, user.Id);

                if (projectContributor == null ||
                    (projectContributor != null && projectContributor.ContributorRole != DataLayer.Enums.ContributorRole.Administrator))
                    rulesException.ErrorForModel(USER_IS_NOT_AUTHORISED);
            }
            if (rulesException.Errors.Any())
                throw rulesException;
        }
    }
}