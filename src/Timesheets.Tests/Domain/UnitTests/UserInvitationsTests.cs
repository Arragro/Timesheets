using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserInvitationsTests
    {
        /*

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
         */
    }
}