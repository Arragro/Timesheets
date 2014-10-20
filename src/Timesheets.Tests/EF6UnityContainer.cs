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
        public static IUnityContainer GetEF6Container()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterInstance(new CacheSettings(new TimeSpan(0, 0, 0, 10)));

            TimesheetsContextExtensions.WithDbContext(x =>
            {
                if (x.Database.Exists())
                    x.Database.Delete();
                x.Database.CreateIfNotExists();
            });

            unityContainer.RegisterType<IProjectRepository, ProjectRepository>();
            unityContainer.RegisterType<ITimesheetEntryRepository, TimesheetEntryRepository>();
            unityContainer.RegisterType<IProjectInvitationRepository, ProjectInvitationRepository>();

            return unityContainer;
        }
    }
}