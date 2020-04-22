using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADManagement.LIB.Helpers
{
    public class Helper
    {
        public static string HandleNull(object waarde)
        {
            if (waarde == null)
                return "";
            else
                return waarde.ToString();
        }
    }
}
