using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    [VTName("TriggerEvent")]
    public class ConditionalTriggerEvent : ITriggerEvent
    {
        [Id]
        public int Id { get; set; }

        public bool Enabled { get; set; }

        public string TriggerType { get; } = "Conditional";

        [IdLink("conditional")]
        public required Conditional Conditional { get; set; }

        public string EventName { get; set; } = "New Trigger Event";

        public EventInfo? EventInfo { get; set; }
    }
}
