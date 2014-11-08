using Arragro.Common.CacheProvider;
using Microsoft.Practices.Unity;
using System;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Interfaces;
using Timesheets.Persistance.EF6;
using Timesheets.Persistance.EF6.Repositories;

namespace Timesheets.Tests
{
    public static class EF6UnityContainer
    {
        public static IUnityContainer GetEF6Container(bool dropExistingDatabase = true)
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterInstance(new CacheSettings(new TimeSpan(0, 0, 0, 10)));

            TimesheetsContextExtensions.WithDbContext(x =>
            {
                if (x.Database.Exists() && dropExistingDatabase)
                    x.Database.Delete();
                x.Database.CreateIfNotExists();
            });

            // Forces repositories to use the same context to prevent object graph issues.
            unityContainer.RegisterType<TimesheetsContext, TimesheetsContext>(new ContainerControlledLifetimeManager());

            unityContainer.RegisterType<IProjectRepository, ProjectRepository>();
            unityContainer.RegisterType<ITimesheetEntryRepository, TimesheetEntryRepository>();
            unityContainer.RegisterType<IProjectInvitationRepository, ProjectInvitationRepository>();
            unityContainer.RegisterType<IProjectContributorRepository, ProjectContributorRepository>();

            return unityContainer;
        }
    }
}