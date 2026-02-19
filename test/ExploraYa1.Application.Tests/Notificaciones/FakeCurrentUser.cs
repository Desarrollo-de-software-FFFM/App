using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace ExploraYa1.Notificaciones
{
    public class FakeCurrentUser : ICurrentUser
    {
        public Guid? Id { get; }

        public FakeCurrentUser(Guid id)
        {
            Id = id;
        }

        public string UserName => "test-user";
        public string Name => null;
        public string SurName => null;
        public string Email => null;
        public string PhoneNumber => null;
        public bool PhoneNumberVerified => false;
        public string EmailVerified => null;
        public string TenantId => null;
        public string[] Roles => Array.Empty<string>();
        public bool IsInRole(string role) => false;
        public bool IsAuthenticated => true;

        bool ICurrentUser.EmailVerified => throw new NotImplementedException();

        Guid? ICurrentUser.TenantId => throw new NotImplementedException();

        public IEnumerable<string> GetAllRoles() => Roles;

        public Claim? FindClaim(string claimType)
        {
            throw new NotImplementedException();
        }

        public Claim[] FindClaims(string claimType)
        {
            throw new NotImplementedException();
        }

        public Claim[] GetAllClaims()
        {
            throw new NotImplementedException();
        }
    }
}
