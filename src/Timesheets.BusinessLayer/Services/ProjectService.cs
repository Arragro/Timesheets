using Arragro.Common.ServiceBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Services
{
    public class ProjectService : AuditableService<IProjectRepository, Project, Guid, Guid>
    {
        public const string REQUIRED_OWNERUSERID = "The project must have a OwnerUserId";
        public const string DUPLICATE_NAME_FOR_USER = "There is already a project with the name {0} that you have created, project names must be unique";
        public const string PROJECT_MUST_BE_AT_LEAST_A_DAY_LONG = "A project must be at least a day long, please adjust the start and end dates";

        public ProjectService(IProjectRepository repository)
            : base(repository)
        {
        }

        private void HasOwnerUserId(Project model)
        {
            if (model.OwnerUserId == default(Guid))
                RulesException.ErrorFor(x => x.OwnerUserId, REQUIRED_OWNERUSERID);
        }

        private void ProjectUniqueForUser(Project model)
        {
            if (Repository.All()
                    .Where(x => x.OwnerUserId == model.OwnerUserId
                             && x.Name == model.Name
                             && x.ProjectId != model.ProjectId)
                    .Any())
                RulesException.ErrorFor(x => x.Name, string.Format(DUPLICATE_NAME_FOR_USER, model.Name));
        }

        private void ValidateDates(Project model)
        {
            if ((model.EndDate - model.StartDate).Days < 1)
                RulesException.ErrorForModel(PROJECT_MUST_BE_AT_LEAST_A_DAY_LONG);
        }

        protected override void ValidateModelRules(Project model)
        {
            HasOwnerUserId(model);
            ProjectUniqueForUser(model);
            ValidateDates(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public override Project InsertOrUpdate(Project model, Guid userId)
        {
            var add = default(Guid) == model.ProjectId;
            AddOrUpdateAudit(model, userId, add);
            return Repository.InsertOrUpdate(model, add);
        }

        public IEnumerable<Project> GetUserProjects(Guid userId)
        {
            return Repository.All()
                .Where(p => p.OwnerUserId == userId)
                .ToList();
        }
    }
}