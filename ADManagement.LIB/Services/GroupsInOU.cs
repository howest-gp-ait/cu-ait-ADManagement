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
    public class GroupsInOU
    {
        public List<Group> Groups { get; set; }

        public GroupsInOU(string OUPath)
        {
            if (OUPath.Substring(0, 4) != "LDAP")
                OUPath = ActiveDirectory.LDAPShort + OUPath;
            Groups = new List<Group>();
            DirectorySearcher zoeker = new DirectorySearcher(new DirectoryEntry(OUPath))
            {
                Filter = "(objectCategory=group)",
                SearchScope = SearchScope.OneLevel
            };
            foreach (SearchResult resultaat in zoeker.FindAll())
            {
                PrincipalContext pc = new PrincipalContext(ContextType.Domain);
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, resultaat.Properties["name"][0].ToString());
                Group group = new Group();
                group.Name = Helper.HandleNull(resultaat.Properties["name"][0]);
                // zoek de OU op waarin deze groep zich bevindt
                DirectoryEntry dirEntryGroup = (DirectoryEntry)gp.GetUnderlyingObject();
                DirectoryEntry dirEntOU = dirEntryGroup.Parent;
                group.OU = dirEntOU.Path.Replace("LDAP://", "");

                group.UserMembers = new UsersInGroup(resultaat.Properties["sAMAccountName"][0].ToString()).Users;
                group.GroupMembers = new GroupsInGroups(resultaat.Properties["sAMAccountName"][0].ToString()).Groups;
                Groups.Add(group);
            }


        }
    }
}
