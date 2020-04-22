using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADManagement.LIB.Entities;
using ADManagement.LIB.Services;

namespace ADManagement.LIB.Entities
{
    public class OrgUnit
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public List<OrgUnit> Childeren { get; set; }
        
        public OrgUnit()
        {
            Childeren = new List<OrgUnit>();

        }
    }
}
