using Arragro.Common.Repository;
using System;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.DataLayer.InMemoryRepositories
{
    public class TimesheetRepository : InMemoryRepository<Timesheet, Guid>, ITimesheetRepository
    {
    }
}