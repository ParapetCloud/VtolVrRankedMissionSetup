using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT
{
    public class IdLinkAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public string ValuePrefix { get; set; } = string.Empty;

        public IdLinkAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
