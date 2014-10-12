using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Moq;
using Timesheets.BusinessLayer.Domain;

namespace Timesheets.Tests
{
    public static class TestHelper
    {
        public static UserTimesheetEntries GetUserTimesheetEntries(int userId)
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<UserTimesheetEntries>(
                new ParameterOverride("userId", userId));
        }

        public static UserProjectAdministration GetUserProjectAdministration(int userId)
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<UserProjectAdministration>(
                new ParameterOverride("userId", userId));
        }

        public static IUser<int> GetUser(int id, string userName)
        {
            var user = new Mock<IUser<int>>();
            user.Setup(x => x.Id).Returns(id);
            user.Setup(x => x.UserName).Returns(userName);
            return user.Object;
        }

        public static IUser<int> GetFoo()
        {
            return GetUser(1, "Foo");
        }
    }
}