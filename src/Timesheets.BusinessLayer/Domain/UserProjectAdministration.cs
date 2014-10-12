using Arragro.Common.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserProjectAdministration
    {
        public const string PROJECT_HAS_NOT_BEEN_SAVED = "You cannot save a project that has not been saved";
        public const string USER_IS_NOT_AUTHORISED = "User is not authorised to modify the project";

        public int UserId { get; private set; }

        private readonly ProjectService _projectService;

        public UserProjectAdministration(
            int userId,
            ProjectService projectService)
        {
            if (projectService == null) throw new ArgumentNullException("projectService");

            UserId = userId;
            _projectService = projectService;
        }

        private void EnsureProjectIsAlreadySaved(Project project)
        {
            var rulesException = new RulesException<Project>();

            if (project.ProjectId == default(Guid))
                rulesException.ErrorForModel(PROJECT_HAS_NOT_BEEN_SAVED);

            var existingProject = _projectService.Find(project.ProjectId);
            if (existingProject == null)
                rulesException.ErrorForModel(PROJECT_HAS_NOT_BEEN_SAVED);

            if (rulesException.Errors.Any()) throw rulesException;
        }

        private Project EnsureOwnerUserIdIsTheModifyingUser(
            Project project)
        {
            project.OwnerUserId = UserId;
            return project;
        }

        private void IsUserAuthorisedForAdministration(
            Project project)
        {
            var existingProject = _projectService.Find(project.ProjectId);
            if (existingProject != null && existingProject.OwnerUserId != UserId)
            {
                var rulesException = new RulesException<Project>();
                rulesException.ErrorForModel(USER_IS_NOT_AUTHORISED);
                throw rulesException;
            }
        }

        public Project TransferProjectOwnership(
            Project project, int newOwnerUserId)
        {
            EnsureProjectIsAlreadySaved(project);
            IsUserAuthorisedForAdministration(project);
            project.OwnerUserId = newOwnerUserId;

            _projectService.InsertOrUpdate(project, UserId);
            _projectService.SaveChanges();

            return project;
        }

        public Project UpdateProject(Project project)
        {
            EnsureProjectIsAlreadySaved(project);
            IsUserAuthorisedForAdministration(project);
            project = EnsureOwnerUserIdIsTheModifyingUser(project);
            project = _projectService.ValidateAndInsertOrUpdate(project, UserId);
            _projectService.SaveChanges();
            return project;
        }

        public Project AddProject(Project project)
        {
            project = EnsureOwnerUserIdIsTheModifyingUser(project);
            project = _projectService.ValidateAndInsertOrUpdate(project, UserId);
            _projectService.SaveChanges();
            return project;
        }

        public IEnumerable<Project> GetUsersProjects()
        {
            return _projectService.GetUsersProjects(UserId);
        }
    }
}