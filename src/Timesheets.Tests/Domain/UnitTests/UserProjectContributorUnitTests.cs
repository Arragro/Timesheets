using Arragro.Common.BusinessRules;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.BusinessLayer.Domain;
using Timesheets.DataLayer.Enums;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Domain.UnitTests
{
    public class UserProjectContributorUnitTests
    {
        [Fact]
        public void change_of_project_owner_fails_with_invalid_project()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    using (var testHelper = new TestHelper())
                    {
                        var user = TestHelper.GetOwnerUser();
                        var project = new Project("Test 1", user.Id);
                        var projectContrbutor = new ProjectContributor(project, Guid.NewGuid());
                        var userProjectContributor = testHelper.GetUserProjectContributor(user);

                        try
                        {
                            userProjectContributor.ChangeProjectContributorsRole(projectContrbutor, ContributorRole.ReadOnlyAdministrator);
                        }
                        catch (ArgumentNullException ex)
                        {
                            Assert.Equal(ex.ParamName, "project");
                            throw;
                        }
                    }
                });
        }

        [Fact]
        public void change_of_project_owner_fails_with_project_owner_error()
        {
            Assert.Throws<RulesException>(
                () =>
                {
                    using (var testHelper = new TestHelper())
                    {
                        var ownerUser = TestHelper.GetOwnerUser();
                        var result = DomainObjectBuilder.CreateAndGetProjectContributors(
                            testHelper, ownerUser, new[] { TestHelper.VALID_EMAIL_ADDRESS }).First();

                        var userProjectContributor = testHelper.GetUserProjectContributor(ownerUser);
                        var projectContributor = userProjectContributor.GetProjectContributor(result.Item1, ownerUser.Id);

                        try
                        {
                            userProjectContributor.ChangeProjectContributorsRole(projectContributor, ContributorRole.ReadOnlyAdministrator);
                        }
                        catch (RulesException ex)
                        {
                            Assert.Equal(ex.Errors[0].Message, UserProjectContributor.OWNER_ROLE_CANNOT_BE_CHANGED);
                            throw;
                        }
                    }
                });
        }

        [Fact]
        public void user_cannot_get_project_contributors_because_they_dont_have_permission()
        {
            Assert.Throws<RulesException>(
                () =>
                {
                    using (var testHelper = new TestHelper())
                    {
                        var ownerUser = TestHelper.GetOwnerUser();
                        var result = DomainObjectBuilder.CreateAndGetProjectContributors(
                            testHelper, ownerUser, new[] { TestHelper.VALID_EMAIL_ADDRESS }).First();

                        var userProjects = testHelper.GetUserProjects(ownerUser);
                        var project = userProjects.GetUserProjects().First();
                        var userProjectContributor = testHelper.GetUserProjectContributor(result.Item4);

                        try
                        {
                            userProjectContributor.GetProjectContributors(project);
                        }
                        catch (RulesException ex)
                        {
                            Assert.Equal(ex.Errors[0].Message, SecurityRules.USER_IS_NOT_AUTHORISED_TO_READ);
                            throw;
                        }
                    }
                });
        }

        [Fact]
        public void change_of_project_contributor_role_by_owner()
        {
            using (var testHelper = new TestHelper())
            {
                var ownerUser = TestHelper.GetOwnerUser();
                var result = DomainObjectBuilder.CreateAndGetProjectContributors(
                    testHelper, ownerUser, new[] { TestHelper.VALID_EMAIL_ADDRESS }).First();
                var userProjectContributor = testHelper.GetUserProjectContributor(ownerUser);
                var barProjectContributor = result.Item3;

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.Administrator);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.Administrator);

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.ReadOnlyAdministrator);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.ReadOnlyAdministrator);

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.User);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.User);
            }
        }

        [Fact]
        public void change_of_project_contributor_role_by_administrator()
        {
            using (var testHelper = new TestHelper())
            {
                var ownerUser = TestHelper.GetOwnerUser();
                var result = DomainObjectBuilder.CreateAndGetProjectContributors(
                    testHelper, ownerUser, new[] { TestHelper.VALID_EMAIL_ADDRESS }).First();
                var userProjectContributor = testHelper.GetUserProjectContributor(ownerUser);
                var barProjectContributor = result.Item3;

                barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.Administrator);

                Assert.Equal(barProjectContributor.ContributorRole, ContributorRole.Administrator);
            }
        }

        [Fact]
        public void change_of_project_contributor_role_fails_when_not_adminiatrator()
        {
            Assert.Throws<RulesException>(() =>
                {
                    try
                    {
                        using (var testHelper = new TestHelper())
                        {
                            var ownerUser = TestHelper.GetOwnerUser();
                            var result = DomainObjectBuilder.CreateAndGetProjectContributors(
                                testHelper, ownerUser, new[] { TestHelper.VALID_EMAIL_ADDRESS }).First();
                            var userProjectContributor = testHelper.GetUserProjectContributor(result.Item4);

                            var barProjectContributor = result.Item3;
                            barProjectContributor = userProjectContributor.ChangeProjectContributorsRole(barProjectContributor, ContributorRole.Administrator);
                        }
                    }
                    catch(RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, SecurityRules.USER_IS_NOT_AUTHORISED_TO_MODIFY);
                        throw;
                    }
                });
        }
    }
}