using Arragro.EF6;
using System;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.Persistance.EF6.Repositories
{
    public class ProjectContributorRepository : DbContextRepositoryBase<ProjectContributor, Guid>, IProjectContributorRepository
    {
        public ProjectContributorRepository(TimesheetsContext timesheetsContext)
            : base(timesheetsContext)
        {
        }
    }
}