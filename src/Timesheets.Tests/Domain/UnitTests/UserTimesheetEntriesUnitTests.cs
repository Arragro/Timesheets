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
        private void LoadTimeSheetEntries(
            UserTimesheetEntries userTimesheetEntries,
            IUser<Guid> user,
            int numberOfTimesheets = 31)
        {
            var counter = 0;
            while (counter < numberOfTimesheets)
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
        public void UserTimesheetEntries_instantiation_throws_error_when_null_services_supplied()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new UserTimesheetEntries(Guid.NewGuid(), null, null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal(ex.ParamName, "cacheSettings");
                        throw ex;
                    }
                });
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new UserTimesheetEntries(
                            Guid.NewGuid(), TestHelper.GetCacheSettings(), null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal(ex.ParamName, "timesheetEntryService");
                        throw ex;
                    }
                });
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
            LoadTimeSheetEntries(userTimeSheetEntries, fooUser);

            var timesheetEntries = userTimeSheetEntries.GetLastMonthsTimesheetEntries();
            Assert.Equal(30, timesheetEntries.Count());
        }

        [Fact]
        public void get_User_range_of_TimesheetEntries()
        {
            var fooUser = TestHelper.GetFoo();
            var userTimeSheetEntries = TestHelper.GetUserTimesheetEntries(fooUser.Id);
            LoadTimeSheetEntries(userTimeSheetEntries, fooUser);

            var timesheetEntries =
                userTimeSheetEntries.GetRangeOfTimesheetEntries(
                    DateTime.Now.AddDays(-9), DateTime.Now);
            Assert.Equal(10, timesheetEntries.Count());
        }

        [Fact]
        public void get_User_TimesheetEntries_only_return_that_Users()
        {
            var numberOfUserTimesheets = 5;

            var fooUser = TestHelper.GetFoo();
            var fooUserTimeSheetEntries = TestHelper.GetUserTimesheetEntries(fooUser.Id);
            LoadTimeSheetEntries(fooUserTimeSheetEntries, fooUser, numberOfUserTimesheets);

            var barUser = TestHelper.GetBar();
            var barUserTimeSheetEntries = TestHelper.GetUserTimesheetEntries(barUser.Id);
            LoadTimeSheetEntries(barUserTimeSheetEntries, barUser, numberOfUserTimesheets);

            Assert.Equal(numberOfUserTimesheets,
                fooUserTimeSheetEntries.GetLastMonthsTimesheetEntries().Count());
            Assert.Equal(numberOfUserTimesheets,
                barUserTimeSheetEntries.GetRangeOfTimesheetEntries(
                    DateTime.Now.AddDays(0 - numberOfUserTimesheets), DateTime.Now).Count());
        }
    }
}