using Arragro.Common.BusinessRules;
using System;
using System.ComponentModel.DataAnnotations;

namespace Timesheets.DataLayer.Models
{
    public class ProjectInvitation : Auditable<Guid>
    {
        public Guid ProjectInvitationId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? UserId { get; set; }
        [MaxLength(254)]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        public Guid InvitationCode { get; set; }
        public bool InvitationSent { get; set; }
        public bool? InvitationAccepted { get; set; }
    }
}