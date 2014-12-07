using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using Timesheets.BusinessLayer.Domain;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests
{
    public class TestContributorContainer
    {
        public ProjectContributor ProjectContributor { get; private set; }
        public IUser<Guid> User { get; private set; }

        public TestContributorContainer(
            ProjectContributor projectContributor, IUser<Guid> user)
        {
            ProjectContributor = projectContributor;
            User = user;
        }
    }

    public class TestProjectContainer
    {
        public Project Project { get; private set; }
        public TestContributorContainer ProjectOwner { get; private set; }
        public List<TestContributorContainer> ProjectContributors { get; private set;}
        public List<TestContributorContainer> ProjectAdministrators { get; private set;}
        public List<TestContributorContainer> ProjectReadOnlyAdministrators { get; private set;}

        public TestProjectContainer(
            IEnumerable<Tuple<Project, ProjectContributor, IUser<Guid>>> results)
        {
            if (results == null) throw new ArgumentNullException("results");
            var resultsList = results.ToList();
            if (results.Count() != 10) throw new ArgumentException("Expecting 10 results (1 Admin, 3 Contributors, 3 Admins & 3 Read Only Admins)");

            ProjectContributors = new List<TestContributorContainer>();
            ProjectAdministrators = new List<TestContributorContainer>();
            ProjectReadOnlyAdministrators = new List<TestContributorContainer>();

            for (int i = 0; i < resultsList.Count; i++)
            {
                var result = resultsList[i];
                if (i == 0)
                {
                    Project = result.Item1;
                    ProjectOwner = new TestContributorContainer(result.Item2, result.Item3);
                }
                if (i > 0 && i <= 3)
                {
                    ProjectContributors.Add(new TestContributorContainer(result.Item2, result.Item3));
                }
                if (i > 3 && i <= 6)
                {
                    ProjectAdministrators.Add(new TestContributorContainer(result.Item2, result.Item3));
                }
                if (i > 6 && i <= 9)
                {
                    ProjectReadOnlyAdministrators.Add(new TestContributorContainer(result.Item2, result.Item3));
                }
            }
        }
    }

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

                var invitee = TestHelper.GetUser(emailAddress);

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
            TestHelper testHelper, IUser<Guid> ownerUser, string[] emailAddresses)
        {
            var userProjectAdministration = testHelper.GetUserProjects(ownerUser);
            var project = userProjectAdministration.AddProject(new Project("Test 1", ownerUser.Id));

            return CreateAndGetProjectContributors(testHelper, ownerUser, project, emailAddresses);
        }

        public static IEnumerable<Tuple<Project, ProjectInvitation, ProjectContributor, IUser<Guid>>> CreateAndGetProjectContributors(
            TestHelper testHelper, IUser<Guid> ownerUser, Project project, string[] emailAddresses)
        {
            var results = DomainObjectBuilder.AcceptRejectProjectInvitations(
                    testHelper, ownerUser, project, emailAddresses);

            return results.Select(x => new Tuple<Project, ProjectInvitation, ProjectContributor, IUser<Guid>>(project, x.Item1, x.Item2, x.Item3));
        }

        public static TestProjectContainer CreateTestProjectContainerData(
            TestHelper testHelper)
        {
            var ownerUser = TestHelper.GetOwnerUser();
            var userProjectAdministration = testHelper.GetUserProjects(ownerUser);
            var project = userProjectAdministration.AddProject(new Project("Test 1", ownerUser.Id));
            var userProjectContributor = testHelper.GetUserProjectContributor(ownerUser);

            var projectOwnerContributor = userProjectContributor.GetProjectContributor(project, ownerUser.Id);

            var results = new List<Tuple<Project, ProjectContributor, IUser<Guid>>>();
            results.Add(new Tuple<Project, ProjectContributor, IUser<Guid>>(project, projectOwnerContributor, ownerUser));

            var projectContributors = CreateAndGetProjectContributors(
                                        testHelper, ownerUser, project,
                                        TestHelper.GetEmailAddresses(3, 1));
            results.AddRange(
                projectContributors.Select(x => new Tuple<Project, ProjectContributor, IUser<Guid>>(project, x.Item3, x.Item4)));
            
            var projectAdministrators = CreateAndGetProjectContributors(
                                        testHelper, ownerUser, project,
                                        TestHelper.GetEmailAddresses(3, 4));

            foreach (var projectAdministrator in projectAdministrators)
            {
                var contrib = userProjectContributor.GetProjectContributor(project, projectAdministrator.Item4.Id);
                contrib = userProjectContributor.ChangeProjectContributorsRole(contrib, DataLayer.Enums.ContributorRole.Administrator);
                results.Add(new Tuple<Project,ProjectContributor,IUser<Guid>>(project, contrib, projectAdministrator.Item4));
            }

            var projectReadOnlyAdministrators = CreateAndGetProjectContributors(
                                        testHelper, ownerUser, project,
                                        TestHelper.GetEmailAddresses(3, 7));

            foreach (var projectReadOnlyAdministrator in projectReadOnlyAdministrators)
            {
                var contrib = userProjectContributor.GetProjectContributor(project, projectReadOnlyAdministrator.Item4.Id);
                contrib = userProjectContributor.ChangeProjectContributorsRole(contrib, DataLayer.Enums.ContributorRole.ReadOnlyAdministrator);
                results.Add(new Tuple<Project, ProjectContributor, IUser<Guid>>(project, contrib, projectReadOnlyAdministrator.Item4));
            }

            return new TestProjectContainer(results);
        }

        [Fact]
        public void create_test_project_container()
        {
            using (var testHelper = new TestHelper())
            {
                var tpc = DomainObjectBuilder.CreateTestProjectContainerData(testHelper);
            }
        }
    }
}
