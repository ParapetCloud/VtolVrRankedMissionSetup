using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class UnitGroup
    {
        [VTName("Alpha")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Alpha { get; set; }

        [VTName("Bravo")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Bravo { get; set; }

        [VTName("Charlie")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Charlie { get; set; }

        [VTName("Delta")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Delta { get; set; }

        [VTName("Echo")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Echo { get; set; }

        [VTName("Foxtrot")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Foxtrot { get; set; }

        [VTName("Golf")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Golf { get; set; }

        [VTName("Hotel")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Hotel { get; set; }

        [VTName("India")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? India { get; set; }

        [VTName("Juliett")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Juliett { get; set; }

        [VTName("Kilo")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Kilo { get; set; }

        [VTName("Lima")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Lima { get; set; }

        [VTName("Mike")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Mike { get; set; }

        [VTName("November")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? November { get; set; }

        [VTName("Oscar")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Oscar { get; set; }

        [VTName("Papa")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Papa { get; set; }

        [VTName("Quebec")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Quebec { get; set; }

        [VTName("Romeo")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Romeo { get; set; }

        [VTName("Sierra")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Sierra { get; set; }

        [VTName("Tango")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Tango { get; set; }

        [VTName("Uniform")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Uniform { get; set; }

        [VTName("Victor")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Victor { get; set; }

        [VTName("Whiskey")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Whiskey { get; set; }

        [VTName("Xray")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Xray { get; set; }

        [VTName("Yankee")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Yankee { get; set; }

        [VTName("Zulu")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public string? Zulu { get; set; }

        [VTName("Alpha_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? AlphaSettings { get; set; }

        [VTName("Bravo_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? BravoSettings { get; set; }

        [VTName("Charlie_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? CharlieSettings { get; set; }

        [VTName("Delta_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? DeltaSettings { get; set; }

        [VTName("Echo_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? EchoSettings { get; set; }

        [VTName("Foxtrot_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? FoxtrotSettings { get; set; }

        [VTName("Golf_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? GolfSettings { get; set; }

        [VTName("Hotel_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? HotelSettings { get; set; }

        [VTName("India_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? IndiaSettings { get; set; }

        [VTName("Juliett_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? JuliettSettings { get; set; }

        [VTName("Kilo_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? KiloSettings { get; set; }

        [VTName("Lima_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? LimaSettings { get; set; }

        [VTName("Mike_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? MikeSettings { get; set; }

        [VTName("November_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? NovemberSettings { get; set; }

        [VTName("Oscar_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? OscarSettings { get; set; }

        [VTName("Papa_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? PapaSettings { get; set; }

        [VTName("Quebec_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? QuebecSettings { get; set; }

        [VTName("Romeo_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? RomeoSettings { get; set; }

        [VTName("Sierra_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? SierraSettings { get; set; }

        [VTName("Tango_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? TangoSettings { get; set; }

        [VTName("Uniform_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? UniformSettings { get; set; }

        [VTName("Victor_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? VictorSettings { get; set; }

        [VTName("Whiskey_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? WhiskeySettings { get; set; }

        [VTName("Xray_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? XraySettings { get; set; }

        [VTName("Yankee_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? YankeeSettings { get; set; }

        [VTName("Zulu_SETTINGS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? ZuluSettings { get; set; }
    }
}
