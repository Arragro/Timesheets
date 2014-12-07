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
                            new UserTimesheetEntries(TestHelper.GetOwnerUser(), null, null);
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
                                TestHelper.GetOwnerUser(), testHelper.GetCacheSettings(), null);
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
                var ownerUser = TestHelper.GetOwnerUser();
                var userTimeSheetEntries = testHelper.GetUserTimesheetEntries(ownerUser);

                var timesheetEntry = new TimesheetEntry(
                    ownerUser.Id, DateTime.Now.Date, 8, "Foo Entry");

                timesheetEntry = userTimeSheetEntries.AddTimesheetEntry(timesheetEntry);
                Assert.NotEqual(default(Guid), timesheetEntry.TimesheetEntryId);
            }
        }

        [Fact]
        public void get_User_last_months_worth_of_TimesheetEntries()
        {
            using (var testHelper = new TestHelper())
            {
                var ownerUser = TestHelper.GetOwnerUser();
                var userTimeSheetEntries = testHelper.GetUserTimesheetEntries(ownerUser);
                DomainObjectBuilder.LoadTimeSheetEntries(userTimeSheetEntries, ownerUser);

                var timesheetEntries = userTimeSheetEntries.GetLastMonthsTimesheetEntries();
                Assert.Equal(30, timesheetEntries.Count());
            }
        }

        [Fact]
        public void get_User_range_of_TimesheetEntries()
        {
            using (var testHelper = new TestHelper())
            {
                var ownerUser = TestHelper.GetOwnerUser();
                var userTimeSheetEntries = testHelper.GetUserTimesheetEntries(ownerUser);
                DomainObjectBuilder.LoadTimeSheetEntries(userTimeSheetEntries, ownerUser);

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

                var ownerUser = TestHelper.GetOwnerUser();
                var ownerUserTimeSheetEntries = testHelper.GetUserTimesheetEntries(ownerUser);
                DomainObjectBuilder.LoadTimeSheetEntries(ownerUserTimeSheetEntries, ownerUser, numberOfTimesheets: numberOfUserTimesheets);

                var user = TestHelper.GetUser(TestHelper.VALID_EMAIL_ADDRESS);
                var userTimeSheetEntries = testHelper.GetUserTimesheetEntries(user);
                DomainObjectBuilder.LoadTimeSheetEntries(userTimeSheetEntries, user, numberOfTimesheets: numberOfUserTimesheets);

                Assert.Equal(numberOfUserTimesheets,
                    ownerUserTimeSheetEntries.GetLastMonthsTimesheetEntries().Count());
                Assert.Equal(numberOfUserTimesheets,
                    userTimeSheetEntries.GetRangeOfTimesheetEntries(
                        DateTime.Now.AddDays(0 - numberOfUserTimesheets), DateTime.Now).Count());
            }
        }
    }
}