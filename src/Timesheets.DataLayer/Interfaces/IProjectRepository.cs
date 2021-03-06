﻿using Arragro.Common.Repository;
using System;
using Timesheets.DataLayer.Models;

namespace Timesheets.DataLayer.Interfaces
{
    public interface IProjectRepository : IRepository<Project, Guid>
    {
    }
}