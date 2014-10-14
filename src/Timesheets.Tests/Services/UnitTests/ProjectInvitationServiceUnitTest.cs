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
        private const string VALID_EMAIL_ADDRESS = "email.is.good@test.com";

        [Fact]
        public void ProjectInvitationService_validates_model_attributes()
        {
            var projectIntegrationService = GetProjectInvitationService();

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        projectIntegrationService.ValidateModel(new ProjectInvitation());
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(3, ex.Errors.Count);
                        Assert.NotNull(ex.ContainsErrorForProperty(".EmailAddress"));
                        Assert.NotNull(ex.ContainsErrorForProperty(".ProjectId"));
                        Assert.NotNull(ex.ContainsErrorForProperty(".InvitationCode"));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectInvitationService.PROJECTID_NOT_SET));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectInvitationService.INVITATIONCODE_IS_NOT_SET));
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
                        projectIntegrationService.ValidateModel(
                            new ProjectInvitation
                            {
                                ProjectId = Guid.NewGuid(),
                                InvitationCode = Guid.NewGuid(),
                                EmailAddress = "bad_email"
                            });
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

            projectIntegrationService.ValidateModel(
                            new ProjectInvitation
                            {
                                ProjectId = Guid.NewGuid(),
                                InvitationCode = Guid.NewGuid(),
                                EmailAddress = VALID_EMAIL_ADDRESS
                            });
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
                        projectIntegrationService.ValidateModel(
                            new ProjectInvitation
                            {
                                ProjectId = Guid.NewGuid(),
                                InvitationCode = Guid.NewGuid(),
                                EmailAddress = VALID_EMAIL_ADDRESS,
                                UserId = new Guid()
                            });
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

            Assert.Throws<RulesException<ProjectInvitation>>(
                () =>
                {
                    try
                    {
                        projectIntegrationService.ValidateModel(
                            new ProjectInvitation
                            {
                                ProjectId = Guid.NewGuid(),
                                InvitationCode = Guid.NewGuid(),
                                EmailAddress = VALID_EMAIL_ADDRESS,
                                InvitationSent = false,
                                InvitationAccepted = true
                            });
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(1, ex.Errors.Count);
                        Assert.NotNull(ex.ContainsErrorForProperty(".InvitationAccepted"));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == ProjectInvitationService.INVITATION_ACCEPTED_BEFORE_SENT));
                        throw ex;
                    }
                });
        }
    }
}