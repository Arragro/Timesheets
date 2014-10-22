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
        public void ProjectContributor_fails_when_a_null_or_default_userId_is_supplied()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new ProjectContributor(new ProjectInvitation(new Project("Test", Guid.NewGuid()), string.Empty, new Guid()));
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal("projectInvitation.UserId", ex.ParamName);
                        throw;
                    }
                });
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new ProjectContributor(new ProjectInvitation(new Project("Test", Guid.NewGuid()), string.Empty, null));
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal("projectInvitation.UserId", ex.ParamName);
                        throw;
                    }
                });
        }
    }
}