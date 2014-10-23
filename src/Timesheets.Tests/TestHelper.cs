using Arragro.Common.CacheProvider;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Moq;
using System;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;

namespace Timesheets.Tests
{
    public static class TestHelper
    {
        static TestHelper()
        {
            UnityContainer(true);
        }

        public const string VALID_EMAIL_ADDRESS = "email.is.good@test.com";

        public static IUnityContainer UnityContainer(bool dropDatabase = false)
        {
#if !INTEGRATION_TESTS
            return InMemoryUnityContainer.GetInMemoryContainer();
#else
            return EF6UnityContainer.GetEF6Container(dropDatabase);
#endif
        }

        public static ProjectService GetProjectService()
        {
            return UnityContainer().Resolve<ProjectService>();
        }

        public static TimesheetEntryService GetTimesheetEntryService()
        {
            return UnityContainer().Resolve<TimesheetEntryService>();
        }

        public static ProjectInvitationService GetProjectInvitationService()
        {
            return UnityContainer().Resolve<ProjectInvitationService>();
        }

        public static ProjectContributorService GetProjectContributorService()
        {
            return UnityContainer().Resolve<ProjectContributorService>();
        }

        public static UserTimesheetEntries GetUserTimesheetEntries(IUser<Guid> user)
        {
            return UnityContainer(false).Resolve<UserTimesheetEntries>(
                new ParameterOverride("user", user));
        }

        public static UserProjects GetUserProjectAdministration(IUser<Guid> user)
        {
            return UnityContainer(false).Resolve<UserProjects>(
                new ParameterOverride("user", user));
        }

        public static CacheSettings GetCacheSettings()
        {
            return UnityContainer(false).Resolve<CacheSettings>();
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