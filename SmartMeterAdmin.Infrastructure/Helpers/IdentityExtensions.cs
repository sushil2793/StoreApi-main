using SmartMeterAdmin.Infrastructure.Enumeration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SmartMeterAdmin.Infrastructure.Helpers
{
    public static class IdentityExtensions
    {

        //get userid claim value
        public static int GetUserId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).Claims.Where(c => c.Type == "UserID").Select(c => c.Value).SingleOrDefault();
            return (claim != null) ? int.Parse(claim) : -1;
        }
        public static string GetCompanyName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).Claims.Where(c => c.Type == "OwnerName").Select(c => c.Value).SingleOrDefault();
            return claim;
        }
        public static string GetUserRoleName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).Claims.Where(c => c.Type == "RoleName").Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrEmpty(claim))
                return RoleEnum.Unknown.ToString();
            var role = Enum.Parse(typeof(RoleEnum), claim);
            return (RoleEnum)role == RoleEnum.Electrician ? RoleEnum.Subcontractor.ToString() : claim;
        }

        //get Name claim value (login user ka full name)
        public static string GetName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).Claims.Where(c => c.Type == "Name").Select(c => c.Value).SingleOrDefault();
            return claim;
        }
    }
}
