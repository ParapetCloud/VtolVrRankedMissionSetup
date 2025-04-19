using System.ComponentModel;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;

namespace VtolVrRankedMissionSetup.VTS
{
    public class BaseInfo : INotifyPropertyChanged
    {
        [IdLink("id")]
        public StaticPrefab Prefab { get; set; }

        public string OverrideBaseName { get; set; } = string.Empty;
        public Team BaseTeam { get; set; } = Team.Allied;
        [VTName("CUSTOM_DATA")]
        public Empty? CustomData { get; set; }

        [VTIgnore]
        private string _layout;
        [VTIgnore]
        public string Layout
        {
            get => _layout;
            set
            {
                if (_layout == value)
                    return;

                _layout = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Layout)));
            }
        }

        public BaseInfo(StaticPrefab prefab)
        {
            Prefab = prefab;
            Layout = "";
        }

        [VTIgnore]
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
