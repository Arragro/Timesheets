using Microsoft.AspNet.Identity;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserTimesheetEntries
    {
        public int UserId { get; private set; }

        private readonly TimesheetEntryService _timesheetEntryService;

        public UserTimesheetEntries(TimesheetEntryService timesheetEntryService)
        {
            _timesheetEntryService = timesheetEntryService;
        }

        public TimesheetEntry AddTimesheetEntry(
            TimesheetEntry timesheetEntry, IUser<int> auditUser)
        {
            timesheetEntry =
                _timesheetEntryService.ValidateAndInsertOrUpdate(timesheetEntry, auditUser.Id);
            _timesheetEntryService.SaveChanges();
            return timesheetEntry;
        }
    }
}