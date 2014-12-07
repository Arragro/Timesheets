using Arragro.Common.BusinessRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    [Serializable]
    public class Project : Auditable<Guid>
    {
        public Guid ProjectId { get; private set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; private set; }
        public Guid OwnerUserId { get; private set; }
        [MaxLength(20)]
        public string Code { get; private set; }
        [MaxLength(20)]
        public string PurchaseOrderNumber { get; private set; }
        public decimal? Budget { get; private set; }
        public decimal? WeeklyContributorHoursLimit { get; private set; }
        public bool RequiresApproval { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public int test { get; set; }

        public ICollection<ProjectContributor> ProjectContributors { get; private set; }

        protected Project()
        {
            ProjectContributors = new HashSet<ProjectContributor>();
        }

        public Project(
            string name, Guid ownerUserId, string code = null, string purchaseOrderNumber = null,
            decimal? budget = null, decimal? weeklContributorHoursLimit = null,
            bool requiresApproval = false, DateTime? startDate = null,
            DateTime? endDate = null) : this()
        {
            Name = name;
            OwnerUserId = ownerUserId;
            Code = code;
            PurchaseOrderNumber = purchaseOrderNumber;
            Budget = budget;
            WeeklyContributorHoursLimit = weeklContributorHoursLimit;
            RequiresApproval = requiresApproval;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void ChangeProjectOwner(Guid newOwnerUserId)
        {
            OwnerUserId = newOwnerUserId;
        }

        public void SetProjectId()
        {
            if (ProjectId != default(Guid)) throw new Exception("The ProjectId is already set.");
            ProjectId = Guid.NewGuid();
        }
    }
}