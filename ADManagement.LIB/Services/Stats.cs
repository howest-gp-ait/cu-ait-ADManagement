using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using ADManagement.LIB.Entities;
namespace ADManagement.LIB.Services
{
    public class Stats
    {
        public static List<DirectoryEntry> GetAllOUs()
        {

            List<DirectoryEntry> deOus = new List<DirectoryEntry>();
            DirectorySearcher zoeker = new DirectorySearcher(ActiveDirectory.LDAPLong)
            {
                Filter = "(objectCategory=organizationalUnit)",
                SearchScope = SearchScope.Subtree
            };
            foreach (SearchResult resultaat in zoeker.FindAll())
            {
                deOus.Add(resultaat.GetDirectoryEntry());
            }
            return deOus;
        }
        public static List<DirectoryEntry> GetAllGroups()
        {
            List<DirectoryEntry> deGroups = new List<DirectoryEntry>();
            DirectorySearcher zoeker = new DirectorySearcher(ActiveDirectory.LDAPLong)
            {
                Filter = "(objectCategory=Group)",
                SearchScope = SearchScope.Subtree
            };
            foreach (SearchResult resultaat in zoeker.FindAll())
            {
                deGroups.Add(resultaat.GetDirectoryEntry());
            }
            return deGroups;
        }
        public static List<DirectoryEntry> GetAllUsers()
        {
            List<DirectoryEntry> deUsers = new List<DirectoryEntry>();
            DirectorySearcher zoeker = new DirectorySearcher(ActiveDirectory.LDAPLong)
            {
                Filter = "(objectCategory=User)",
                SearchScope = SearchScope.Subtree
            };
            foreach (SearchResult resultaat in zoeker.FindAll())
            {
                deUsers.Add(resultaat.GetDirectoryEntry());
            }
            return deUsers;
        }
        public static List<GroupPrincipal> UserBelongsToGroups(string user_samaccountname)
        {
            List<GroupPrincipal> Groepen = new List<GroupPrincipal>();
            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, user_samaccountname);
            foreach (GroupPrincipal groep in up.GetGroups())
            {
                Groepen.Add(groep);
            }
            return Groepen;
        }
        public static List<GroupPrincipal> UserDoesNotBelongToGroups(string user_samaccountname)
        {
            List<GroupPrincipal> BelongsTo = UserBelongsToGroups(user_samaccountname);
            List<GroupPrincipal> DoesNotBelongTo = new List<GroupPrincipal>();

            PrincipalContext pc = new PrincipalContext(ContextType.Domain);
            GroupPrincipal gp = new GroupPrincipal(pc);
            PrincipalSearcher srch = new PrincipalSearcher(gp);
            foreach (GroupPrincipal sgp in srch.FindAll())
            {
                if (sgp != null)
                {
                    bool found = false;
                    foreach(GroupPrincipal chkPrincipal in BelongsTo)
                    {
                        if(chkPrincipal == sgp)
                        {
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                    {
                        DoesNotBelongTo.Add(sgp);
                    }
                }
            }
            return DoesNotBelongTo;
        }
    }
}
