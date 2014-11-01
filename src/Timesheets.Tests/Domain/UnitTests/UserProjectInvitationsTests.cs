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
    }
}