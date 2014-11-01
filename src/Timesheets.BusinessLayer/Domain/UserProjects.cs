﻿using Arragro.Common.BusinessRules;
using Arragro.Common.CacheProvider;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserProjects
    {
        public const string PROJECT_HAS_NOT_BEEN_SAVED = "You cannot save a project that has not been saved";
        public const string USER_IS_NOT_AUTHORISED = "User is not authorised to modify the project";

        private string UserProjectsKey
        {
            get
            {
                const string UserProjects = "UserTimesheetEntries:UserProjects:{0}";
                return string.Format(UserProjects, User.Id);
            }
        }

        public IUser<Guid> User { get; private set; }

        private readonly ProjectService _projectService;
        private readonly ProjectInvitationService _projectInvitationService;
        private readonly CacheSettings _cacheSettings;

        public UserProjects(
            IUser<Guid> user,
            CacheSettings cacheSettings,
            ProjectService projectService,
            ProjectInvitationService projectInvitationService)
        {
            if (cacheSettings == null) throw new ArgumentNullException("cacheSettings");
            if (projectService == null) throw new ArgumentNullException("projectService");
            if (projectInvitationService == null) throw new ArgumentNullException("projectInvitationService");

            User = user;
            _cacheSettings = cacheSettings;
            _projectService = projectService;
            _projectInvitationService = projectInvitationService;
        }

        private void ClearCache()
        {
            Cache.RemoveFromCache(UserProjectsKey);
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

        private Project EnsureOwnerUserIdIsTheModifyingUser(Project project)
        {
            project.ChangeProjectOwner(User.Id);
            return project;
        }

        private void IsUserAuthorisedForAdministration(Project project)
        {
            var existingProject = _projectService.Find(project.ProjectId);
            if (existingProject != null && existingProject.OwnerUserId != User.Id)
            {
                var rulesException = new RulesException<Project>();
                rulesException.ErrorForModel(USER_IS_NOT_AUTHORISED);
                throw rulesException;
            }
        }

        public Project TransferProjectOwnership(
            Project project, Guid newOwnerUserId)
        {
            EnsureProjectIsAlreadySaved(project);
            IsUserAuthorisedForAdministration(project);
            project.ChangeProjectOwner(newOwnerUserId);

            _projectService.InsertOrUpdate(project, User.Id);
            _projectService.SaveChanges();

            ClearCache();
            return project;
        }

        public Project UpdateProject(Project project)
        {
            EnsureProjectIsAlreadySaved(project);
            IsUserAuthorisedForAdministration(project);
            project = EnsureOwnerUserIdIsTheModifyingUser(project);
            project = _projectService.ValidateAndInsertOrUpdate(project, User.Id);
            _projectService.SaveChanges();
            ClearCache();
            return project;
        }

        public Project AddProject(Project project)
        {
            project = EnsureOwnerUserIdIsTheModifyingUser(project);
            project = _projectService.ValidateAndInsertOrUpdate(project, User.Id);
            _projectService.SaveChanges();
            ClearCache();
            return project;
        }

        public IEnumerable<Project> GetUserProjects()
        {
            return Cache.Get(
                UserProjectsKey,
                () => _projectService.GetUserProjects(User.Id),
                _cacheSettings);
        }
    }
}