using Arragro.Common.CacheProvider;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Moq;
using System;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;

namespace Timesheets.Tests
{
    // Test helper is IDisposable, the DbContext is going to exist per request on a web application
    // and for the lifetime of a series of calls.
    public class TestHelper : IDisposable
    {
        private IUnityContainer _unityContainer;

        public TestHelper(bool dropDatabase = false)
        {
#if !INTEGRATION_TESTS
            _unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
#else
            _unityContainer = EF6UnityContainer.GetEF6Container(dropDatabase);
#endif
        }

        public const string VALID_EMAIL_ADDRESS = "email.is.good@test.com";

        public static string GetEmailAddress(int number)
        {
            const string valid_email_address = "email.is.good{0}@test.com";
            return string.Format(valid_email_address, number);
        }

        public ProjectService GetProjectService()
        {
            return _unityContainer.Resolve<ProjectService>();
        }

        public TimesheetEntryService GetTimesheetEntryService()
        {
            return _unityContainer.Resolve<TimesheetEntryService>();
        }

        public ProjectInvitationService GetProjectInvitationService()
        {
            return _unityContainer.Resolve<ProjectInvitationService>();
        }

        public ProjectContributorService GetProjectContributorService()
        {
            return _unityContainer.Resolve<ProjectContributorService>();
        }

        public BackEndAdministration GetBackEndAdministration()
        {
            return _unityContainer.Resolve<BackEndAdministration>();
        }

        public UserTimesheetEntries GetUserTimesheetEntries(IUser<Guid> user)
        {
            return _unityContainer.Resolve<UserTimesheetEntries>(
                new ParameterOverride("user", user));
        }

        public UserProjects GetUserProjects(IUser<Guid> user)
        {
            return _unityContainer.Resolve<UserProjects>(
                new ParameterOverride("user", user));
        }

        public UserProjectInvitations GetUserProjectInvitations(IUser<Guid> user)
        {
            return _unityContainer.Resolve<UserProjectInvitations>(
                new ParameterOverride("user", user));
        }

        public UserProjectContributor GetUserProjectContributor(IUser<Guid> user)
        {
            return _unityContainer.Resolve<UserProjectContributor>(
                new ParameterOverride("user", user));
        }

        public CacheSettings GetCacheSettings()
        {
            return _unityContainer.Resolve<CacheSettings>();
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

        public static IUser<Guid> GetBoo()
        {
            return GetUser(Guid.NewGuid(), "Boo");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unityContainer = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}