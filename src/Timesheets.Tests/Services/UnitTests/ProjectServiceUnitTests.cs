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
        private ProjectService GetProjectService()
        {
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<ProjectService>();
        }

        [Fact]
        public void Project_attributes_validate_correctly()
        {
            var projectService = GetProjectService();

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
        public void Project_fails_with_no_UserId()
        {
            var projectService = GetProjectService();

            var project1 = new Project { Name = "Test" };
            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.InsertOrUpdate(project1, 1);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, ProjectService.REQUIRED_USERID);
                        throw;
                    }
                });
        }

        [Fact]
        public void Project_not_valid_when_Name_is_duplicate()
        {
            var projectService = GetProjectService();

            var userId = 1;
            var project1 = new Project
            {
                Name = "Test",
                UserId = userId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };

            projectService.InsertOrUpdate(project1, userId);

            var project2 = new Project
            {
                Name = "Test",
                UserId = userId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };
            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.InsertOrUpdate(project2, userId);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, string.Format(ProjectService.DUPLICATE_NAME_FOR_USER, project2.Name));
                        throw;
                    }
                });
        }

        [Fact]
        public void Project_fails_on_invalid_start_end_dates()
        {
            var projectService = GetProjectService();
            var userId = 1;
            var project = new Project
            {
                Name = "Test",
                UserId = userId,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now
            };

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.InsertOrUpdate(project, userId);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, ProjectService.PROJECT_MUST_BE_AT_LEAST_A_DAY_LONG);
                        throw ex;
                    }
                });
        }
    }
}