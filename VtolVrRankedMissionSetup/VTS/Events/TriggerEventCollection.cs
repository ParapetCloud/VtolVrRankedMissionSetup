using System;
using System.Collections.Generic;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    public class TriggerEventCollection
    {
        [VTInlineArray]
        public ITriggerEvent[] TriggerEvents => TriggerEventList.ToArray();

        [VTIgnore]
        private List<ITriggerEvent> TriggerEventList { get; }

        public TriggerEventCollection()
        {
            TriggerEventList = [];
        }

        public ConditionalTriggerEvent CreateConditionalTriggerEvent(string name, bool enabled, Conditional conditional, EventTarget[] eventTargets)
        {
            ConditionalTriggerEvent triggerEvent = new()
            {
                Id = TriggerEventList.Count,
                EventName = name,
                Enabled = enabled,
                Conditional = conditional,
                EventInfo = new EventInfo(eventTargets),
            };

            TriggerEventList.Add(triggerEvent);

            return triggerEvent;
        }

        public ProximityTriggerEvent CreateProximityTriggerEvent(string name, bool enabled, Waypoint waypoint, double radius, EventTarget[] eventTargets, bool sphericalRadius = false, TriggerMode triggerMode = TriggerMode.Player, ProxyMode proxyMode = ProxyMode.OnEnter)
        {
            ProximityTriggerEvent triggerEvent = new()
            {
                Id = TriggerEventList.Count,
                EventName = name,
                Enabled = enabled,
                ProxyMode = proxyMode,
                TriggerMode = triggerMode,
                SphericalRadius = sphericalRadius,
                Radius = radius,
                Waypoint = waypoint,
                EventInfo = new EventInfo(eventTargets),
            };

            TriggerEventList.Add(triggerEvent);

            return triggerEvent;
        }
    }
}
