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

        public static IEnumerable<ProjectInvitation> LoadProjectInvitations(
            TestHelper testHelper,
            IUser<Guid> user,
            Project project,
            string[] emailAddresses,
            bool sendInvitations = true)
        {
            var projectInvitations = new List<ProjectInvitation>();
            
            var userProjectInvitations = testHelper.GetUserProjectInvitations(user);
            var backEndAdministration = testHelper.GetBackEndAdministration();

            foreach (var emailAddress in emailAddresses)
            {
                var projectInvitation = userProjectInvitations.InviteUserToProject(project, emailAddress);
                if (sendInvitations)
                    backEndAdministration.SetProjectInvitationSent(projectInvitation);
                projectInvitations.Add(projectInvitation);
            }
            return projectInvitations;
        }

        public static IEnumerable<Tuple<ProjectInvitation, ProjectContributor, IUser<Guid>>> AcceptRejectProjectInvitations(
            TestHelper testHelper,
            IUser<Guid> user,
            Project project,
            string[] emailAddresses,
            bool accept = true)
        {
            var output = new List<Tuple<ProjectInvitation, ProjectContributor, IUser<Guid>>>();

            var projectInvitations = LoadProjectInvitations(
                testHelper, user, project, emailAddresses);

            for (int i = 0; i < projectInvitations.Count(); i++)
            {
                var projectInvitation = projectInvitations.ElementAt(i);
                var emailAddress = emailAddresses[i];

                var invitee = TestHelper.GetBar();
                invitee.UserName = emailAddress;

                var inviteeProjectInvitations = testHelper.GetUserProjectInvitations(invitee);
                ProjectContributor contributor = null;
                if (accept)
                    contributor = inviteeProjectInvitations.AcceptInvitation(projectInvitation.InvitationCode);
                else
                    inviteeProjectInvitations.RejectInvitation(projectInvitation.InvitationCode);

                var userProjectInvitations = testHelper.GetUserProjectInvitations(user);
                projectInvitation = userProjectInvitations.GetProjectInvitations(project).First(x => x.InvitationCode == projectInvitation.InvitationCode);

                output.Add(new Tuple<ProjectInvitation, ProjectContributor, IUser<Guid>>(projectInvitation, contributor, invitee));
            }
            return output;
        }

        public static IEnumerable<Tuple<Project, ProjectInvitation, ProjectContributor, IUser<Guid>>> CreateAndGetProjectContributors(
            TestHelper testHelper, IUser<Guid> fooUser, string[] emailAddresses)
        {
            var userProjectAdministration = testHelper.GetUserProjects(fooUser);
            var project = userProjectAdministration.AddProject(new Project("Test 1", fooUser.Id));

            var results = DomainObjectBuilder.AcceptRejectProjectInvitations(
                    testHelper, fooUser, project, emailAddresses);

            return results.Select(x => new Tuple<Project, ProjectInvitation, ProjectContributor, IUser<Guid>>(project, x.Item1, x.Item2, x.Item3));
        }
    }
}
