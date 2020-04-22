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
    public class GroupsInGroups
    {
        public List<Group> Groups { get; set; }
        public GroupsInGroups(string sAMAccountName)
        {
            Groups = new List<Group>();


            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
            foreach (var item in gp.Members)
            {
                if (item is GroupPrincipal)
                {
                    GroupPrincipal up = (GroupPrincipal)item;
                    Group group = new Group();
                    group.Name = item.Name;
                    group.UserMembers = new UsersInGroup(item.SamAccountName).Users;
                    group.GroupMembers = new GroupsInGroups(item.SamAccountName).Groups;

                    Groups.Add(group);
                }
            }

        }
    }
}
