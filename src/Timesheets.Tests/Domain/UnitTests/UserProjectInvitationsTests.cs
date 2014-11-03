using System;
using System.Linq;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserInvitationsTests
    {
        [Fact]
        public void UserProjectAdministration_Invite_User_to_Project()
        {
            var user = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjects(user);
            userProjectAdministration.AddProject(new Project("Test", user.Id));

            var project = userProjectAdministration.GetUserProjects().First();

            var invitationAdministration = TestHelper.GetUserProjectInvitations(user);
            var projectInvitation = invitationAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS);

            Assert.NotNull(projectInvitation);
            Assert.NotEqual(default(Guid), projectInvitation.ProjectInvitationId);
        }

        [Fact]
        public void UserProjectAdministration_returns_Invitations_for_Project()
        {
            var user = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjects(user);
            userProjectAdministration.AddProject(new Project("Test 1", user.Id));
            userProjectAdministration.AddProject(new Project("Test 2", user.Id));

            var project1 = userProjectAdministration.GetUserProjects().First();
            var project2 = userProjectAdministration.GetUserProjects().Last();

            var invitationAdministration = TestHelper.GetUserProjectInvitations(user);
            var project1Invitation = invitationAdministration.InviteUserToProject(project1, TestHelper.VALID_EMAIL_ADDRESS);
            var project2Invitation = invitationAdministration.InviteUserToProject(project2, TestHelper.VALID_EMAIL_ADDRESS);

            var project2Invitations = invitationAdministration.GetProjectInvitations(project2);

            Assert.NotNull(project2Invitations);
            Assert.Equal(1, project2Invitations.Count());
            Assert.Equal(project2Invitation.ProjectInvitationId, project2Invitations.First().ProjectInvitationId);

            Assert.NotNull(project2Invitation.Project);
            Assert.Equal(project2.ProjectId, project2Invitation.Project.ProjectId);
        }

        [Fact]
        public void User_accepts_invitation_and_becomes_contributor()
        {
            var user = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjects(user);
            var project = userProjectAdministration.AddProject(new Project("Test 1", user.Id));

            var invitationAdministration = TestHelper.GetUserProjectInvitations(user);
            invitationAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS);
            var projectInvitation = invitationAdministration.GetProjectInvitations(project).First(i => i.EmailAddress == TestHelper.VALID_EMAIL_ADDRESS);

            // Will be set by the email service once the email has gone
            var backEndAdministration = TestHelper.GetBackEndAdministration();
            backEndAdministration.SetProjectInvitationSent(projectInvitation);
            projectInvitation = invitationAdministration.GetProjectInvitations(project).First(i => i.EmailAddress == TestHelper.VALID_EMAIL_ADDRESS);

            var invitee = TestHelper.GetBar();
            invitee.UserName = TestHelper.VALID_EMAIL_ADDRESS;
            invitationAdministration = TestHelper.GetUserProjectInvitations(invitee);
            var contributor = invitationAdministration.AcceptInvitation(projectInvitation.InvitationCode);
            projectInvitation = invitationAdministration.GetProjectInvitations(project).First(i => i.InvitationCode == projectInvitation.InvitationCode);

            Assert.NotNull(projectInvitation);
            Assert.Equal(invitee.Id, projectInvitation.UserId);
            Assert.True(projectInvitation.InvitationAccepted.HasValue);
            Assert.True(projectInvitation.InvitationAccepted.Value);

            Assert.NotNull(contributor);
            Assert.Equal(invitee.Id, contributor.UserId);
        }

        [Fact]
        public void User_rejects_invitation()
        {
            var user = TestHelper.GetFoo();
            var userProjectAdministration = TestHelper.GetUserProjects(user);
            var project = userProjectAdministration.AddProject(new Project("Test 1", user.Id));

            var invitationAdministration = TestHelper.GetUserProjectInvitations(user);
            var projectInvitation = invitationAdministration.InviteUserToProject(project, TestHelper.VALID_EMAIL_ADDRESS);

            // Will be set by the email service once the email has gone
            var backEndAdministration = TestHelper.GetBackEndAdministration();
            backEndAdministration.SetProjectInvitationSent(projectInvitation);

            var invitee = TestHelper.GetBar();
            invitee.UserName = TestHelper.VALID_EMAIL_ADDRESS;
            invitationAdministration = TestHelper.GetUserProjectInvitations(invitee);
            invitationAdministration.RejectInvitation(projectInvitation.InvitationCode);

            projectInvitation = invitationAdministration.GetProjectInvitations(project).First(i => i.InvitationCode == projectInvitation.InvitationCode);

            Assert.NotNull(projectInvitation);
            Assert.Equal(invitee.Id, projectInvitation.UserId);
            Assert.True(projectInvitation.InvitationAccepted.HasValue);
            Assert.False(projectInvitation.InvitationAccepted.Value);
        }
    }
}