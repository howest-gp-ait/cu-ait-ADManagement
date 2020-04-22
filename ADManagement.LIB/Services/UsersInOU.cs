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
    public class UsersInOU
    {
        public List<User> Users { get; set; }
        public UsersInOU(string OUPath)
        {

            if(OUPath.Substring(0,4) != "LDAP")
                OUPath = ActiveDirectory.LDAPShort + OUPath;
            Users = new List<User>();

            DirectorySearcher zoeker = new DirectorySearcher(new DirectoryEntry(OUPath))
            {
                Filter = "(objectCategory=person)",
                SearchScope = SearchScope.OneLevel
            };
            foreach (SearchResult resultaat in zoeker.FindAll())
            {
                PrincipalContext pc = new PrincipalContext(ContextType.Domain);
                UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, resultaat.Properties["sAMAccountName"][0].ToString());


                User user = new User();
                user.FirstName = up.GivenName;              // Helper.HandleNull(resultaat.Properties["GivenName"][0]);
                user.LastName = up.Name;                    // Helper.HandleNull(resultaat.Properties["sn"][0]);
                user.Email = up.EmailAddress;               //Helper.HandleNull(resultaat.Properties["mail"]);
                user.LoginName = up.UserPrincipalName;      // resultaat.Properties["userPrincipalName"][0].ToString();
                user.DisplayName = up.DisplayName;          // Helper.HandleNull(resultaat.Properties["displayName"][0]);

                user.SamAccountName = up.SamAccountName;
                user.IsAccountLockedOut = up.IsAccountLockedOut();
                user.AccountExpirationDate = up.AccountExpirationDate;
                // zoek de OU op waarin deze gebruiker zich bevindt
                DirectoryEntry dirEntryUser = (DirectoryEntry)up.GetUnderlyingObject();
                DirectoryEntry dirEntOU = dirEntryUser.Parent;
                //user.OU = dirEntOU.Name;
                user.OU = dirEntOU.Path.Replace("LDAP://","");

                user.Tag = resultaat.Path;
                user.MemberOff = UserGroupMembership.GetUserGroupMembership(resultaat.Properties["sAMAccountName"][0].ToString());
                Users.Add(user);
            }

        }
    }


 

}
