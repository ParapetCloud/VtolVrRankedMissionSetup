using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT.Methods
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class ParamAttrInfoAttribute: Attribute
    {
        public string Type { get; }
        public string Data { get; }

        public ParamAttrInfoAttribute(string type, string data)
        {
            Type = type;
            Data = data;
        }
    }
}
