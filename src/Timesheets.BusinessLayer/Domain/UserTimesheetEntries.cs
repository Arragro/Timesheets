using Arragro.Common.CacheProvider;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserTimesheetEntries
    {
        private string LastMonthsKey()
        {
            const string LastMonths = "UserTimesheetEntries:LastMonths:{0}";
            return string.Format(LastMonths, UserId);
        }

        private void ClearCache()
        {
            Cache.RemoveFromCache(LastMonthsKey(), false);
        }

        private void CacheTimesheetEntryIfInLastMonth(TimesheetEntry timesheetEntry)
        {
            List<TimesheetEntry> cachedTimesheetEntries =
                Cache.Get<List<TimesheetEntry>>(LastMonthsKey());

            if (cachedTimesheetEntries == null)
                cachedTimesheetEntries = new List<TimesheetEntry>();

            if (!cachedTimesheetEntries.Any(t => t.TimesheetEntryId == timesheetEntry.TimesheetEntryId))
                cachedTimesheetEntries.Add(timesheetEntry);
            else
            {
                cachedTimesheetEntries.Remove(timesheetEntry);
                cachedTimesheetEntries.Add(timesheetEntry);
            }
        }

        public int UserId { get; private set; }
        private readonly CacheSettings _cacheSettings;

        private readonly TimesheetEntryService _timesheetEntryService;

        public UserTimesheetEntries(
            int userId,
            CacheSettings cacheSettings,
            TimesheetEntryService timesheetEntryService)
        {
            if (cacheSettings == null) throw new ArgumentNullException("cacheSettings");
            if (timesheetEntryService == null) throw new ArgumentNullException("timesheetEntryService");

            UserId = userId;
            _cacheSettings = cacheSettings;
            _timesheetEntryService = timesheetEntryService;
        }

        public IEnumerable<TimesheetEntry> GetLastMonthsTimesheetEntries()
        {
            return Cache.Get(
                LastMonthsKey(),
                () => _timesheetEntryService.GetLastMonthsTimesheets());
        }

        public IEnumerable<TimesheetEntry> GetRangeOfTimesheetEntries(
            DateTime fromDate, DateTime toDate)
        {
            return _timesheetEntryService.GetTimesheetsByRange(fromDate, toDate);
        }

        public TimesheetEntry AddTimesheetEntry(
            TimesheetEntry timesheetEntry, IUser<int> auditUser)
        {
            timesheetEntry =
                _timesheetEntryService.ValidateAndInsertOrUpdate(timesheetEntry, auditUser.Id);
            _timesheetEntryService.SaveChanges();
            CacheTimesheetEntryIfInLastMonth(timesheetEntry);
            return timesheetEntry;
        }
    }
}