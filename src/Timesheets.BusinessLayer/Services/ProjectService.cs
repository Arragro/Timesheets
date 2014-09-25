using Arragro.Common.ServiceBase;
using System;
using System.Linq;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Services
{
    public class ProjectService : AuditableService<IProjectRepository, Project, Guid, Guid>
    {
        public const string REQUIREDUSERID = "The project must have a UserId";
        public const string DUPLICATENAMEFORUSER = "There is already a project with the name {0} that you have created, project names must be unique";

        public ProjectService(IProjectRepository repository)
            : base(repository)
        {
        }

        private void HasUserId(Project model)
        {
            if (model.UserId == default(Guid))
                RulesException.ErrorFor(x => x.UserId, REQUIREDUSERID);
        }

        private void ProjectUniqueForUser(Project model)
        {
            if (Repository.All()
                    .Where(x => x.UserId == model.UserId
                             && x.Name == model.Name
                             && x.ProjectId != model.ProjectId)
                    .Any())
                RulesException.ErrorFor(x => x.Name, string.Format(DUPLICATENAMEFORUSER, model.Name));
        }

        public override void EnsureValidModel(Project model, params object[] relatedModels)
        {
            HasUserId(model);
            ProjectUniqueForUser(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public override Project InsertOrUpdate(Project model, Guid userId)
        {
            EnsureValidModel(model);
            var add = default(Guid) == model.ProjectId;
            AddOrUpdateAudit(model, userId, add);
            return Repository.InsertOrUpdate(model, add);
        }
    }
}