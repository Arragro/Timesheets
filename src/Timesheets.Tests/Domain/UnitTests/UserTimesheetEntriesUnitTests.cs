using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Domain;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserTimesheetEntriesUnitTests
    {
        private void Load100TimeSheetEntries(
            UserTimesheetEntries userTimesheetEntries,
            IUser<int> user)
        {
            var counter = 0;
            while (counter < 31)
            {
                var timesheetEntry = new TimesheetEntry
                {
                    UserId = user.Id,
                    Date = DateTime.Now.Date.AddDays(0 - counter),
                    Description = "Foo Entry",
                    NumberOfHours = 8
                };
                userTimesheetEntries.AddTimesheetEntry(timesheetEntry);
                counter++;
            }
        }

        [Fact]
        public void add_User_TimesheetEntry()
        {
            var fooUser = TestHelper.GetFoo();
            var userTimeSheetEntries = TestHelper.GetUserTimesheetEntries(fooUser.Id);

            var timesheetEntry = new TimesheetEntry
            {
                UserId = fooUser.Id,
                Date = DateTime.Now.Date,
                Description = "Foo Entry",
                NumberOfHours = 8
            };

            timesheetEntry = userTimeSheetEntries.AddTimesheetEntry(timesheetEntry);
            Assert.NotEqual(default(Guid), timesheetEntry.TimesheetEntryId);
        }

        [Fact]
        public void get_User_last_months_worth_of_TimesheetEntries()
        {
            var fooUser = TestHelper.GetFoo();
            var userTimeSheetEntries = TestHelper.GetUserTimesheetEntries(fooUser.Id);
            Load100TimeSheetEntries(userTimeSheetEntries, fooUser);

            var timesheetEntries = userTimeSheetEntries.GetLastMonthsTimesheetEntries();
            Assert.Equal(30, timesheetEntries.Count());
        }

        [Fact]
        public void get_User_range_of_TimesheetEntries()
        {
            var fooUser = TestHelper.GetFoo();
            var userTimeSheetEntries = TestHelper.GetUserTimesheetEntries(fooUser.Id);
            Load100TimeSheetEntries(userTimeSheetEntries, fooUser);

            var timesheetEntries =
                userTimeSheetEntries.GetRangeOfTimesheetEntries(
                    DateTime.Now.AddDays(-9), DateTime.Now);
            Assert.Equal(10, timesheetEntries.Count());
        }
    }
}