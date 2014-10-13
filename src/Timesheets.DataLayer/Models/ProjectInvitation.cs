using Arragro.Common.BusinessRules;
using System;

namespace Timesheets.DataLayer.Models
{
    public class ProjectInvitation : Auditable<Guid>
    {
        public Guid InvitationId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public Guid InvitationCode { get; set; }
        public bool InvitationSent { get; set; }
        public bool? InvitationAccepted { get; set; }
    }
}