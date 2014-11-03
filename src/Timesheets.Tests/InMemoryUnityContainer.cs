using Arragro.Common.CacheProvider;
using Microsoft.Practices.Unity;
using System;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.InMemoryRepositories;
using Timesheets.DataLayer.Interfaces;

namespace Timesheets.Tests
{
    public static class InMemoryUnityContainer
    {
        private static Lazy<IUnityContainer> LazyUnityContainer = new Lazy<IUnityContainer>(() => new UnityContainer(), true);
        private static object _locker = new object();
        private static bool _containerSet = false;

        public static IUnityContainer GetInMemoryContainer()
        {
            var unityContainer = LazyUnityContainer.Value;

            if (!_containerSet)
            {
                lock (_locker)
                {
                    if (!_containerSet)
                    {
                        unityContainer.RegisterInstance(new CacheSettings(new TimeSpan(0, 0, 0, 10)));

                        unityContainer.RegisterType<IProjectRepository, ProjectRepository>(new ContainerControlledLifetimeManager());
                        unityContainer.RegisterType<ITimesheetEntryRepository, TimesheetEntryRepository>(new ContainerControlledLifetimeManager());
                        unityContainer.RegisterType<IProjectInvitationRepository, ProjectInvitationRepository>(new ContainerControlledLifetimeManager());
                        unityContainer.RegisterType<IProjectContributorRepository, ProjectContributorRepository>(new ContainerControlledLifetimeManager());
                        _containerSet = true;
                    }
                }
            }

            return unityContainer;
        }
    }
}