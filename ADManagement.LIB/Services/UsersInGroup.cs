using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADManagement.LIB.Entities;
using ADManagement.LIB.Helpers;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ADManagement.LIB.Services
{
    public class UsersInGroup
    {
        public List<User> Users { get; set; }
        public UsersInGroup(string sAMAccountName)
        {
            Users = new List<User>();

            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
            foreach(var item in gp.Members)
            {
                if(item is UserPrincipal)
                {
                    UserPrincipal up = (UserPrincipal)item;
                    User user = new User();
                    user.DisplayName = up.DisplayName;
                    user.Email = up.EmailAddress;
                    user.FirstName = up.GivenName;
                    user.LastName = up.Surname;
                    user.LoginName = up.UserPrincipalName;
                    user.SamAccountName = up.SamAccountName;
                    user.IsAccountLockedOut = up.IsAccountLockedOut();
                    user.AccountExpirationDate = up.AccountExpirationDate;

                    // zoek de OU op waarin deze gebruiker zich bevindt
                    DirectoryEntry dirEntryUser = (DirectoryEntry)up.GetUnderlyingObject();
                    DirectoryEntry dirEntOU = dirEntryUser.Parent;
                    user.OU = dirEntOU.Name;

                    user.Tag = up.DistinguishedName;
                    Users.Add(user);
                }
            }
        }
    }
}
