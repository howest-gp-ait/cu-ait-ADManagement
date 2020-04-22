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
    public class GroupMemberShip
    {
        public static List<string> MemberUsers(string groupname)
        {
            List<string> users = new List<string>();
            if (groupname != null && groupname != "")
            {
                PrincipalContext PC = new PrincipalContext(ContextType.Domain);
                GroupPrincipal GP = GroupPrincipal.FindByIdentity(PC, groupname);
                foreach (Principal p in GP.Members)
                {
                    if (p is UserPrincipal)
                    {
                        users.Add(((UserPrincipal)p).SamAccountName);

                    }
                }
            }
            return users;
        }
        public static List<string> MemberGroups(string groupname)
        {
            List<string> groups = new List<string>();
            if (groupname != null && groupname != "")
            {
                PrincipalContext PC = new PrincipalContext(ContextType.Domain);
                GroupPrincipal GP = GroupPrincipal.FindByIdentity(PC, groupname);
                foreach (Principal p in GP.Members)
                {
                    if (p is GroupPrincipal)
                    {
                        groups.Add(((GroupPrincipal)p).Name);

                    }
                }
            }
            return groups;
        }

        public static void AddUsersToThisGroup(string groupName, List<string> userNames)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName);
            if (gp == null) return;

            // eerst alle gebruikers wissen uit deze groep
            gp.Members.Clear();
            gp.Save();



            // nu alle gebruikers uit de lijst opnieuw toevoegen
            foreach (string userName in userNames)
            {
                UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName);
                if (up == null) continue;
                gp.Members.Add(up);
                gp.Save();
            }


        }
        public static void AddGroupsToThisGroup(string groupName, List<string> groupNames)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groupName);
            if (gp == null) return;

            foreach (string groepnaam in groupNames)
            {
                GroupPrincipal ngp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groepnaam);
                if (ngp == null) continue;
                gp.Members.Add(ngp);
                try
                {
                    gp.Save();
                }
                catch { }
            }

        }
    }
}
