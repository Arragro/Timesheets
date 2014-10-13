using Arragro.Common.CacheProvider;
using System;
using System.Collections.Generic;
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

        public Guid UserId { get; private set; }
        private readonly CacheSettings _cacheSettings;

        private readonly TimesheetEntryService _timesheetEntryService;

        public UserTimesheetEntries(
            Guid userId,
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
                () => _timesheetEntryService.GetLastMonthsTimesheets(UserId));
        }

        public IEnumerable<TimesheetEntry> GetRangeOfTimesheetEntries(
            DateTime fromDate, DateTime toDate)
        {
            return _timesheetEntryService.GetTimesheetsByRange(UserId, fromDate, toDate);
        }

        public TimesheetEntry AddTimesheetEntry(
            TimesheetEntry timesheetEntry)
        {
            timesheetEntry =
                _timesheetEntryService.ValidateAndInsertOrUpdate(timesheetEntry, UserId);
            _timesheetEntryService.SaveChanges();
            ClearCache();
            return timesheetEntry;
        }
    }
}