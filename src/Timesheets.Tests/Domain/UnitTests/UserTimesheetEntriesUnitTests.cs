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
                var timesheetEntry = new TimesheetEntry(
                    user.Id, DateTime.Now.Date.AddDays(0 - counter),
                    8, "Foo Entry");
                userTimesheetEntries.AddTimesheetEntry(timesheetEntry);
                counter++;
            }
        }

        [Fact]
        public void UserTimesheetEntries_instantiation_throws_error_when_null_services_supplied()
        {
            using (var testHelper = new TestHelper())
            {
                Assert.Throws<ArgumentNullException>(
                    () =>
                    {
                        try
                        {
                            new UserTimesheetEntries(TestHelper.GetFoo(), null, null);
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
                                TestHelper.GetFoo(), testHelper.GetCacheSettings(), null);
                        }
                        catch (ArgumentNullException ex)
                        {
                            Assert.Equal(ex.ParamName, "timesheetEntryService");
                            throw ex;
                        }
                    });
            }
        }

        [Fact]
        public void add_User_TimesheetEntry()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var userTimeSheetEntries = testHelper.GetUserTimesheetEntries(fooUser);

                var timesheetEntry = new TimesheetEntry(
                    fooUser.Id, DateTime.Now.Date, 8, "Foo Entry");

                timesheetEntry = userTimeSheetEntries.AddTimesheetEntry(timesheetEntry);
                Assert.NotEqual(default(Guid), timesheetEntry.TimesheetEntryId);
            }
        }

        [Fact]
        public void get_User_last_months_worth_of_TimesheetEntries()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var userTimeSheetEntries = testHelper.GetUserTimesheetEntries(fooUser);
                LoadTimeSheetEntries(userTimeSheetEntries, fooUser);

                var timesheetEntries = userTimeSheetEntries.GetLastMonthsTimesheetEntries();
                Assert.Equal(31, timesheetEntries.Count());
            }
        }

        [Fact]
        public void get_User_range_of_TimesheetEntries()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var userTimeSheetEntries = testHelper.GetUserTimesheetEntries(fooUser);
                LoadTimeSheetEntries(userTimeSheetEntries, fooUser);

                var timesheetEntries =
                    userTimeSheetEntries.GetRangeOfTimesheetEntries(
                        DateTime.Now.AddDays(-10), DateTime.Now);
                Assert.Equal(10, timesheetEntries.Count());
            }
        }

        [Fact]
        public void get_User_TimesheetEntries_only_return_that_Users()
        {
            using (var testHelper = new TestHelper())
            {
                var numberOfUserTimesheets = 5;

                var fooUser = TestHelper.GetFoo();
                var fooUserTimeSheetEntries = testHelper.GetUserTimesheetEntries(fooUser);
                LoadTimeSheetEntries(fooUserTimeSheetEntries, fooUser, numberOfUserTimesheets);

                var barUser = TestHelper.GetBar();
                var barUserTimeSheetEntries = testHelper.GetUserTimesheetEntries(barUser);
                LoadTimeSheetEntries(barUserTimeSheetEntries, barUser, numberOfUserTimesheets);

                Assert.Equal(numberOfUserTimesheets,
                    fooUserTimeSheetEntries.GetLastMonthsTimesheetEntries().Count());
                Assert.Equal(numberOfUserTimesheets,
                    barUserTimeSheetEntries.GetRangeOfTimesheetEntries(
                        DateTime.Now.AddDays(0 - numberOfUserTimesheets), DateTime.Now).Count());
            }
        }
    }
}