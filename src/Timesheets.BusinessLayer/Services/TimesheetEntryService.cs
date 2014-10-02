using Arragro.Common.ServiceBase;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Services
{
    public class TimesheetEntryService : AuditableService<ITimesheetEntryRepository, TimesheetEntry, Guid, int>
    {
        public const string REQUIRED_USERID = "The time sheet entry must have a UserId";
        public const string DATE_NOT_SET = "The date must be set";
        public const string HOURS_MORE_THAN_24 = "The number of hours cannot be more than 24";
        public const string HOURS_MORE_THAN_24_WITH_RELATED_TIMESHEETS = "The number of hours cannot be more than 24 in relation to other time sheet entries.  You only have {0} hours remaining";

        public TimesheetEntryService(ITimesheetEntryRepository repository)
            : base(repository)
        {
        }

        private void HasUserId(TimesheetEntry model)
        {
            if (model.UserId == default(int))
                RulesException.ErrorFor(x => x.UserId, REQUIRED_USERID);
        }

        private void DateIsValid(TimesheetEntry model)
        {
            if (model.Date == default(DateTime))
                RulesException.ErrorFor(t => t.Date, DATE_NOT_SET);
        }

        private void NumberOfHoursIsValid(TimesheetEntry model)
        {
            if (model.NumberOfHours > 24)
                RulesException.ErrorFor(t => t.NumberOfHours, HOURS_MORE_THAN_24);

            var timesheetEnties = Repository.All().Where(
                t => t.UserId == model.UserId
                  && t.Date.Date == model.Date.Date);
            var numberOfCurrentHours = timesheetEnties.Sum(t => t.NumberOfHours);

            if ((numberOfCurrentHours + model.NumberOfHours) > 24)
                RulesException.ErrorFor(
                    t => t.NumberOfHours,
                    string.Format(HOURS_MORE_THAN_24_WITH_RELATED_TIMESHEETS, 24 - numberOfCurrentHours));
        }

        public override void EnsureValidModel(TimesheetEntry model, params object[] relatedModels)
        {
            HasUserId(model);
            DateIsValid(model);
            NumberOfHoursIsValid(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public override TimesheetEntry InsertOrUpdate(TimesheetEntry model, int userId)
        {
            EnsureValidModel(model);
            var add = default(Guid) == model.TimesheetEntryId;
            AddOrUpdateAudit(model, userId, add);
            return Repository.InsertOrUpdate(model, add);
        }
    }
}