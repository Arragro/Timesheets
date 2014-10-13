using System;
using Timesheets.DataLayer.Models.Bases;

namespace Timesheets.DataLayer.Models
{
    public class ProjectContributor : ContributorDetails
    {
        public Guid ProjectContributorId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
    }
}