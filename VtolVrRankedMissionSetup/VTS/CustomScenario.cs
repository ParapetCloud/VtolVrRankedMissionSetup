﻿using System;
using System.Collections.Generic;
using System.Reflection;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;
using VtolVrRankedMissionSetup.VTS.Events;
using VtolVrRankedMissionSetup.VTS.Objectives;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.VTS
{
    public class CustomScenario
    {
        public string GameVersion { get; set; } = "1.12.4f3";
        public string CampaignID { get; set; } = string.Empty;
        public uint CampaignOrderIdx { get; set; } = 0;
        public string ScenarioName { get; set; } = string.Empty;
        public string ScenarioID { get; set; } = string.Empty;
        public string ScenarioDescription { get; set; } = string.Empty;
        public string MapID { get; set; } = string.Empty;
        public string Vehicle { get; set; } = "F-45A";
        public bool Multiplayer { get; set; } = true;
        public string AllowedEquips { get; set; } = "f45_gun;f45_sidewinderx2;f45_aim9x1;f45_amraamInternal;f45_amraamRail;f45_mk82x1;f45_mk82Internal;f45_mk82x4Internal;f45_gbu12x2Internal;f45_gbu12x1;f45-gbu39;f45_agm161;f45_agm161Internal;f45_droptank;f45_gbu38x1;f45_gbu38x2Internal;f45_gbu38x4Internal;f45_mk83x1;f45_mk83x1Internal;f45-agm145I;f45-agm145ISide;f45-agm145x3;f45-gbu53;";
        public bool ForceEquips { get; set; } = false;
        public double NormForcedFuel { get; set; } = 1;
        public bool EquipsConfigurable { get; set; } = true;
        public double BaseBudget { get; set; } = 100000;
        public bool IsTraining { get; set; } = false;
        [IdLink("rtbWptID", ValuePrefix = "wpt:")]
        public Waypoint? AlliedRTB { get; set; }
        public string RefuelWptID { get; set; } = string.Empty;
        public uint MpPlayerCount { get; set; } = 16;
        public bool AutoPlayerCount { get; set; } = false;
        public uint OverrideAlliedPlayerCount { get; set; } = 8;
        public uint OverrideEnemyPlayerCount { get; set; } = 8;
        public double ScorePerDeath_A { get; set; } = 0;
        public double ScorePerDeath_B { get; set; } = 0;
        public double ScorePerKill_A { get; set; } = 0;
        public double ScorePerKill_B { get; set; } = 0;
        public string MpBudgetMode { get; set; } = "Life";
        [IdLink("rtbWptID_B", ValuePrefix = "wpt:")]
        public Waypoint? EnemyRTB { get; set; }
        public string RefuelWptID_B { get; set; } = string.Empty;
        public bool SeparateBriefings { get; set; } = false;
        public double BaseBudgetB { get; set; } = 100000;
        public bool InfiniteAmmo { get; set; } = false;
        public double InfAmmoReloadDelay { get; set; } = 5;
        public double FuelDrainMult { get; set; } = 1;
        public string EnvName { get; set; } = "day";
        public bool SelectableEnv { get; set; } = true;
        public double WindDir { get; set; } = 0;
        public double WindSpeed { get; set; } = 0;
        public double WindVariation { get; set; } = 0;
        public double WindGusts { get; set; } = 0;
        public int DefaultWeather { get; set; } = 0;
        public int CustomTimeOfDay { get; set; } = 11;
        public bool OverrideLocation { get; set; } = true;
        public double OverrideLatitude { get; set; } = 0;
        public double OverrideLongitude { get; set; } = 0;
        public int Month { get; set; } = 1;
        public int Day { get; set; } = 1;
        public int Year { get; set; } = 2024;
        public double TimeOfDaySpeed { get; set; } = 1;
        public string QsMode { get; set; } = "Anywhere";
        public int QsLimit { get; set; } = -1;

        [VTName("WEATHER_PRESETS")]
        public object? WeatherPresets { get; set; }

        [VTName("UNITS")]
        public IUnitSpawner[]? Units { get; set; }

        [VTName("PATHS")]
        public object? Paths { get; set; }

        [VTName("WAYPOINTS")]
        public WaypointCollection? Waypoints { get; set; }

        [VTName("UNITGROUPS")]
        public UnitGroups? UnitGroups
        {
            get
            {
                UnitGroups groups = new();

                if (Units != null)
                {
                    foreach (IUnitSpawner spawner in Units)
                    {
                        if (spawner is not MultiplayerSpawn mpSpawn)
                            continue;

                        string[] ids = mpSpawn.MultiplayerSpawnFields.UnitGroup.Split(':');

                        UnitGroup? group = ids[0] == "Allied" ?
                            (groups.Allied ??= new()) :
                            (groups.Enemy ??= new());

                        Type groupType = typeof(UnitGroup);

                        PropertyInfo groupList = groupType.GetProperty(ids[1])!;

                        string list = (string?)groupList.GetValue(group) ?? "2;";

                        list += $"{mpSpawn.UnitInstanceID};";

                        groupList.SetValue(group, list);

                        PropertyInfo groupSettings = groupType.GetProperty($"{ids[1]}Settings")!;
                        if (groupSettings.GetValue(group) == null)
                        {
                            groupSettings.SetValue(group, new UnitGroupSettings());
                        }
                    }
                }

                return groups;
            }
        }

        public TimedEventGroupCollection? TimedEventGroups { get; set; }

        [VTName("TRIGGER_EVENTS")]
        public TriggerEventCollection? TriggerEvents { get; set; }

        [VTName("OBJECTIVES")]
        public Objective[]? AlliedObjectives { get; set; }

        [VTName("OBJECTIVES_OPFOR")]
        public Objective[]? EnemyObjectives { get; set; }

        public object? StaticObjects { get; set; }

        public ConditionalCollection? Conditionals { get; set; }

        public ConditionalActionsCollection? ConditionalActions { get; set; }

        public object? RandomEvents { get; set; }

        public SequenceCollection? EventSequences { get; set; }

        [VTName("BASES")]
        public BaseInfo[] Bases { get; set; }

        public GlobalValueCollection? GlobalValues { get; set; }
        public BriefingNote[]? Briefing { get; set; }
        public BriefingNote[]? Briefing_B { get; set; }

        public CustomScenario(VTMapCustom map)
        {
            MapID = map.MapID;

            List<BaseInfo> infos = [];
            foreach (StaticPrefab prefab in map.StaticPrefabs)
            {
                if (!prefab.Prefab.StartsWith("airbase"))
                    continue;

                infos.Add(new BaseInfo(prefab));
            }
            Bases = infos.ToArray();
        }
    }
}
