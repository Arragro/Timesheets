using Arragro.Common.CacheProvider;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Moq;
using System;
using Timesheets.BusinessLayer.Domain;

namespace Timesheets.Tests
{
    public static class TestHelper
    {
        public const string VALID_EMAIL_ADDRESS = "email.is.good@test.com";

        public static IUnityContainer UnityContainer
        {
            get
            {
                return InMemoryUnityContainer.GetInMemoryContainer();
            }
        }

        public static UserTimesheetEntries GetUserTimesheetEntries(Guid userId)
        {
            return UnityContainer.Resolve<UserTimesheetEntries>(
                new ParameterOverride("userId", userId));
        }

        public static UserProjectAdministration GetUserProjectAdministration(Guid userId)
        {
            return UnityContainer.Resolve<UserProjectAdministration>(
                new ParameterOverride("userId", userId));
        }

        public static CacheSettings GetCacheSettings()
        {
            return UnityContainer.Resolve<CacheSettings>();
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

        public static IUser<Guid> GetBar()
        {
            return GetUser(Guid.NewGuid(), "Bar");
        }
    }
}