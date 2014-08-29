using Arragro.Common.BusinessRules;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.InMemoryRepositories;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Services.UnitTests
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

    public class ProjectServiceUnitTests
    {
        [Fact]
        public void NoUserIdForProject()
        {
            var userId = Guid.NewGuid();
            var project1 = new Project { Name = "Test" };

            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            var projectService = unityContainer.Resolve<ProjectService>();

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.InsertOrUpdate(project1, userId);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, ProjectService.REQUIREDUSERID);
                        throw;
                    }
                });
        }

        [Fact]
        public void DuplicateProjectNameForUserFails()
        {
            var userId = Guid.NewGuid();
            var project1 = new Project { Name = "Test", UserId = userId };

            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();

            var projectService = unityContainer.Resolve<ProjectService>();
            projectService.InsertOrUpdate(project1, userId);

            var project2 = new Project { Name = "Test", UserId = userId };
            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.InsertOrUpdate(project2, userId);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, string.Format(ProjectService.DUPLICATENAMEFORUSER, project2.Name));
                        throw;
                    }
                });
        }
    }
}
