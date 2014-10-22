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
    }
}