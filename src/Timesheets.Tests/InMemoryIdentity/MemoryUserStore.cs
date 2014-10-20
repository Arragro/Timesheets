using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Timesheets.Tests.InMemoryIdentity
{
    public class MemoryUserStore :
                    IUserStore<MemoryUser>, IUserLoginStore<MemoryUser>, IUserRoleStore<MemoryUser>,
                    IUserClaimStore<MemoryUser>, IUserPasswordStore<MemoryUser>, IUserSecurityStampStore<MemoryUser>
    {
        private Dictionary<string, MemoryUser> _users = new Dictionary<string, MemoryUser>();
        private Dictionary<UserLoginInfo, MemoryUser> _logins = new Dictionary<UserLoginInfo, MemoryUser>();

        public Task CreateAsync(MemoryUser user)
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task UpdateAsync(MemoryUser user)
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task<MemoryUser> FindByIdAsync(string userId)
        {
            if (_users.ContainsKey(userId))
            {
                return Task.FromResult(_users[userId]);
            }
            return Task.FromResult<MemoryUser>(null);
        }

        public void Dispose()
        {
        }

        public IQueryable<MemoryUser> Users
        {
            get
            {
                return _users.Values.AsQueryable();
            }
        }

        public Task<MemoryUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(Users.Where(u => u.UserName.ToUpper() == userName.ToUpper()).FirstOrDefault());
        }

        public Task AddLoginAsync(MemoryUser user, UserLoginInfo login)
        {
            user.Logins.Add(login);
            _logins[login] = user;
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(MemoryUser user, UserLoginInfo login)
        {
            var logs = user.Logins.Where(l => l.ProviderKey == login.ProviderKey && l.LoginProvider == login.LoginProvider).ToList();
            foreach (var l in logs)
            {
                user.Logins.Remove(l);
                _logins[l] = null;
            }
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(MemoryUser user)
        {
            return Task.FromResult(user.Logins);
        }

        public Task<MemoryUser> FindAsync(UserLoginInfo login)
        {
            if (_logins.ContainsKey(login))
            {
                return Task.FromResult(_logins[login]);
            }
            return Task.FromResult<MemoryUser>(null);
        }

        public Task AddToRoleAsync(MemoryUser user, string role)
        {
            user.Roles.Add(role);
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(MemoryUser user, string role)
        {
            user.Roles.Remove(role);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(MemoryUser user)
        {
            return Task.FromResult(user.Roles);
        }

        public Task<bool> IsInRoleAsync(MemoryUser user, string role)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(MemoryUser user)
        {
            return Task.FromResult(user.Claims);
        }

        public Task AddClaimAsync(MemoryUser user, Claim claim)
        {
            user.Claims.Add(claim);
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(MemoryUser user, Claim claim)
        {
            user.Claims.Remove(claim);
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(MemoryUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(MemoryUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task SetSecurityStampAsync(MemoryUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(MemoryUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task DeleteAsync(MemoryUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(MemoryUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}