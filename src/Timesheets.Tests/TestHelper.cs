using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Moq;
using System;
using Timesheets.BusinessLayer.Domain;

namespace Timesheets.Tests
{
    public static class TestHelper
    {
        public static UserTimesheetEntries GetUserTimesheetEntries(Guid userId)
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<UserTimesheetEntries>(
                new ParameterOverride("userId", userId));
        }

        public static UserProjectAdministration GetUserProjectAdministration(Guid userId)
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<UserProjectAdministration>(
                new ParameterOverride("userId", userId));
        }

        public static IUser<Guid> GetUser(Guid id, string userName)
        {
            var user = new Mock<IUser<Guid>>();
            user.Setup(x => x.Id).Returns(id);
            user.Setup(x => x.UserName).Returns(userName);
            return user.Object;
        }

        public static IUser<Guid> GetFoo()
        {
            return GetUser(Guid.NewGuid(), "Foo");
        }
    }
}