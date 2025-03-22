using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT
{
    public class EventTargetAttribute : Attribute
    {
        public string EventName { get; set; }
        public string TargetTypeName { get; set; }

        public int TargetId { get; set; }
        public int AltTargetIdx { get; set; } = -1;

        public EventTargetAttribute(string eventName, string typeName)
        {
            EventName = eventName;
            TargetTypeName = typeName;
        }
    }
}
