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
#if !INTEGRATION_TESTS
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
#else
            var unityContainer = EF6UnityContainer.GetEF6Container();
#endif
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
                        projectService.ValidateModel(
                           new Project(
                               new String('X', 51),
                               new Guid(),
                               code: new String('X', 21),
                               purchaseOrderNumber: new String('X', 21),
                               startDate: DateTime.Now));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(5, ex.Errors.Count);
                        Assert.NotNull(ex.ContainsErrorForProperty(".Name"));
                        Assert.NotNull(ex.ContainsErrorForProperty(".Code"));
                        Assert.NotNull(ex.ContainsErrorForProperty(".PurchaseOrderNumber"));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectService.REQUIRED_OWNERUSERID));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectService.PROJECT_MUST_HAVE_BOTH_START_AND_END_DATE_SET));
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
                           new Project(
                               new String('X', 50),
                               Guid.NewGuid(),
                               startDate: DateTime.Now,
                               endDate: DateTime.Now));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(1, ex.Errors.Count);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectService.PROJECT_MUST_BE_AT_LEAST_A_DAY_LONG));
                        throw ex;
                    }
                });
        }

        [Fact]
        public void Project_fails_with_no_UserId()
        {
            var projectService = GetProjectService();

            var project1 = new Project("Test", new Guid());
            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        projectService.ValidateModel(project1);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, ProjectService.REQUIRED_OWNERUSERID);
                        throw;
                    }
                });
        }

        [Fact]
        public void Project_not_valid_when_Name_is_duplicate()
        {
            var projectService = GetProjectService();

            var userId = Guid.NewGuid();
            var project1 = new Project(
                "Test", userId,
                startDate: DateTime.Now,
                endDate: DateTime.Now.AddDays(1));

            projectService.InsertOrUpdate(project1, userId);
            projectService.SaveChanges();

            var project2 = new Project(
                "Test", userId,
                startDate: DateTime.Now,
                endDate: DateTime.Now.AddDays(1));

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
            var userId = Guid.NewGuid();
            var project = new Project(
                "Test", userId,
                startDate: DateTime.Now.AddDays(1),
                endDate: DateTime.Now);

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