using Arragro.Common.BusinessRules;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Services.UnitTests
{
    public class ProjectInvitationServiceUnitTest
    {
        private ProjectInvitationService GetProjectInvitationService()
        {
            var unitContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unitContainer.Resolve<ProjectInvitationService>();
        }

        [Fact]
        public void ProjectInvitationService_validates_model_attributes()
        {
            var projectInvitationService = GetProjectInvitationService();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        var project = new Project("Test", Guid.NewGuid());
                        project.SetProjectId();
                        projectInvitationService.ValidateModel(new ProjectInvitation(null, null));
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
            var projectIntegrationService = GetProjectInvitationService();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        var project = new Project("Test", Guid.NewGuid());
                        project.SetProjectId();

                        projectIntegrationService.ValidateModel(
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
            var projectIntegrationService = GetProjectInvitationService();

            var project = new Project("Test", Guid.NewGuid());
            project.SetProjectId();

            projectIntegrationService.ValidateModel(
                            new ProjectInvitation(
                                project, TestHelper.VALID_EMAIL_ADDRESS));

            Assert.True(true, "Email Address is valid");
        }

        [Fact]
        public void ProjectInvitationServie_validates_UserId_invalid()
        {
            var projectIntegrationService = GetProjectInvitationService();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        var project = new Project("Test", Guid.NewGuid());
                        project.SetProjectId();

                        projectIntegrationService.ValidateModel(
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
        public void ProjectInvitationServie_validates_InvitationAccepted_invalid()
        {
            var projectIntegrationService = GetProjectInvitationService();

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
            var projectIntegrationService = GetProjectInvitationService();

            var project = new Project("Test", Guid.NewGuid());
            project.SetProjectId();

            var userId = Guid.NewGuid();

            projectIntegrationService.ValidateAndInsertOrUpdate(
                new ProjectInvitation(
                    project, TestHelper.VALID_EMAIL_ADDRESS, userId), userId);
            projectIntegrationService.SaveChanges();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        projectIntegrationService.ValidateModel(
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
            var projectIntegrationService = GetProjectInvitationService();

            var project = new Project("Test", Guid.NewGuid());
            project.SetProjectId();
            var userId = Guid.NewGuid();

            projectIntegrationService.ValidateAndInsertOrUpdate(
                new ProjectInvitation(
                    project, TestHelper.VALID_EMAIL_ADDRESS), userId);
            projectIntegrationService.SaveChanges();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        projectIntegrationService.ValidateModel(
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