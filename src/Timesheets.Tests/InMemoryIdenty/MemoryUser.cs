﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Timesheets.Tests.InMemoryIdenty
{
    public class MemoryUser : IUser
    {
        private readonly IList<UserLoginInfo> _logins;
        private readonly IList<Claim> _claims;
        private readonly IList<string> _roles;

        public MemoryUser(string name)
        {
            Id = Guid.NewGuid().ToString();
            _logins = new List<UserLoginInfo>();
            _claims = new List<Claim>();
            _roles = new List<string>();
            UserName = name;
        }

        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }

        /// <summary>
        /// The salted/hashed form of the user password
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        public IList<UserLoginInfo> Logins { get { return _logins; } }

        public IList<Claim> Claims { get { return _claims; } }

        public IList<string> Roles { get { return _roles; } }
    }
}
