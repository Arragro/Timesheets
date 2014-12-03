using Arragro.Common.BusinessRules;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Domain;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserInvitationsTests
    {
        private void ValidateProjectInvitation(
            Project project, ProjectInvitation projectInvitation, string emailAddress, bool accepted = true)
        {
            Assert.NotNull(projectInvitation);
            Assert.Equal(projectInvitation.ProjectId, project.ProjectId);
            Assert.Equal(projectInvitation.EmailAddress, emailAddress);
            Assert.True(projectInvitation.InvitationAccepted.HasValue);
            if (accepted)
                Assert.True(projectInvitation.InvitationAccepted.Value);
            else
                Assert.False(projectInvitation.InvitationAccepted.Value);
        }

        [Fact]
        public void UserProjectAdministration_Invite_User_to_Project()
        {
            using (var testHelper = new TestHelper())
            {
                var user = TestHelper.GetFoo();
                var userProjectAdministration = testHelper.GetUserProjects(user);
                userProjectAdministration.AddProject(new Project("Test", user.Id));

                var project = userProjectAdministration.GetUserProjects().First();

                var invitationAdministration = testHelper.GetUserProjectInvitations(user);
                var projectInvitation = invitationAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS);

                Assert.NotNull(projectInvitation);
                Assert.NotEqual(default(Guid), projectInvitation.ProjectInvitationId);
            }
        }

        [Fact]
        public void UserProjectAdministration_returns_Invitations_for_Project()
        {
            using (var testHelper = new TestHelper())
            {
                var user = TestHelper.GetFoo();
                var userProjectAdministration = testHelper.GetUserProjects(user);
                userProjectAdministration.AddProject(new Project("Test 1", user.Id));
                userProjectAdministration.AddProject(new Project("Test 2", user.Id));

                var project1 = userProjectAdministration.GetUserProjects().First();
                var project2 = userProjectAdministration.GetUserProjects().Last();

                var invitationAdministration = testHelper.GetUserProjectInvitations(user);
                var project1Invitation = invitationAdministration.InviteUserToProject(project1, TestHelper.VALID_EMAIL_ADDRESS);
                var project2Invitation = invitationAdministration.InviteUserToProject(project2, TestHelper.VALID_EMAIL_ADDRESS);

                var project2Invitations = invitationAdministration.GetProjectInvitations(project2);

                Assert.NotNull(project2Invitations);
                Assert.Equal(1, project2Invitations.Count());
                Assert.Equal(project2Invitation.ProjectInvitationId, project2Invitations.First().ProjectInvitationId);

                Assert.NotNull(project2Invitation.Project);
                Assert.Equal(project2.ProjectId, project2Invitation.Project.ProjectId);
            }
        }

        [Fact]
        public void User_accepts_invitation_and_becomes_contributor()
        {
            using (var testHelper = new TestHelper())
            {
                var user = TestHelper.GetFoo();
                var userProjects = testHelper.GetUserProjects(user);
                var userProjectInvitations = testHelper.GetUserProjectInvitations(user);
                var backEndAdministration = testHelper.GetBackEndAdministration();

                var userEmails = new[] { TestHelper.VALID_EMAIL_ADDRESS };

                var project = userProjects.AddProject(new Project("Test 1", user.Id));

                var projectInvitations = 
                    DomainObjectBuilder.LoadProjectInvitation(
                        project, userProjectInvitations, backEndAdministration, userEmails);

                var results = DomainObjectBuilder.AcceptProjectInvitations(testHelper, project, projectInvitations, userEmails);

                for (int i = 0; i < results.Count(); i++)
                {
                    var emailAddress = userEmails[i];
                    var projectInvitation = results.ElementAt(i).Item1;
                    var contributor = results.ElementAt(i).Item2;

                    ValidateProjectInvitation(project, projectInvitation, emailAddress);

                    Assert.NotNull(contributor);
                    Assert.Equal(projectInvitation.UserId, contributor.UserId);
                }
            }
        }

        [Fact]
        public void User_rejects_invitation()
        {
            using (var testHelper = new TestHelper())
            {
                var user = TestHelper.GetFoo();
                var userProjectAdministration = testHelper.GetUserProjects(user);
                var backEndAdministration = testHelper.GetBackEndAdministration();
                var userProjectInvitations = testHelper.GetUserProjectInvitations(user);
                var invitationAdministration = testHelper.GetUserProjectInvitations(user);

                var userEmails = new[] { TestHelper.VALID_EMAIL_ADDRESS };
                var project = userProjectAdministration.AddProject(new Project("Test 1", user.Id));
                
                var projectInvitations =
                    DomainObjectBuilder.LoadProjectInvitation(
                        project, userProjectInvitations, backEndAdministration, userEmails);

                foreach (var projectInvitation in projectInvitations)
                {
                    var invitee = TestHelper.GetBar();
                    invitee.UserName = TestHelper.VALID_EMAIL_ADDRESS;
                    invitationAdministration = testHelper.GetUserProjectInvitations(invitee);
                    invitationAdministration.RejectInvitation(projectInvitation.InvitationCode);

                    var rejectedProjectInvitation = invitationAdministration.GetProjectInvitations(project).First(i => i.InvitationCode == projectInvitation.InvitationCode);

                    ValidateProjectInvitation(project, rejectedProjectInvitation, TestHelper.VALID_EMAIL_ADDRESS, false);
                }
            }
        }

        [Fact]
        public void User_security_cannot_invite_user()
        {
            Assert.Throws<RulesException>(
                () =>
                {
                    using (var testHelper = new TestHelper())
                    {
                        var user = TestHelper.GetFoo();
                        var userProjects = testHelper.GetUserProjects(user);
                        var project = userProjects.AddProject(new Project("Test 1", user.Id));

                        var invitationAdministration = testHelper.GetUserProjectInvitations(user);
                        var projectInvitation = invitationAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS);

                        // Will be set by the email service once the email has gone
                        var backEndAdministration = testHelper.GetBackEndAdministration();
                        backEndAdministration.SetProjectInvitationSent(projectInvitation);

                        var invitee = TestHelper.GetBar();
                        invitee.UserName = TestHelper.VALID_EMAIL_ADDRESS;
                        invitationAdministration = testHelper.GetUserProjectInvitations(invitee);
                        var projectContributor = invitationAdministration.AcceptInvitation(projectInvitation.InvitationCode);

                        try
                        {
                            invitationAdministration = testHelper.GetUserProjectInvitations(invitee);
                            invitationAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS);
                        }
                        catch (RulesException ex)
                        {
                            Assert.Equal(ex.Errors[0].Message, SecurityRules.USER_IS_NOT_AUTHORISED_TO_MODIFY);
                            throw;
                        }
                    }
                });
        }
    }
}