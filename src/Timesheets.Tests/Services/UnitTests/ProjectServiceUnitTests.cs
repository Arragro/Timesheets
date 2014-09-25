using Arragro.Common.BusinessRules;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Services.UnitTests
{
    public class ProjectServiceUnitTests
    {
        [Fact]
        public void ModelAttributesValidateCorrectly()
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            var projectService = unityContainer.Resolve<ProjectService>();

            var errors = projectService.ValidateModel(new Project());
            Assert.Equal(1, errors.Count);

            errors = projectService.ValidateModel(
                        new Project
                        {
                            Name = new String('X', 51),
                            Code = new String('X', 21),
                            PurchaseOrderNumber = new String('X', 21)
                        });
            Assert.Equal(3, errors.Count);
            Assert.NotNull(errors.SingleOrDefault(x => x.MemberNames.Contains("Name")));
            Assert.NotNull(errors.SingleOrDefault(x => x.MemberNames.Contains("Code")));
            Assert.NotNull(errors.SingleOrDefault(x => x.MemberNames.Contains("PurchaseOrderNumber")));
        }

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