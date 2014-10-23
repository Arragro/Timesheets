using System;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Models
{
    public class ProjectInvitationTests
    {
        [Fact]
        public void ProjectInvitation_instantation_fails_with_null_Project()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        new ProjectInvitation(null, null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal(ex.ParamName, "project");
                        throw ex;
                    }
                });
        }

        [Fact]
        public void ProjectInvitation_fails_when_a_null_Project_is_supplied()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    try
                    {
                        var projectInvitation =
                                new ProjectInvitation(
                                    new Project("Test", Guid.NewGuid()), string.Empty);
                        projectInvitation.SetProject(null);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Assert.Equal("project", ex.ParamName);
                        throw;
                    }
                });
        }

        [Fact]
        public void ProjectInvitation_set_user_id_fails_when_already_set()
        {
            Assert.Throws<Exception>(
                () =>
                {
                    try
                    {
                        var projectInvitation =
                                new ProjectInvitation(
                                    new Project("Test", Guid.NewGuid()), string.Empty);
                        projectInvitation.SetUserId(Guid.NewGuid());
                        projectInvitation.SetUserId(Guid.NewGuid());
                    }
                    catch (Exception ex)
                    {
                        Assert.Equal("The UserId is already set.", ex.Message);
                        throw;
                    }
                });
        }

        [Fact]
        public void ProjectInvitation_set_user_id_fails_when_default_guid_supplied()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    try
                    {
                        var projectInvitation =
                                new ProjectInvitation(
                                    new Project("Test", Guid.NewGuid()), string.Empty);
                        projectInvitation.SetUserId(new Guid());
                    }
                    catch (ArgumentException ex)
                    {
                        Assert.Equal("userId", ex.ParamName);
                        throw;
                    }
                });
        }
    }
}