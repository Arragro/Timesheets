using Arragro.Common.BusinessRules;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Services.UnitTests
{
    public class ProjectContributorServiceUnitTests
    {
        private ProjectInvitation GetProjectInvitation()
        {
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(Guid.NewGuid());

            var project = new Project("Test", Guid.NewGuid(), startDate: DateTime.Now, endDate: DateTime.Now.AddDays(10));
            project = userProjectAdministration.AddProject(project);

            return new ProjectInvitation(project, string.Empty, Guid.NewGuid());
        }

        [Fact]
        public void ProjectContributor_fails_when_Dates_are_invalid()
        {
            var projectContributorService = TestHelper.GetProjectContributorService();

            Assert.Throws<RulesException<ProjectContributor>>(
                () =>
                {
                    try
                    {
                        var projectContributor = new ProjectContributor(GetProjectInvitation());
                        projectContributor.SetContributorProjectDetails(startDate: DateTime.Now);
                        projectContributorService.ValidateModel(projectContributor);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors.Count, 1);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectContributorService.DATES_BOTH_MUST_BE_ENTERED_OR_NONE));
                        throw;
                    }
                });

            Assert.Throws<RulesException<ProjectContributor>>(
                () =>
                {
                    try
                    {
                        var projectContributor = new ProjectContributor(GetProjectInvitation());
                        projectContributor.SetContributorProjectDetails(endDate: DateTime.Now);
                        projectContributorService.ValidateModel(projectContributor);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors.Count, 1);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectContributorService.DATES_BOTH_MUST_BE_ENTERED_OR_NONE));
                        throw;
                    }
                });
        }

        [Fact]
        public void ProjectContributor_fails_when_Dates_are_the_wrong_way_round()
        {
            var projectContributorService = TestHelper.GetProjectContributorService();

            Assert.Throws<RulesException<ProjectContributor>>(
                () =>
                {
                    try
                    {
                        var projectContributor = new ProjectContributor(GetProjectInvitation());
                        projectContributor.SetContributorProjectDetails(startDate: DateTime.Now, endDate: DateTime.Now.AddDays(-1));
                        projectContributorService.ValidateModel(projectContributor);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors.Count, 1);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectContributorService.END_DATE_MUST_BE_MORE_THAN_OR_EQUAL_TO_START_DATE));
                        throw;
                    }
                });
        }

        [Fact]
        public void ProjectContributor_fails_when_Dates_outside_of_Project_dates()
        {
            var projectContributorService = TestHelper.GetProjectContributorService();

            Assert.Throws<RulesException<ProjectContributor>>(
                () =>
                {
                    try
                    {
                        var projectContributor = new ProjectContributor(GetProjectInvitation());
                        projectContributor.SetContributorProjectDetails(startDate: DateTime.Now, endDate: DateTime.Now.AddDays(11));
                        projectContributorService.ValidateModel(projectContributor);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors.Count, 1);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectContributorService.CONTRIBUTOR_MUST_HAVE_DATES_THAT_ARE_WITHIN_THE_PROJECT_DATES));
                        throw;
                    }
                });

            projectContributorService.RulesException.Errors.Clear();

            Assert.Throws<RulesException<ProjectContributor>>(
                () =>
                {
                    try
                    {
                        var projectContributor = new ProjectContributor(GetProjectInvitation());
                        projectContributor.SetContributorProjectDetails(startDate: DateTime.Now.AddDays(-10), endDate: DateTime.Now.AddDays(1));
                        projectContributorService.ValidateModel(projectContributor);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors.Count, 1);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectContributorService.CONTRIBUTOR_MUST_HAVE_DATES_THAT_ARE_WITHIN_THE_PROJECT_DATES));
                        throw;
                    }
                });

            projectContributorService.RulesException.Errors.Clear();

            Assert.Throws<RulesException<ProjectContributor>>(
                () =>
                {
                    try
                    {
                        var projectContributor = new ProjectContributor(GetProjectInvitation());
                        projectContributor.SetContributorProjectDetails(startDate: DateTime.Now.AddDays(-10), endDate: DateTime.Now.AddDays(11));
                        projectContributorService.ValidateModel(projectContributor);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors.Count, 1);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectContributorService.CONTRIBUTOR_MUST_HAVE_DATES_THAT_ARE_WITHIN_THE_PROJECT_DATES));
                        throw;
                    }
                });
        }
    }
}