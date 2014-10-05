using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Moq;
using System;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain
{
    public class UserTimesheetEntriesUseCase
    {
        private TimesheetEntryService GetTimesheetEntryService()
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<TimesheetEntryService>();
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

        [Fact]
        public void add_User_TimesheetEntry()
        {
            var timesheetEntryService = GetTimesheetEntryService();
            var userTimeSheetEntries = new UserTimesheetEntries(timesheetEntryService);

            var fooUser = GetFoo();
            var timesheetEntry = new TimesheetEntry
            {
                UserId = fooUser.Id,
                Date = DateTime.Now.Date,
                Description = "Foo Entry",
                NumberOfHours = 8
            };

            timesheetEntryService.ValidateAndInsertOrUpdate(
                timesheetEntry, fooUser.Id);
            timesheetEntryService.SaveChanges();

            Assert.NotEqual(default(Guid), timesheetEntry.TimesheetEntryId);
        }
    }
}