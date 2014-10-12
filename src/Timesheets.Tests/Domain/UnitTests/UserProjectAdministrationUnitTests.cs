using Arragro.Common.BusinessRules;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Domain;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserProjectAdministrationUnitTests
    {
        [Fact]
        public void UserProjectAdministrion_instantiation_throws_error_when_null_services_supplied()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new UserProjectAdministration(1, null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal(ex.ParamName, "projectService");
                        throw ex;
                    }
                });
        }

        [Fact]
        public void Project_update_fails_when_project_has_not_been_saved()
        {
            var fooUser = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(fooUser.Id);

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        userProjectAdministration.UpdateProject(
                            new Project
                            {
                                Name = "Test",
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(1)
                            });
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, UserProjectAdministration.PROJECT_HAS_NOT_BEEN_SAVED);
                        throw ex;
                    }
                });
        }

        [Fact]
        public void Project_cannot_be_modified_by_unauthorised_user()
        {
            var fooUser = TestHelper.GetFoo();

            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            var projectService = unityContainer.Resolve<ProjectService>();
            var userProjectAdministration = unityContainer.Resolve<UserProjectAdministration>(
                new ParameterOverride("userId", fooUser.Id),
                new ParameterOverride("projectService", projectService));

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        var project = userProjectAdministration.AddProject(
                            new Project
                            {
                                Name = "Test",
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(1)
                            });

                        var tempUserProjectAdministration =
                            unityContainer.Resolve<UserProjectAdministration>(
                                new ParameterOverride("userId", fooUser.Id + 1),
                                new ParameterOverride("projectService", projectService));

                        tempUserProjectAdministration.UpdateProject(project);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, UserProjectAdministration.USER_IS_NOT_AUTHORISED);
                        throw ex;
                    }
                });
        }

        [Fact]
        public void Project_OwnerUserId_is_same_as_creator()
        {
            var fooUser = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(fooUser.Id);

            var project = userProjectAdministration.AddProject(
                new Project
                {
                    Name = "Test",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    OwnerUserId = 6
                });

            Assert.Equal(project.OwnerUserId, fooUser.Id);
        }

        [Fact]
        public void Project_transfer_fails_when_user_is_not_owner()
        {
            var fooUser = TestHelper.GetFoo();

            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            var projectService = unityContainer.Resolve<ProjectService>();
            var userProjectAdministration = unityContainer.Resolve<UserProjectAdministration>(
                new ParameterOverride("userId", fooUser.Id),
                new ParameterOverride("projectService", projectService));

            Assert.Throws<RulesException<Project>>(
                () =>
                {
                    try
                    {
                        var project = userProjectAdministration.AddProject(
                            new Project
                            {
                                Name = "Test",
                                StartDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(1)
                            });

                        var tempUserProjectAdministration = unityContainer.Resolve<UserProjectAdministration>(
                            new ParameterOverride("userId", fooUser.Id + 1),
                            new ParameterOverride("projectService", projectService));

                        tempUserProjectAdministration.TransferProjectOwnership(
                            project, fooUser.Id + 1);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, UserProjectAdministration.USER_IS_NOT_AUTHORISED);
                        throw ex;
                    }
                });
        }

        [Fact]
        public void Project_transfer_ownership_succeeds_when_owner_initiates()
        {
            var fooUser = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(fooUser.Id);

            var project = userProjectAdministration.AddProject(
                new Project
                {
                    Name = "Test",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                });

            project = userProjectAdministration.TransferProjectOwnership(
                project, 2);

            Assert.Equal(project.OwnerUserId, 2);
        }

        [Fact]
        public void get_User_Projects()
        {
            var fooUser = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(fooUser.Id);

            userProjectAdministration.AddProject(
                new Project
                {
                    Name = "Test 1",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                });

            userProjectAdministration.AddProject(
                new Project
                {
                    Name = "Test 2",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                });

            var userProjects = userProjectAdministration.GetUsersProjects();
            Assert.Equal(2, userProjects.Count());
        }
    }
}