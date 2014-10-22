using Arragro.Common.ServiceBase;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Timesheets.DataLayer.Interfaces;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Services
{
    public class TimesheetEntryService : AuditableService<ITimesheetEntryRepository, TimesheetEntry, Guid, Guid>
    {
        public const string USER_IS_NOT_NULL_NOT_SET = "UserId is not null and is not set.";
        public const string DATE_NOT_SET = "The date must be set.";
        public const string HOURS_MORE_THAN_24 = "The number of hours cannot be more than 24.";
        public const string HOURS_MORE_THAN_24_WITH_RELATED_TIMESHEETS = "The number of hours cannot be more than 24 in relation to other time sheet entries.  You only have {0} hours remaining.";

        public TimesheetEntryService(ITimesheetEntryRepository repository)
            : base(repository)
        {
        }

        private void HasUserId(TimesheetEntry model)
        {
            if (model.UserId == default(Guid))
                RulesException.ErrorFor(x => x.UserId, USER_IS_NOT_NULL_NOT_SET);
        }

        private void DateIsValid(TimesheetEntry model)
        {
            if (model.Date == default(DateTime))
                RulesException.ErrorFor(t => t.Date, DATE_NOT_SET);
        }

        private void NumberOfHoursIsValid(TimesheetEntry model)
        {
            if (model.NumberOfHours > 0 && model.NumberOfHours <= 24)
            {
                var date = model.Date.Date;
                var timesheetEnties = Repository.All().Where(
                    t => t.UserId == model.UserId
                      && t.Date == date).ToList();
                var numberOfCurrentHours = timesheetEnties.Sum(t => t.NumberOfHours);

                if ((numberOfCurrentHours + model.NumberOfHours) > 24)
                    RulesException.ErrorFor(
                        t => t.NumberOfHours,
                        string.Format(HOURS_MORE_THAN_24_WITH_RELATED_TIMESHEETS, 24 - numberOfCurrentHours));
            }
        }

        protected override void ValidateModelRules(TimesheetEntry model)
        {
            HasUserId(model);
            DateIsValid(model);
            NumberOfHoursIsValid(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public override TimesheetEntry InsertOrUpdate(TimesheetEntry model, Guid userId)
        {
            var add = default(Guid) == model.TimesheetEntryId;
            if (add) model.SetTimesheetEntryId();
            AddOrUpdateAudit(model, userId, add);
            return Repository.InsertOrUpdate(model, add);
        }

        public IEnumerable<TimesheetEntry> GetLastMonthsTimesheets(Guid userId)
        {
            var date = DateTime.Now.AddMonths(-1).Date;
            return Repository.All()
                .Where(t => t.UserId == userId
                         && t.Date > date).ToList();
        }

        public IEnumerable<TimesheetEntry> GetTimesheetsByRange(
            Guid userId,
            DateTime fromDate, DateTime toDate)
        {
            var fromShortDate = fromDate.Date;
            var toShortDate = toDate.Date;
            return Repository.All()
                .Where(t => t.UserId == userId
                         && t.Date >= fromDate
                         && t.Date <= toDate).ToList();
        }
    }
}