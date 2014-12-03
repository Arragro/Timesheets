using Arragro.EF6;
using System;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.Persistance.EF6.Repositories
{
    public class ProjectRepository : DbContextRepositoryBase<Project, Guid>, IProjectRepository
    {
        public ProjectRepository(TimesheetsContext timesheetsContext)
            : base(timesheetsContext)
        {
        }
    }
}