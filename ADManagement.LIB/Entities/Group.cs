using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace ADManagement.LIB.Entities
{
    public class Group
    {
        public string Name { get; set; }
        public List<User> UserMembers { get; set; }
        public List<Group> GroupMembers { get; set; }
        public string OU { get; set; }
        public Group()
        {
            UserMembers = new List<User>();
            GroupMembers = new List<Group>();
        }
        public override string ToString()
        {
            return Name;

        }

        public string CreateThisGroup(string ou)
        {
            using (var pc = new PrincipalContext(ContextType.Domain, ActiveDirectory.ADDomainNameShort, ou))
            {
                using (var gp = new GroupPrincipal(pc))
                {
                    gp.Name = Name;
                    try
                    {
                        gp.Save();
                    }
                    catch (Exception fout)
                    {
                        return fout.Message;
                    }
                }
            }
            return "";
        }
        public string UpdateThisGroup(string newOU, string newName)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, Name);

            // zoek de OU op waarin deze groep zich bevindt
            DirectoryEntry dirEntryGroup = (DirectoryEntry)gp.GetUnderlyingObject();
            string oldOU = dirEntryGroup.Parent.Path;
            if (newOU.Substring(0, 4) != "LDAP")
                newOU = ActiveDirectory.LDAPShort + newOU;
            //indien nieuwe OU verschillend van oude OU : verplaatsen
            if (newOU != oldOU)
            {
                DirectoryEntry directoryEntryNewOU = new DirectoryEntry(newOU);
                dirEntryGroup.MoveTo(directoryEntryNewOU);
            }
            try
            {


                dirEntryGroup.Rename("CN=" +newName);
                dirEntryGroup.CommitChanges();

            }
            catch (Exception fout)
            {
                return fout.Message;
            }
            return "";
        }
        public void DeleteThisGroup()
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, Name);
            gp.Delete();

        }
    }
}
