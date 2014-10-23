using Arragro.Common.CacheProvider;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class UserTimesheetEntries
    {
        private string LastMonthsKey
        {
            get
            {
                const string LastMonths = "UserTimesheetEntries:LastMonths:{0}";
                return string.Format(LastMonths, User.Id);
            }
        }

        private void ClearCache()
        {
            Cache.RemoveFromCache(LastMonthsKey, false);
        }

        public IUser<Guid> User { get; private set; }
        private readonly CacheSettings _cacheSettings;

        private readonly TimesheetEntryService _timesheetEntryService;

        public UserTimesheetEntries(
            IUser<Guid> user,
            CacheSettings cacheSettings,
            TimesheetEntryService timesheetEntryService)
        {
            if (cacheSettings == null) throw new ArgumentNullException("cacheSettings");
            if (timesheetEntryService == null) throw new ArgumentNullException("timesheetEntryService");

            User = user;
            _cacheSettings = cacheSettings;
            _timesheetEntryService = timesheetEntryService;
        }

        public IEnumerable<TimesheetEntry> GetLastMonthsTimesheetEntries()
        {
            return Cache.Get(
                LastMonthsKey,
                () => _timesheetEntryService.GetLastMonthsTimesheets(User.Id),
                _cacheSettings);
        }

        public IEnumerable<TimesheetEntry> GetRangeOfTimesheetEntries(
            DateTime fromDate, DateTime toDate)
        {
            return _timesheetEntryService.GetTimesheetsByRange(User.Id, fromDate, toDate);
        }

        public TimesheetEntry AddTimesheetEntry(
            TimesheetEntry timesheetEntry)
        {
            timesheetEntry =
                _timesheetEntryService.ValidateAndInsertOrUpdate(timesheetEntry, User.Id);
            _timesheetEntryService.SaveChanges();
            ClearCache();
            return timesheetEntry;
        }
    }
}