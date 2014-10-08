using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Moq;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserTimesheetEntriesUnitTests
    {
        private UserTimesheetEntries GetTimesheetEntryService(int userId)
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<UserTimesheetEntries>(
                new ParameterOverride("userId", userId));
        }

        private IUser<int> GetUser(int id, string userName)
        {
            var user = new Mock<IUser<int>>();
            user.Setup(x => x.Id).Returns(id);
            user.Setup(x => x.UserName).Returns(userName);
            return user.Object;
        }

        private IUser<int> GetFoo()
        {
            return GetUser(1, "Foo");
        }

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
                userTimesheetEntries.AddTimesheetEntry(timesheetEntry, user);
                counter++;
            }
        }

        [Fact]
        public void add_User_TimesheetEntry()
        {
            var fooUser = GetFoo();
            var userTimeSheetEntries = GetTimesheetEntryService(fooUser.Id);

            var timesheetEntry = new TimesheetEntry
            {
                UserId = fooUser.Id,
                Date = DateTime.Now.Date,
                Description = "Foo Entry",
                NumberOfHours = 8
            };

            timesheetEntry = userTimeSheetEntries.AddTimesheetEntry(timesheetEntry, fooUser);
            Assert.NotEqual(default(Guid), timesheetEntry.TimesheetEntryId);
        }

        [Fact]
        public void get_User_last_months_worth_of_TimesheetEntries()
        {
            var fooUser = GetFoo();
            var userTimeSheetEntries = GetTimesheetEntryService(fooUser.Id);
            Load100TimeSheetEntries(userTimeSheetEntries, fooUser);

            var timesheetEntries = userTimeSheetEntries.GetLastMonthsTimesheetEntries();
            Assert.Equal(30, timesheetEntries.Count());
        }

        [Fact]
        public void get_User_range_of_TimesheetEntries()
        {
            var fooUser = GetFoo();
            var userTimeSheetEntries = GetTimesheetEntryService(fooUser.Id);
            Load100TimeSheetEntries(userTimeSheetEntries, fooUser);

            var timesheetEntries =
                userTimeSheetEntries.GetRangeOfTimesheetEntries(
                    DateTime.Now.AddDays(-9), DateTime.Now);
            Assert.Equal(10, timesheetEntries.Count());
        }
    }
}