using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADManagement.LIB.Entities;
using System.DirectoryServices;

namespace ADManagement.LIB.Services
{
    public class OrgUnits
    {
        public List<OrgUnit> OUs { get; set; }
        public OrgUnits()
        {
            OUs = new List<OrgUnit>();
            DirectorySearcher zoeker = new DirectorySearcher(ActiveDirectory.LDAPLong)
            {
                Filter = "(objectCategory=organizationalUnit)",
                SearchScope = SearchScope.OneLevel
            };
            foreach (SearchResult resultaat in zoeker.FindAll())
            {
                OrgUnit ou = new OrgUnit();
                ou.Name = resultaat.Properties["name"][0].ToString();
                ou.Tag = resultaat.Path.Replace(ActiveDirectory.LDAPLong , "");
                ou.Childeren = getChilderen(resultaat.Path);
                OUs.Add(ou);
            }
        }
        //public OrgUnit FindOrgUnitByName(string zoekou)
        //{
        //    ActiveDirectory ad = new ActiveDirectory();
        //    DirectorySearcher zoeker = new DirectorySearcher(ad.Domain)
        //    {
        //        Filter = "(objectCategory=organizationalUnit)",
        //        SearchScope = SearchScope.OneLevel
        //    };
        //    foreach (SearchResult resultaat in zoeker.FindAll())
        //    {
        //        OrgUnit ou = new OrgUnit();
        //        ou.Name = resultaat.Properties["name"][0].ToString();
        //        ou.Tag = resultaat.Path;
        //        ou.Childeren = getChilderen(ou.Tag);
        //        OUs.Add(ou);
        //    }
        //}
        private List<OrgUnit> getChilderen(string path)
        {
            List<OrgUnit> subOUs = new List<OrgUnit>();
            DirectorySearcher zoeker = new DirectorySearcher(new DirectoryEntry(path))
            {
                Filter = "(objectCategory=organizationalUnit)",
                SearchScope = SearchScope.OneLevel
            };
            foreach (SearchResult resultaat in zoeker.FindAll())
            {
                OrgUnit ou = new OrgUnit();
                ou.Name = resultaat.Properties["name"][0].ToString();
                ou.Tag = resultaat.Path.Replace(ActiveDirectory.LDAPLong , "");
                ou.Childeren = getChilderen(resultaat.Path);
                subOUs.Add(ou);
            }
            return subOUs;
        }




    }
}
