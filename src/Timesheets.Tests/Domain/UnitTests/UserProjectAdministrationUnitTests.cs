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
    public class UserProjectAdministrationUnitTests
    {
        private void LoadProjects(
            UserProjectAdministration userProjectAdministration,
            Guid ownerUserId,
            int numberOfProjects = 2)
        {
            var counter = 0;
            while (counter < numberOfProjects)
            {
                var project = new Project(
                    "Test " + (counter + 1).ToString(),
                    ownerUserId,
                    startDate: DateTime.Now,
                    endDate: DateTime.Now.AddDays(1));

                userProjectAdministration.AddProject(project);
                counter++;
            }
        }

        [Fact]
        public void UserProjectAdministrion_instantiation_throws_error_when_null_services_supplied()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new UserProjectAdministration(Guid.NewGuid(), null, null, null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal(ex.ParamName, "cacheSettings");
                        throw ex;
                    }
                });
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new UserProjectAdministration(
                            Guid.NewGuid(), TestHelper.GetCacheSettings(), null, null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal(ex.ParamName, "projectService");
                        throw ex;
                    }
                });
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new UserProjectAdministration(
                            Guid.NewGuid(), TestHelper.GetCacheSettings(),
                            TestHelper.UnityContainer.Resolve<ProjectService>(), null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal(ex.ParamName, "projectInvitationService");
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
                            new Project("Test", Guid.NewGuid()));
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
                            new Project("Test", Guid.NewGuid()));

                        var tempUserProjectAdministration =
                            unityContainer.Resolve<UserProjectAdministration>(
                                new ParameterOverride("userId", Guid.NewGuid()),
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
                new Project(
                    "Test", Guid.NewGuid(),
                    startDate: DateTime.Now,
                    endDate: DateTime.Now.AddDays(1)));

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
                            new Project(
                                "Test", Guid.NewGuid(),
                                startDate: DateTime.Now,
                                endDate: DateTime.Now.AddDays(1)));

                        var tempUserProjectAdministration = unityContainer.Resolve<UserProjectAdministration>(
                            new ParameterOverride("userId", Guid.NewGuid()),
                            new ParameterOverride("projectService", projectService));

                        tempUserProjectAdministration.TransferProjectOwnership(
                            project, Guid.NewGuid());
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
                new Project(
                    "Test", Guid.NewGuid(),
                    startDate: DateTime.Now,
                    endDate: DateTime.Now.AddDays(1)));

            var newOwner = Guid.NewGuid();
            project = userProjectAdministration.TransferProjectOwnership(
                project, newOwner);

            Assert.Equal(project.OwnerUserId, newOwner);
        }

        [Fact]
        public void get_User_Projects()
        {
            var fooUser = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(fooUser.Id);

            LoadProjects(userProjectAdministration, Guid.NewGuid());

            var userProjects = userProjectAdministration.GetUserProjects();
            Assert.Equal(2, userProjects.Count());
        }

        [Fact]
        public void get_User_Projects_only_returns_that_Users()
        {
            var fooUser = TestHelper.GetFoo();
            var fooUserProjectAdministration = TestHelper.GetUserProjectAdministration(fooUser.Id);

            var barUser = TestHelper.GetBar();
            var barUserProjectAdministration = TestHelper.GetUserProjectAdministration(barUser.Id);

            LoadProjects(fooUserProjectAdministration, fooUser.Id);
            LoadProjects(barUserProjectAdministration, fooUser.Id);

            var userProjects = barUserProjectAdministration.GetUserProjects();
            Assert.Equal(2, userProjects.Count());
        }

        [Fact]
        public void UserProjectAdministration_Invite_User_to_Project()
        {
            var user = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(user.Id);

            LoadProjects(userProjectAdministration, user.Id, 1);
            var project = userProjectAdministration.GetUserProjects().First();

            var projectInvitation = userProjectAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS, user);

            Assert.NotNull(projectInvitation);
            Assert.NotEqual(default(Guid), projectInvitation.ProjectInvitationId);
        }

        [Fact]
        public void UserProjectAdministration_returns_Invitations_for_Project()
        {
            var user = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(user.Id);

            LoadProjects(userProjectAdministration, Guid.NewGuid(), 2);
            var project1 = userProjectAdministration.GetUserProjects().First();
            var project2 = userProjectAdministration.GetUserProjects().Last();

            var project1Invitation = userProjectAdministration.InviteUserToProject(project1, TestHelper.VALID_EMAIL_ADDRESS, user);
            var project2Invitation = userProjectAdministration.InviteUserToProject(project2, TestHelper.VALID_EMAIL_ADDRESS, user);

            var project2Invitations = userProjectAdministration.GetProjectInvitations(project2);

            Assert.NotNull(project2Invitations);
            Assert.Equal(1, project2Invitations.Count());
            Assert.Equal(project2Invitation.ProjectInvitationId, project2Invitations.First().ProjectInvitationId);
        }
    }
}