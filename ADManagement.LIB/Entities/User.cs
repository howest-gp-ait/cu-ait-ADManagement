using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ADManagement.LIB.Entities
{
    public class User
    {
        public string DisplayName { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Tag { get; set; }
        public List<Group> MemberOff { get; set; }

        public string SamAccountName { get; set; }
        public bool IsAccountLockedOut { get; set; }
        public DateTime? AccountExpirationDate { get; set; }
        public string OU { get; set; }


        public User()
        {
            MemberOff = new List<Group>();
        }
        public override string ToString()
        {
            return $"{DisplayName}";
        }

        public string CreateThisUser(string ou, string paswoord)
        {
            using (var pc = new PrincipalContext(ContextType.Domain,ActiveDirectory.ADDomainNameShort,ou))
            {
                using (var up = new UserPrincipal(pc))
                {
                    up.DisplayName = DisplayName;
                    up.UserPrincipalName = LoginName;
                    up.GivenName = FirstName;
                    up.Surname = LastName;
                    up.EmailAddress = Email;
                    up.SamAccountName = SamAccountName;
                    up.SetPassword(paswoord);
                    up.AccountExpirationDate = AccountExpirationDate;
                    up.Enabled = true;
                    try
                    {
                        up.Save();
                    }
                    catch(Exception fout)
                    {
                        return fout.Message;
                    }
                }
            }

            return "";
        }
        public string UpdateThisUser(string nieuweOU, string paswoord)
        {

            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, SamAccountName);

            // zoek de OU op waarin deze gebruiker zich bevindt
            DirectoryEntry dirEntryUser = (DirectoryEntry)up.GetUnderlyingObject();
            string oudeOU = dirEntryUser.Parent.Path;
            if (nieuweOU.Substring(0, 4) != "LDAP")
                nieuweOU = ActiveDirectory.LDAPShort + nieuweOU;
            //indien nieuwe OU verschillend van oude OU : verplaatsen
            if(nieuweOU != oudeOU)
            {
                DirectoryEntry directoryEntryNieuweOU = new DirectoryEntry(nieuweOU);
                dirEntryUser.MoveTo(directoryEntryNieuweOU);
            }

            up.DisplayName = DisplayName;
            up.UserPrincipalName = LoginName;
            up.GivenName = FirstName;
            up.Surname = LastName;
            up.EmailAddress = Email;
            up.SamAccountName = SamAccountName;
            if(paswoord != "")
                up.SetPassword(paswoord);
            up.AccountExpirationDate = AccountExpirationDate;
            up.Enabled = true;
            try
            {
                up.Save();
            }
            catch (Exception fout)
            {
                return fout.Message;
            }


            return "";
        }
        public void DeleteThisUser()
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, SamAccountName);
            up.Delete();

        }
    }

    //future use : PrincipalContext & UserPrincipal
}
