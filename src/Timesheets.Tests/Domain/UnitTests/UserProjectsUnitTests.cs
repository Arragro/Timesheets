using Arragro.Common.BusinessRules;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserProjectsUnitTests
    {
        [Fact]
        public void Project_update_fails_when_project_has_not_been_saved()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var userProjectAdministration = testHelper.GetUserProjects(TestHelper.GetFoo());

                Assert.Throws<RulesException<Project>>(
                    () =>
                    {
                        try
                        {
                            userProjectAdministration.UpdateProject(
                                new Project("Test", Guid.NewGuid()));
                        }
                        catch (RulesException ex)
                        {
                            Assert.Equal(ex.Errors[0].Message, UserProjects.PROJECT_HAS_NOT_BEEN_SAVED);
                            throw ex;
                        }
                    });
            }
        }

        [Fact]
        public void Project_cannot_be_modified_by_unauthorised_user()
        {
            var fooUser = TestHelper.GetFoo();

            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            var projectService = unityContainer.Resolve<ProjectService>();
            var userProjectAdministration = unityContainer.Resolve<UserProjects>(
                new ParameterOverride("user", fooUser),
                new ParameterOverride("projectService", projectService));

            Assert.Throws<RulesException>(
                () =>
                {
                    try
                    {
                        var project = userProjectAdministration.AddProject(
                            new Project("Test", Guid.NewGuid()));

                        var tempUserProjectAdministration =
                            unityContainer.Resolve<UserProjects>(
                                new ParameterOverride("user", TestHelper.GetBar()),
                                new ParameterOverride("projectService", projectService));

                        tempUserProjectAdministration.UpdateProject(project);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, SecurityRules.USER_IS_NOT_AUTHORISED_TO_MODIFY);
                        throw ex;
                    }
                });
        }

        [Fact]
        public void Project_OwnerUserId_is_same_as_creator()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var userProjectAdministration = testHelper.GetUserProjects(fooUser);

                var project = userProjectAdministration.AddProject(
                    new Project(
                        "Test", Guid.NewGuid(),
                        startDate: DateTime.Now,
                        endDate: DateTime.Now.AddDays(1)));

                Assert.Equal(project.OwnerUserId, fooUser.Id);
            }
        }

        [Fact]
        public void Project_transfer_fails_when_user_is_not_owner()
        {
            var fooUser = TestHelper.GetFoo();

            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            var projectService = unityContainer.Resolve<ProjectService>();
            var userProjectAdministration = unityContainer.Resolve<UserProjects>(
                new ParameterOverride("user", fooUser),
                new ParameterOverride("projectService", projectService));

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        var project = userProjectAdministration.AddProject(
                            new Project(
                                "Test", Guid.NewGuid(),
                                startDate: DateTime.Now,
                                endDate: DateTime.Now.AddDays(1)));

                        var tempUserProjectAdministration = unityContainer.Resolve<UserProjects>(
                            new ParameterOverride("user", TestHelper.GetBar()),
                            new ParameterOverride("projectService", projectService));

                        tempUserProjectAdministration.TransferProjectOwnership(
                            project, Guid.NewGuid());
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, UserProjects.USER_IS_NOT_THE_PRESENT_OWNER);
                        throw ex;
                    }
                });
        }

        [Fact]
        public void Project_transfer_ownership_succeeds_when_owner_initiates()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var userProjectAdministration = testHelper.GetUserProjects(TestHelper.GetFoo());

                var project = userProjectAdministration.AddProject(
                    new Project(
                        "Test", Guid.NewGuid(),
                        startDate: DateTime.Now,
                        endDate: DateTime.Now.AddDays(1)));

                var newOwner = Guid.NewGuid();
                project = userProjectAdministration.TransferProjectOwnership(
                    project, newOwner);

                Assert.Equal(project.OwnerUserId, newOwner);
            }
        }

        [Fact]
        public void get_User_Projects()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var userProjectAdministration = testHelper.GetUserProjects(TestHelper.GetFoo());

                DomainObjectBuilder.LoadProjects(userProjectAdministration, Guid.NewGuid());

                var userProjects = userProjectAdministration.GetUserProjects();
                Assert.Equal(2, userProjects.Count());
            }
        }

        [Fact]
        public void get_User_Projects_only_returns_that_Users()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var fooUserProjectAdministration = testHelper.GetUserProjects(TestHelper.GetFoo());

                var barUser = TestHelper.GetBar();
                var barUserProjectAdministration = testHelper.GetUserProjects(TestHelper.GetFoo());

                DomainObjectBuilder.LoadProjects(fooUserProjectAdministration, fooUser.Id);
                DomainObjectBuilder.LoadProjects(barUserProjectAdministration, fooUser.Id);

                var userProjects = barUserProjectAdministration.GetUserProjects();
                Assert.Equal(2, userProjects.Count());
            }
        }
    }
}