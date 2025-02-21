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
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Alpha { get; set; }

        [VTName("Bravo")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Bravo { get; set; }

        [VTName("Charlie")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Charlie { get; set; }

        [VTName("Delta")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Delta { get; set; }

        [VTName("Echo")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Echo { get; set; }

        [VTName("Foxtrot")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Foxtrot { get; set; }

        [VTName("Golf")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Golf { get; set; }

        [VTName("Hotel")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Hotel { get; set; }

        [VTName("India")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? India { get; set; }

        [VTName("Juliett")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Juliett { get; set; }

        [VTName("Kilo")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Kilo { get; set; }

        [VTName("Lima")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Lima { get; set; }

        [VTName("Mike")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Mike { get; set; }

        [VTName("November")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? November { get; set; }

        [VTName("Oscar")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Oscar { get; set; }

        [VTName("Papa")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Papa { get; set; }

        [VTName("Quebec")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Quebec { get; set; }

        [VTName("Romeo")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Romeo { get; set; }

        [VTName("Sierra")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Sierra { get; set; }

        [VTName("Tango")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Tango { get; set; }

        [VTName("Uniform")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Uniform { get; set; }

        [VTName("Victor")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Victor { get; set; }

        [VTName("Whiskey")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Whiskey { get; set; }

        [VTName("Xray")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Xray { get; set; }

        [VTName("Yankee")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Yankee { get; set; }

        [VTName("Zulu")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public string? Zulu { get; set; }

        [VTName("Alpha_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? AlphaSettings { get; set; }

        [VTName("Bravo_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? BravoSettings { get; set; }

        [VTName("Charlie_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? CharlieSettings { get; set; }

        [VTName("Delta_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? DeltaSettings { get; set; }

        [VTName("Echo_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? EchoSettings { get; set; }

        [VTName("Foxtrot_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? FoxtrotSettings { get; set; }

        [VTName("Golf_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? GolfSettings { get; set; }

        [VTName("Hotel_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? HotelSettings { get; set; }

        [VTName("India_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? IndiaSettings { get; set; }

        [VTName("Juliett_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? JuliettSettings { get; set; }

        [VTName("Kilo_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? KiloSettings { get; set; }

        [VTName("Lima_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? LimaSettings { get; set; }

        [VTName("Mike_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? MikeSettings { get; set; }

        [VTName("November_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? NovemberSettings { get; set; }

        [VTName("Oscar_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? OscarSettings { get; set; }

        [VTName("Papa_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? PapaSettings { get; set; }

        [VTName("Quebec_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? QuebecSettings { get; set; }

        [VTName("Romeo_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? RomeoSettings { get; set; }

        [VTName("Sierra_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? SierraSettings { get; set; }

        [VTName("Tango_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? TangoSettings { get; set; }

        [VTName("Uniform_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? UniformSettings { get; set; }

        [VTName("Victor_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? VictorSettings { get; set; }

        [VTName("Whiskey_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? WhiskeySettings { get; set; }

        [VTName("Xray_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? XraySettings { get; set; }

        [VTName("Yankee_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? YankeeSettings { get; set; }

        [VTName("Zulu_SETTINGS")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public UnitGroupSettings? ZuluSettings { get; set; }
    }
}
