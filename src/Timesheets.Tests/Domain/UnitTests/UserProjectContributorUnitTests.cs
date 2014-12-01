using Arragro.Common.BusinessRules;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Domain;
using Timesheets.DataLayer.Enums;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserProjectContributorUnitTests
    {
        private ProjectContributor GetProjectContributor(
            TestHelper testHelper, IUser<Guid> fooUser, IUser<Guid> barUser)
        {
            var userProjects = testHelper.GetUserProjects(fooUser);
            var project = userProjects.AddProject(new Project("Test 1", fooUser.Id));

            var invitationAdministration = testHelper.GetUserProjectInvitations(fooUser);
            var projectInvitation = invitationAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS);

            // Will be set by the email service once the email has gone
            var backEndAdministration = testHelper.GetBackEndAdministration();
            backEndAdministration.SetProjectInvitationSent(projectInvitation);

            var invitee = barUser;
            invitee.UserName = TestHelper.VALID_EMAIL_ADDRESS;
            invitationAdministration = testHelper.GetUserProjectInvitations(invitee);
            var projectContributor = invitationAdministration.AcceptInvitation(projectInvitation.InvitationCode);

            return projectContributor;
        }

        [Fact]
        public void change_of_project_owner_fails_with_invalid_project()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    using (var testHelper = new TestHelper())
                    {
                        var user = TestHelper.GetFoo();
                        var project = new Project("Test 1", user.Id);
                        var projectContrbutor = new ProjectContributor(project, Guid.NewGuid());
                        var userProjectContributor = testHelper.GetUserProjectContributor(user);

                        try
                        {
                            userProjectContributor.ChangeProjectContributorsRole(projectContrbutor, ContributorRole.ReadOnlyAdministrator);
                        }
                        catch (ArgumentNullException ex)
                        {
                            Assert.Equal(ex.ParamName, "project");
                            throw;
                        }
                    }
                });
        }

        [Fact]
        public void change_of_project_owner_fails_with_project_owner_error()
        {
            Assert.Throws<RulesException>(
                () =>
                {
                    using (var testHelper = new TestHelper())
                    {
                        var fooUser = TestHelper.GetFoo();
                        var barUser = TestHelper.GetBar();

                        GetProjectContributor(testHelper, fooUser, barUser);
                        var userProjectContributor = testHelper.GetUserProjectContributor(fooUser);

                        var userProjects = testHelper.GetUserProjects(fooUser);
                        var project = userProjects.GetUserProjects().First();
                        var projectContributor = userProjectContributor.GetProjectContributor(project, fooUser.Id);

                        try
                        {
                            userProjectContributor.ChangeProjectContributorsRole(projectContributor, ContributorRole.ReadOnlyAdministrator);
                        }
                        catch (RulesException ex)
                        {
                            Assert.Equal(ex.Errors[0].Message, UserProjectContributor.OWNER_ROLE_CANNOT_BE_CHANGED);
                            throw;
                        }
                    }
                });
        }

        [Fact]
        public void user_cannot_get_project_contributors_because_they_dont_have_permission()
        {
            Assert.Throws<RulesException>(
                () =>
                {
                    using (var testHelper = new TestHelper())
                    {
                        var fooUser = TestHelper.GetFoo();
                        var barUser = TestHelper.GetBar();

                        GetProjectContributor(testHelper, fooUser, barUser);
                        var userProjectContributor = testHelper.GetUserProjectContributor(fooUser);

                        var userProjects = testHelper.GetUserProjects(fooUser);
                        var project = userProjects.GetUserProjects().First();
                        userProjectContributor = testHelper.GetUserProjectContributor(barUser);

                        try
                        {
                            userProjectContributor.GetProjectContributors(project);
                        }
                        catch (RulesException ex)
                        {
                            Assert.Equal(ex.Errors[0].Message, SecurityRules.USER_IS_NOT_AUTHORISED_TO_READ);
                            throw;
                        }
                    }
                });
        }

        [Fact]
        public void change_of_project_contributor_role_by_owner()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var barUser = TestHelper.GetBar();

                var barProjectContributor = GetProjectContributor(testHelper, fooUser, barUser);
                var userProjectContributor = testHelper.GetUserProjectContributor(fooUser);

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.Administrator);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.Administrator);

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.ReadOnlyAdministrator);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.ReadOnlyAdministrator);

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.User);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.User);
            }
        }

        [Fact]
        public void change_of_project_contributor_role_by_administrator()
        {
            using (var testHelper = new TestHelper())
            {
                var fooUser = TestHelper.GetFoo();
                var barUser = TestHelper.GetBar();

                var barProjectContributor = GetProjectContributor(testHelper, fooUser, barUser);
                var userProjectContributor = testHelper.GetUserProjectContributor(fooUser);

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.Administrator);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.Administrator);
            }
        }

        [Fact]
        public void change_of_project_contributor_role_fails_when_not_adminiatrator()
        {
            Assert.Throws<RulesException>(() =>
                {
                    try
                    {
                        using (var testHelper = new TestHelper())
                        {
                            var fooUser = TestHelper.GetFoo();
                            var barUser = TestHelper.GetBar();

                            var barProjectContributor = GetProjectContributor(testHelper, fooUser, barUser);
                            var userProjectContributor = testHelper.GetUserProjectContributor(barUser);

                            barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.Administrator);
                        }
                    }
                    catch(RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, SecurityRules.USER_IS_NOT_AUTHORISED_TO_MODIFY);
                        throw;
                    }
                });
        }
    }
}