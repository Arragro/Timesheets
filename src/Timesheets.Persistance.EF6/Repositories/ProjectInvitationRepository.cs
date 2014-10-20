using Arragro.EF6;
using System;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.Persistance.EF6.Repositories
{
    public class ProjectInvitationRepository : DbContextRepositoryBase<ProjectInvitation, Guid>, IProjectInvitationRepository
    {
        public ProjectInvitationRepository(TimesheetsContext timesheetsContext)
            : base(timesheetsContext)
        {
        }
    }
}