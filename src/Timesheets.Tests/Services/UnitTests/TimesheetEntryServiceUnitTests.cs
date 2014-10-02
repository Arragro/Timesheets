﻿using Arragro.Common.BusinessRules;
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
            var unityContainer = InMemoryUnityContainer.GetInMemoryContainer();
            return unityContainer.Resolve<TimesheetEntryService>();
        }

        [Fact]
        public void Timesheet_attributes_validate_correctly()
        {
            var timesheetEntryService = GetTimesheetEntryService();

            var errors = timesheetEntryService.ValidateModel(new TimesheetEntry());
            Assert.Equal(0, errors.Count);

            errors = timesheetEntryService.ValidateModel(
                        new TimesheetEntry
                        {
                            Description = new String('X', 51)
                        });
            Assert.Equal(1, errors.Count);
            Assert.NotNull(errors.SingleOrDefault(x => x.MemberNames.Contains("Description")));
        }

        [Fact]
        private void TimesheetEntry_fails_with_no_UserId()
        {
            var timesheetEntryService = GetTimesheetEntryService();

            var timesheetEntry = new TimesheetEntry { Date = DateTime.Now };

            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.EnsureValidModel(timesheetEntry);
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
            var timesheetEntry = new TimesheetEntry { UserId = 1 };
            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.EnsureValidModel(timesheetEntry);
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
            var timesheetEntry = new TimesheetEntry { UserId = 1, Date = DateTime.Now, NumberOfHours = 25 };
            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.EnsureValidModel(timesheetEntry);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ex.Errors[0].Message, TimesheetEntryService.HOURS_MORE_THAN_24);
                        throw ex;
                    }
                });
        }

        [Fact]
        private void TimesheetEntry_fails_on_too_many_hours_multiple_entries()
        {
            var timesheetEntryService = GetTimesheetEntryService();
            var userId = 1;

            var timesheetEntry1 = new TimesheetEntry
            {
                UserId = userId,
                Date = DateTime.Now,
                NumberOfHours = 23.5M
            };
            timesheetEntry1 = timesheetEntryService.InsertOrUpdate(timesheetEntry1, userId);
            Assert.NotSame(default(int), timesheetEntry1.TimesheetEntryId);

            var timesheetEntry2 = new TimesheetEntry
            {
                UserId = userId,
                Date = DateTime.Now,
                NumberOfHours = 1
            };

            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.EnsureValidModel(timesheetEntry2);
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
            timesheetEntry2.NumberOfHours = 0.5M;
            timesheetEntry2 = timesheetEntryService.InsertOrUpdate(timesheetEntry2, userId);
            Assert.NotSame(default(int), timesheetEntry2.TimesheetEntryId);

            Assert.Throws<RulesException<TimesheetEntry>>(
                () =>
                {
                    try
                    {
                        timesheetEntryService.EnsureValidModel(
                            new TimesheetEntry
                            {
                                UserId = userId,
                                Date = DateTime.Now,
                                NumberOfHours = 1
                            });
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