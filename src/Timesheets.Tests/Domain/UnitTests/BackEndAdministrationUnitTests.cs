using System.Linq;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class BackEndAdministrationUnitTests
    {
        [Fact]
        public void BackEndAdministration_sets_the_project_invitation_to_sent()
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

                projectInvitation = invitationAdministration.GetProjectInvitations(project).Single(x => x.ProjectInvitationId == projectInvitation.ProjectInvitationId);
                Assert.True(projectInvitation.InvitationSent);
            }
        }
    }
}