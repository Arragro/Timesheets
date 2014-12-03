using Arragro.EF6;
using System;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.Persistance.EF6.Repositories
{
    public class TimesheetEntryRepository : DbContextRepositoryBase<TimesheetEntry, Guid>, ITimesheetEntryRepository
    {
        public TimesheetEntryRepository(TimesheetsContext timesheetsContext)
            : base(timesheetsContext)
        {
        }
    }
}