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
        public static IUnityContainer GetInMemoryContainer()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterInstance(new CacheSettings(new TimeSpan(0, 0, 0, 10)));

            unityContainer.RegisterType<IProjectRepository, ProjectRepository>();
            unityContainer.RegisterType<ITimesheetEntryRepository, TimesheetEntryRepository>();
            unityContainer.RegisterType<IProjectInvitationRepository, ProjectInvitationRepository>();
            unityContainer.RegisterType<IProjectContributorRepository, ProjectContributorRepository>();

            return unityContainer;
        }
    }
}