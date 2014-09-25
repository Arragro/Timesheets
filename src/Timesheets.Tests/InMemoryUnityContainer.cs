using Microsoft.Practices.Unity;
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
            unityContainer.RegisterType<IProjectRepository, ProjectRepository>();
            unityContainer.RegisterType<ProjectService, ProjectService>();
            return unityContainer;
        }
    }
}