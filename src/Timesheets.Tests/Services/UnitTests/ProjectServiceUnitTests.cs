using Arragro.Common.BusinessRules;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.ValidateModel(new Project());
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(3, ex.Errors.Count);
                        throw ex;
                    }
                });

            projectService.RulesException.Errors.Clear();

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.ValidateModel(
                           new Project
                           {
                               Name = new String('X', 51),
                               Code = new String('X', 21),
                               PurchaseOrderNumber = new String('X', 21)
                           });
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(5, ex.Errors.Count);
                        Assert.NotNull(ex.ContainsErrorForProperty(".Name"));
                        Assert.NotNull(ex.ContainsErrorForProperty(".Code"));
                        Assert.NotNull(ex.ContainsErrorForProperty(".PurchaseOrderNumber"));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectService.REQUIRED_USERID));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectService.PROJECT_MUST_BE_AT_LEAST_A_DAY_LONG));
                        throw ex;
                    }
                });
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
                        projectService.ValidateModel(project1);
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
                        projectService.ValidateModel(project2);
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
                        projectService.ValidateModel(project);
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