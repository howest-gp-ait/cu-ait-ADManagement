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
    public class UserGroupMembership
    {
        public static List<Group> GetUserGroupMembership(string sAMAccountName)
        {
            List<Group> MemberOff = new List<Group>();
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
            foreach (var item in up.GetGroups())
            {
                Group group = new Group();
                group.Name = item.Name;
                group.UserMembers = new UsersInGroup(item.SamAccountName).Users;
                group.GroupMembers = new GroupsInGroups(item.SamAccountName).Groups;
                MemberOff.Add(group);
            }
            return MemberOff;
        }
        public static void AddNewUserToGroups(string sAMAccountName, List<string> groepnamen)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
            if (up == null) return;
            foreach (string groepnaam in groepnamen)
            {
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groepnaam);
                if(gp != null)
                {
                    gp.Members.Add(up);
                    try
                    {
                        gp.Save();
                    }
                    catch { }
                }
            }
        }
        public static void UpdateExistingUserGroupMembership(string sAMAccountName, List<string> groepnamen)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
            // eerst gebruiker uit al zijn groepen verwijderen
            foreach (GroupPrincipal gp in up.GetGroups())
            {
                if (gp.Name == "Domain Users")
                    continue;
                gp.Members.Remove(up);
                try
                {
                    gp.Save();
                }
                catch { }
            }
            // gebruiker terug toevoegen aan de groepen waartoe hij hoort
            foreach (string groepnaam in groepnamen)
            {
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groepnaam);
                if (gp != null)
                {
                    gp.Members.Add(up);
                    try
                    {
                        gp.Save();
                    }
                    catch { }
                }
            }

        }
        public static void RemoveUserFromGroup(string sAMAccountName, string groepnaam)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groepnaam);
            if (gp != null)
            {
                gp.Members.Remove(up);
                try
                {
                    gp.Save();
                }
                catch { }
            }

        }
        public static void AddUserToGroup(string sAMAccountName, string groepnaam)
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, sAMAccountName);
            GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, groepnaam);
            if (gp != null)
            {
                gp.Members.Add(up);
                try
                {
                    gp.Save();
                }
                catch { }
            }

        }

    }
}
