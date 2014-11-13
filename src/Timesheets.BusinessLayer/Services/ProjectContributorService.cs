using Arragro.Common.Helpers;
using Arragro.Common.ServiceBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Services
{
    public class ProjectContributorService : AuditableService<IProjectContributorRepository, ProjectContributor, Guid, Guid>
    {
        public const string DATES_BOTH_MUST_BE_ENTERED_OR_NONE = "A ProjectContributor cannot have either a start or end date, they must have both or none.";
        public const string END_DATE_MUST_BE_MORE_THAN_OR_EQUAL_TO_START_DATE = "The End Date must be greater or equal to the Start Date.";
        public const string CONTRIBUTOR_MUST_HAVE_DATES_THAT_ARE_WITHIN_THE_PROJECT_DATES = "The Contributor's Start and End dates must be within the Project Start and End Dates.";
        public const string CONTRIBUTOR_ALREADY_EXISTS = "The Contributor already exists.";

        public ProjectContributorService(
            IProjectContributorRepository projectContributorRepository)
            : base(projectContributorRepository)
        {
        }

        private void ValidateStartEndDate(ProjectContributor model)
        {
            if ((model.StartDate.HasValue && !model.EndDate.HasValue) ||
                (!model.EndDate.HasValue && model.EndDate.HasValue))
                RulesException.ErrorForModel(DATES_BOTH_MUST_BE_ENTERED_OR_NONE);
            else if ((model.StartDate.HasValue && model.EndDate.HasValue) &&
                model.StartDate.Value.Date >= model.EndDate.Value.Date)
                RulesException.ErrorForModel(END_DATE_MUST_BE_MORE_THAN_OR_EQUAL_TO_START_DATE);
        }

        private void ValidAgainstProjectStartEndDates(ProjectContributor model)
        {
            var projectStartDate = model.Project.StartDate;
            var projectEndDate = model.Project.EndDate;
            if ((projectStartDate.HasValue && projectEndDate.HasValue) &&
                (model.StartDate.HasValue && model.EndDate.HasValue))
                if (model.StartDate.Value.Date < projectStartDate.Value.Date ||
                    model.EndDate.Value.Date > projectEndDate.Value.Date)
                    RulesException.ErrorForModel(CONTRIBUTOR_MUST_HAVE_DATES_THAT_ARE_WITHIN_THE_PROJECT_DATES);
        }

        public override ProjectContributor InsertOrUpdate(ProjectContributor model, Guid userId)
        {
            var add = default(Guid) == model.ProjectContributorId;
            if (add) model.SetProjectContributorId();
            AddOrUpdateAudit(model, userId, add);
            Repository.InsertOrUpdate(model, add);
            Repository.SaveChanges();

            return model;
        }

        protected override void ValidateModelRules(ProjectContributor model)
        {
            ValidateStartEndDate(model);
            ValidAgainstProjectStartEndDates(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public IEnumerable<Project> GetProjects(Guid userId)
        {
            return Repository.All()
                    .Where(c => c.UserId == userId)
                    .Select(c => c.Project)
                    .ToList();
        }

        public ProjectContributor GetProjectContributor(Project project, Guid userId)
        {
            var projectContributor = Repository.All()
                    .Where(c => c.ProjectId == project.ProjectId
                             && c.UserId == userId)
                    .SingleOrDefault();
            if (projectContributor != null)
                projectContributor.SetProject(project);
            return projectContributor;
        }

        public IEnumerable<ProjectContributor> GetProjectContributors(Project project)
        {
            var projectContributors = Repository.All().Where(c => c.ProjectId == project.ProjectId);
            foreach (var projectContributor in projectContributors)
                projectContributor.SetProject(project);
            return projectContributors;
        }
    }
}