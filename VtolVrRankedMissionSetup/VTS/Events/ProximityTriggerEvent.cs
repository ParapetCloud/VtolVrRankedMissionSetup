using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    public enum TriggerMode
    {
        Player,
        Unit,
        AnyUnit,
        AnyAllied,
        AnyEnemy,
    }

    public enum ProxyMode
    {
        OnEnter,
        OnExit,
    }

    [VTName("TriggerEvent")]
    public class ProximityTriggerEvent : ITriggerEvent
    {
        [Id]
        public int Id { get; set; }

        public bool Enabled { get; set; }

        public string TriggerType { get; } = "Proximity";

        [IdLink("waypoint")]
        public required Waypoint Waypoint { get; set; }

        public double Radius { get; set; }

        public bool SphericalRadius { get; set; }

        public TriggerMode TriggerMode { get; set; }

        public ProxyMode ProxyMode { get; set; }

        public string EventName { get; set; } = "New Trigger Event";

        public EventInfo? EventInfo { get; set; }
    }
}
