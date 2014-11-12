using System;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Models
{
    public class ProjectContributorTests
    {
        [Fact]
        public void ProjectContributor_fails_when_a_null_Project_Invitation_is_supplied()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new ProjectContributor(null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal("projectInvitation", ex.ParamName);
                        throw;
                    }
                });
        }

        [Fact]
        public void ProjectContributor_fails_when_a_null_Project_is_supplied()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        var project = new Project("Test", Guid.NewGuid());
                        var projectInvitation =
                                new ProjectInvitation(
                                    project, string.Empty);
                        projectInvitation.SetUserId(Guid.NewGuid());
                        projectInvitation.SetProject(null);

                        var projectContributor =
                            new ProjectContributor(projectInvitation);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal("project", ex.ParamName);
                        throw;
                    }
                });
        }
    }
}