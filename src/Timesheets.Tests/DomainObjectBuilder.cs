using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using Timesheets.BusinessLayer.Domain;
using Timesheets.DataLayer.Models;

namespace Timesheets.Tests
{
    public class DomainObjectBuilder
    {
        public static void LoadTimeSheetEntries(
            UserTimesheetEntries userTimesheetEntries,
            IUser<Guid> user,
            Project project = null,
            int numberOfTimesheets = 31)
        {
            var counter = 0;
            while (counter < numberOfTimesheets)
            {
                var timesheetEntry = new TimesheetEntry(
                    user.Id, DateTime.Now.Date.AddDays(0 - counter),
                    8, "Foo Entry", project);
                userTimesheetEntries.AddTimesheetEntry(timesheetEntry);
                counter++;
            }
        }

        public static void LoadProjects(
            UserProjects userProjects,
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

                userProjects.AddProject(project);
                counter++;
            }
        }

        public static IEnumerable<ProjectInvitation> LoadProjectInvitation(
            Project project,
            UserProjectInvitations userProjectInvitations,
            BackEndAdministration backEndAdministration,
            string[] emailAddresses,
            bool sendInvitations = true)
        {
            var projectInvitations = new List<ProjectInvitation>();

            foreach (var emailAddress in emailAddresses)
            {
                var projectInvitation = userProjectInvitations.InviteUserToProject(project, emailAddress);
                if (sendInvitations)
                    backEndAdministration.SetProjectInvitationSent(projectInvitation);
                projectInvitations.Add(projectInvitation);
            }
            return projectInvitations;
        }

        public static IEnumerable<Tuple<ProjectInvitation, ProjectContributor>> AcceptProjectInvitations(
            TestHelper testHelper,
            Project project,            
            IEnumerable<ProjectInvitation> projectInvitations,
            string[] emailAddresses)
        {
            var output = new List<Tuple<ProjectInvitation, ProjectContributor>>();

            for (int i = 0; i < projectInvitations.Count(); i++)
            {
                var projectInvitation = projectInvitations.ElementAt(i);
                var emailAddress = emailAddresses[i];

                var invitee = TestHelper.GetBar();
                invitee.UserName = emailAddress;

                var userProjectInvitations = testHelper.GetUserProjectInvitations(invitee);
                var contributor = userProjectInvitations.AcceptInvitation(projectInvitation.InvitationCode);
                projectInvitation = userProjectInvitations.GetProjectInvitations(project).First(x => x.InvitationCode == projectInvitation.InvitationCode);

                output.Add(new Tuple<ProjectInvitation, ProjectContributor>(projectInvitation, contributor));
            }
            return output;
        }
    }
}
