using Arragro.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.DataLayer.InMemoryRepositories
{
    public class ProjectRepository : InMemoryRepository<Project, Guid>, IProjectRepository
    {
    }
}
