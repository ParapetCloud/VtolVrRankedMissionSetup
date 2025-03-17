using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT.Methods
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class ParamInfoAttribute: Attribute
    {
        public bool UseFullName { get; }
        public string? CustomTypeName { get; set; }
        public string? CustomParameterName { get; set; }

        public ParamInfoAttribute()
        {
        }
    }
}
