using Arragro.Common.BusinessRules;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Services.UnitTests
{
    public class projectInvitationServiceUnitTest
    {
        [Fact]
        public void projectInvitationService_validates_model_attributes()
        {
            var projectInvitationService = TestHelper.GetProjectInvitationService();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        projectInvitationService.ValidateModel(new ProjectInvitation(new Project("Test", Guid.NewGuid()), null));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(2, ex.Errors.Count);
                        Assert.NotNull(ex.ContainsErrorForProperty(".EmailAddress"));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectInvitationService.PROJECTID_NOT_SET));
                        throw ex;
                    }
                });
        }

        [Fact]
        public void ProjectInviationService_validates_model_email_invalid()
        {
            var projectInvitationService = TestHelper.GetProjectInvitationService();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        var project = new Project("Test", Guid.NewGuid());
                        project.SetProjectId();

                        projectInvitationService.ValidateModel(
                            new ProjectInvitation(
                                project, "bad_email"));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(1, ex.Errors.Count);
                        Assert.NotNull(ex.ContainsErrorForProperty(".EmailAddress"));
                        throw ex;
                    }
                });
        }

        [Fact]
        public void ProjectInviationService_validates_model_email_valid()
        {
            var projectInvitationService = TestHelper.GetProjectInvitationService();

            var project = new Project("Test", Guid.NewGuid());
            project.SetProjectId();

            projectInvitationService.ValidateModel(
                            new ProjectInvitation(
                                project, TestHelper.VALID_EMAIL_ADDRESS));

            Assert.True(true, "Email Address is valid");
        }

        [Fact]
        public void projectInvitationService_validates_UserId_invalid()
        {
            var projectInvitationService = TestHelper.GetProjectInvitationService();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        var project = new Project("Test", Guid.NewGuid());
                        project.SetProjectId();

                        projectInvitationService.ValidateModel(
                            new ProjectInvitation(
                                project, TestHelper.VALID_EMAIL_ADDRESS, new Guid()));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(1, ex.Errors.Count);
                        Assert.NotNull(ex.ContainsErrorForProperty(".UserId"));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectInvitationService.USER_IS_NOT_NULL_NOT_SET));
                        throw ex;
                    }
                });
        }

        [Fact]
        public void projectInvitationService_validates_InvitationAccepted_invalid()
        {
            var projectInvitationService = TestHelper.GetProjectInvitationService();

            Assert.Throws<Exception>(
                () =>
                {
                    try
                    {
                        var project = new Project("Test", Guid.NewGuid());
                        project.SetProjectId();

                        var projectInvitation = new ProjectInvitation(
                                project, TestHelper.VALID_EMAIL_ADDRESS);
                        projectInvitation.SetProjectInvitationAccepted(true);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
        }

        [Fact]
        public void ProjectInvitionService_UserId_already_invited()
        {
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(Guid.NewGuid());
            var projectInvitationService = TestHelper.GetProjectInvitationService();

            var project = new Project("Test", Guid.NewGuid());
            userProjectAdministration.AddProject(project);

            var userId = Guid.NewGuid();

            projectInvitationService.ValidateAndInsertOrUpdate(
                new ProjectInvitation(
                    project, TestHelper.VALID_EMAIL_ADDRESS, userId), userId);
            projectInvitationService.SaveChanges();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        projectInvitationService.ValidateModel(
                            new ProjectInvitation(
                                project, TestHelper.VALID_EMAIL_ADDRESS, userId));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(1, ex.Errors.Count);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectInvitationService.INVITATION_ALREADY_EXISTS_FOR_THIS_USERID));
                        throw ex;
                    }
                });
        }

        [Fact]
        public void ProjectInvitionService_EmailAddress_already_invited()
        {
            var userProjectAdministration = TestHelper.GetUserProjectAdministration(Guid.NewGuid());
            var projectInvitationService = TestHelper.GetProjectInvitationService();

            var project = new Project("Test", Guid.NewGuid());
            project = userProjectAdministration.AddProject(project);

            projectInvitationService.ValidateAndInsertOrUpdate(
                new ProjectInvitation(
                    project, TestHelper.VALID_EMAIL_ADDRESS), Guid.NewGuid());
            projectInvitationService.SaveChanges();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        projectInvitationService.ValidateModel(
                            new ProjectInvitation(project, TestHelper.VALID_EMAIL_ADDRESS));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(1, ex.Errors.Count);
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectInvitationService.INVITATION_ALREADY_EXISTS_FOR_THIS_EMAILADDRESS));
                        throw ex;
                    }
                });
        }
    }
}