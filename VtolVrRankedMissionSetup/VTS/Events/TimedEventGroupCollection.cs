using System;
using System.Collections.Generic;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    public class TimedEventGroupCollection
    {
        [VTInlineArray]
        public TimedEventGroup[] TimedEventGroups => TimedEventGroupList.ToArray();

        [VTIgnore]
        internal List<TimedEventGroup> TimedEventGroupList { get; }

        public TimedEventGroupCollection()
        {
            TimedEventGroupList = [];
        }

        public TimedEventGroup CreateTimedEventGroup(string name, bool startImmediately, TimeSpan initialDelay, TimeSpan duration, EventTarget[] eventTargets)
        {
            TimedEventGroup eventGroup = new()
            {
                GroupName = name,
                GroupID = TimedEventGroupList.Count,
                BeginImmediately = startImmediately,
                InitialDelay = initialDelay,
                EventInfos = [new TimedEventInfo() { Targets = eventTargets, Time = duration }],
            };

            TimedEventGroupList.Add(eventGroup);

            return eventGroup;
        }
    }
}
