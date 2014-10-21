using Arragro.Common.BusinessRules;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;
using Xunit;

namespace Timesheets.Tests.Services.UnitTests
{
    public class TimesheetEntryServiceUnitTests
    {
        private TimesheetEntryService GetTimesheetEntryService()
        {
#if !INTEGRATION_TESTS
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
#else
            var unityContainer = EF6UnityContainer.GetEF6Container();
#endif
            return unityContainer.Resolve<TimesheetEntryService>();
        }

        [Fact]
        public void TimesheetEntry_attributes_validate_correctly()
        {
            var timesheetEntryService = GetTimesheetEntryService();

            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.ValidateModel(
                                new TimesheetEntry(
                                    new Guid(), new DateTime(),
                                    0, new String('X', 51)));
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(3, ex.Errors.Count);
                        Assert.True(ex.ContainsErrorForProperty("Description"));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == TimesheetEntryService.REQUIRED_USERID));
                        Assert.NotNull(ex.Errors.SingleOrDefault(x => x.Message == TimesheetEntryService.DATE_NOT_SET));
                        throw;
                    }
                });
        }

        [Fact]
        private void TimesheetEntry_fails_with_no_UserId()
        {
            var timesheetEntryService = GetTimesheetEntryService();

            var timesheetEntry = new TimesheetEntry(new Guid(), DateTime.Now, 0);

            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.ValidateModel(timesheetEntry);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, TimesheetEntryService.REQUIRED_USERID);
                        throw ex;
                    }
                });
        }

        [Fact]
        private void TimesheetEntry_fails_on_unset_date()
        {
            var timesheetEntryService = GetTimesheetEntryService();
            var timesheetEntry = new TimesheetEntry(Guid.NewGuid(), new DateTime(), 0);
            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.ValidateModel(timesheetEntry);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, TimesheetEntryService.DATE_NOT_SET);
                        throw ex;
                    }
                });
        }

        [Fact]
        private void TimesheetEntry_fails_on_too_many_hours()
        {
            var timesheetEntryService = GetTimesheetEntryService();
            var timesheetEntry = new TimesheetEntry(Guid.NewGuid(), DateTime.Now, 25);
            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.ValidateModel(timesheetEntry);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(1, ex.Errors.Count);
                        Assert.True(ex.ContainsErrorForProperty("NumberOfHours"));
                        throw ex;
                    }
                });
        }

        [Fact]
        private void TimesheetEntry_fails_on_too_many_hours_multiple_entries()
        {
            var timesheetEntryService = GetTimesheetEntryService();
            var userId = Guid.NewGuid();

            var timesheetEntry1 = new TimesheetEntry(
                userId, DateTime.Now, 23.5M);
            timesheetEntry1 = timesheetEntryService.ValidateAndInsertOrUpdate(timesheetEntry1, userId);
            timesheetEntryService.SaveChanges();
            Assert.NotSame(default(Guid), timesheetEntry1.TimesheetEntryId);

            var timesheetEntry2 = new TimesheetEntry(
                userId, DateTime.Now, 1);

            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.ValidateModel(timesheetEntry2);
                    }
                    catch (RulesException ex)
                    {
                        var message = string.Format(
                            TimesheetEntryService.HOURS_MORE_THAN_24_WITH_RELATED_TIMESHEETS,
                            0.5M);
                        Assert.Equal(ex.Errors[0].Message, message);
                        throw ex;
                    }
                });

            timesheetEntryService.RulesException.Errors.Clear();
            timesheetEntry2.ChangeNumberOfHours(0.5M);
            timesheetEntry2 = timesheetEntryService.ValidateAndInsertOrUpdate(timesheetEntry2, userId);
            timesheetEntryService.SaveChanges();
            Assert.NotSame(default(Guid), timesheetEntry2.TimesheetEntryId);

            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.ValidateModel(
                            new TimesheetEntry(
                                userId, DateTime.Now, 1));
                    }
                    catch (RulesException ex)
                    {
                        var message = string.Format(
                            TimesheetEntryService.HOURS_MORE_THAN_24_WITH_RELATED_TIMESHEETS,
                            0.0M);
                        Assert.Equal(ex.Errors[0].Message, message);
                        throw ex;
                    }
                });
        }
    }
}