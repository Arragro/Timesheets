using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheets.DataLayer.Models;

namespace Timesheets.DataLayer.Interfaces
{
    public interface IProjectRepository : IRepository<Project, int>
    {
    }
}
