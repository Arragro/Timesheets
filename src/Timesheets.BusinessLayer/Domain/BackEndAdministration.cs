using System;
using Timesheets.BusinessLayer.Services;
using Timesheets.DataLayer.Models;

namespace Timesheets.BusinessLayer.Domain
{
    public class BackEndAdministration
    {
        public Guid AdministrationUserId = new Guid("1f3350d2-358c-46a4-9e78-f168bf69342f");

        private readonly ProjectInvitationService _projectInvitationService;

        public BackEndAdministration(
            ProjectInvitationService projectInvitationService)
        {
            _projectInvitationService = projectInvitationService;
        }

        public void SetProjectInvitationSent(ProjectInvitation projectInvitation)
        {
            projectInvitation.SetProjectInvitationSent(true);
            _projectInvitationService.InsertOrUpdate(projectInvitation, AdministrationUserId);
            _projectInvitationService.SaveChanges();
        }
    }
}