using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs;
using System.Text.Json;
using System.IO;

namespace VtolVrRankedMissionSetup.Step
{
    public class PlaceEverythingStep
    {
        private AirbaseConfig Airbase1;
        private AirbaseConfig Airbase2;
        private static Dictionary<string, string> Equipment = new()
        {
            { "F/A-26B", "fa26_iris-t-x3;af_mk82;fa26_mk82x2;fa26_mk82x3;fa26_mk82HDx1;fa26_mk82HDx2;fa26_mk82HDx3;fa26_mk83x1;fa26_cbu97x1;h70-x7ld;h70-x7ld-under;h70-x14ld;h70-x14ld-under;fa26_cagm-6;fa26_gbu38x1;fa26_gbu38x2;fa26_gbu38x3;fa26_gbu39x4uFront;fa26_gbu39x4uRear;fa26_maverickx1;fa26_maverickx3;fa26_agm89x1;fa26_gbu12x1;fa26_gbu12x2;fa26_gbu12x3;fa26_agm161;"},
            { "F-45A", "f45_mk82x1;f45_mk82Internal;f45_mk82x4Internal;f45_gbu12x2Internal;f45_gbu12x1;f45-gbu39;f45_agm161;f45_agm161Internal;f45_gbu38x1;f45_gbu38x2Internal;f45_gbu38x4Internal;f45_mk83x1;f45_mk83x1Internal;f45-agm145I;f45-agm145ISide;f45-agm145x3;f45-gbu53;"},
            { "EF-24G", "ef24_marmx1;ef24_gbu12x1;ef24_gbu12x2;ef24_gbu12x3;ef24_gbu38x1;ef24_gbu38x2;ef24_gbu38x3;ef24_gbu39x4;ef24_mk82x1;ef24_mk82x2;ef24_mk82x3;ef24_mk82x4;ef24_mk82HDx1;ef24_mk82HDx2;ef24_mk82HDx3;ef24_mk82HDx4;ef24_mk83x1;ef24_mk83x2;ef24_agm161;ef24_agm89x1;"},
            { "T-55", "t55_mk82;t55_mk82x2;t55_mk82x3;t55_mk82HDx1;t55_mk82HDx2;t55_mk82HDx3;t55_mk83x1;t55_cbu97x1;t55_gbu12x1;t55_gbu12x2;t55_gbu12x3;t55_gbu38x1;t55_gbu38x2;t55_gbu38x3;t55_gbu39x4u;t55_maverickx1;t55_maverickx3;t55_cagm-6;t55_h70-x7ld;t55_h70-x14ld;t55_apkws-x7;t55_agm161;t55_agm89x1;"},
        };

        public PlaceEverythingStep()
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            jsonOptions.Converters.Add(new Vector3JsonConverter());

            Airbase1 = JsonSerializer.Deserialize<AirbaseConfig>(File.ReadAllText(Path.Join(Windows.ApplicationModel.Package.Current.InstalledPath, "Configs/airbase1.json")), jsonOptions)!;
            Airbase2 = JsonSerializer.Deserialize<AirbaseConfig>(File.ReadAllText(Path.Join(Windows.ApplicationModel.Package.Current.InstalledPath, "Configs/airbase2.json")), jsonOptions)!;
        }

        public void Start(CustomScenario scenario, BaseInfo baseA, BaseInfo baseB)
        {
            // DONT FORGET ABOUT THE RTBs
            AirbaseConfig airbaseAConfig = baseA.Prefab.Prefab == "airbase1" ? Airbase1 : Airbase2;
            AirbaseConfig airbaseBConfig = baseB.Prefab.Prefab == "airbase1" ? Airbase1 : Airbase2;

            List<IUnitSpawner> spawners = [];
            List<Waypoint> waypoints = [];
            List<Objective> objectives = [];
            List<Objective> objectivesb = [];
            int objectiveCount = 0;

            scenario.Waypoints = new WaypointCollection();

            Waypoint bullseye = new Waypoint()
            {
                Id = waypoints.Count,
                GlobalPoint = (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f,
                Name = "Bullseye",
            };
            waypoints.Add(bullseye);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            AddAircraftToBase(baseA, airbaseAConfig.F26, "F/A-26B", spawners, "Allied:Alpha");
            AddAircraftToBase(baseA, airbaseAConfig.F45, "F-45A", spawners, "Allied:Bravo");
            AddAircraftToBase(baseA, airbaseAConfig.F24, "EF-24G", spawners, "Allied:Charlie", 2);
            AddAircraftToBase(baseA, airbaseAConfig.T55, "T-55", spawners, "Allied:Delta", 2);

            Waypoint teamABase = new()
            {
                Id = waypoints.Count,
                GlobalPoint = MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA),
                Name = "TeamARTB"
            };
            waypoints.Add(teamABase);
            scenario.RtbWpt = teamABase;

            waypoints.Add(new Waypoint()
            {
                Id = waypoints.Count,
                GlobalPoint = MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Protection, baseA),
                Name = "spawncamp_A"
            });

            foreach (Vector3 wp in airbaseAConfig.Waypoints.Perimeter)
            {
                waypoints.Add(new Waypoint()
                {
                    Id = waypoints.Count,
                    GlobalPoint = MathHelpers.BaseToWorld(wp, baseA),
                    Name = "airbase_A"
                });
            }

            AddAircraftToBase(baseB, airbaseBConfig.F26, "F/A-26B", spawners, "Enemy:Zulu");
            AddAircraftToBase(baseB, airbaseBConfig.F45, "F-45A", spawners, "Enemy:Yankee");
            AddAircraftToBase(baseB, airbaseBConfig.F24, "EF-24G", spawners, "Enemy:Xray", 2);
            AddAircraftToBase(baseB, airbaseBConfig.T55, "T-55", spawners, "Enemy:Whiskey", 2);

            Waypoint teamBBase = new()
            {
                Id = waypoints.Count,
                GlobalPoint = MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB),
                Name = "TeamBRTB"
            };
            waypoints.Add(teamBBase);
            scenario.RtbWptB = teamBBase;

            waypoints.Add(new Waypoint()
            {
                Id = waypoints.Count,
                GlobalPoint = MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Protection, baseB),
                Name = "spawncamp_B"
            });

            foreach (Vector3 wp in airbaseBConfig.Waypoints.Perimeter)
            {
                waypoints.Add(new Waypoint()
                {
                    Id = waypoints.Count,
                    GlobalPoint = MathHelpers.BaseToWorld(wp, baseB),
                    Name = "airbase_B"
                });
            }

            objectives.Add(CreateObjectiveForKill(objectiveCount++, objectives.Count, teamBBase));
            objectives.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectives.Count));

            objectivesb.Add(CreateObjectiveForKill(objectiveCount++, objectivesb.Count, teamABase));
            objectivesb.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectivesb.Count));

            scenario.Objectives = objectives.ToArray();
            scenario.ObjectivesOpfor = objectivesb.ToArray();
            scenario.Units = spawners.ToArray();
            scenario.Waypoints.Waypoints = waypoints.ToArray();
        }

        static void AddAircraftToBase(BaseInfo baseInfo, AircraftConfig[]? aircrafts, string vehicle, List<IUnitSpawner> spawners, string unitGroup, int slots = 0)
        {
            if (aircrafts == null)
                return;

            string[] unitParts = unitGroup.Split(':');

            for (int i = 0; i < aircrafts.Length; ++i)
            {
                AircraftConfig aircraft = aircrafts[i];

                Vector3 location = MathHelpers.BaseToWorld(aircraft.Location, baseInfo);
                Vector3 rotation = baseInfo.Prefab.Rotation + aircraft.Rotation;
                MathHelpers.ClampRotation(ref rotation);

                MultiplayerSpawn spawn = new(Enum.Parse<Team>(unitParts[0]), $"{unitParts[1]} 1-{i + 1}")
                {
                    UnitInstanceID = spawners.Count,
                    GlobalPosition = location,
                    Rotation = rotation,
                };

                spawn.MultiplayerSpawnFields.UnitGroup = unitGroup;
                spawn.MultiplayerSpawnFields.Vehicle = vehicle;
                spawn.MultiplayerSpawnFields.Equipment = Equipment[vehicle];
                spawn.MultiplayerSpawnFields.Slots = slots;

                spawners.Add(spawn);
            }
        }

        static Objective CreateObjectiveForKill(int objectiveId, int orderId, Waypoint waypoint)
        {
            return new Objective()
            {
                ObjectiveName = "Kill",
                ObjectiveInfo = "Join the discord: https://discord.gg/yQ8ZW8cQRt",
                ObjectiveID = objectiveId,
                OrderID = orderId,
                Required = true,
                Waypoint = waypoint,
                AutoSetWaypoint = true,
                StartMode = ObjectiveStartMode.Immediate,
                ObjectiveType = ObjectiveType.Conditional,
            };
        }

        static Objective CreateObjectiveForSpawnProt(int objectiveId, int orderId)
        {
            Objective objective = new()
            {
                ObjectiveName = "SpawncampProt",
                ObjectiveInfo = string.Empty,
                ObjectiveID = objectiveId,
                OrderID = orderId,
                Required = false,
                AutoSetWaypoint = false,
                StartMode = ObjectiveStartMode.Triggered,
                ObjectiveType = ObjectiveType.Conditional
            };

            objective.StartEvent.EventInfo.EventTarget = new ObjectiveEventTarget()
            {
                TargetType = "System",
                TargetID = 1,
                EventName = "Display Message",
                MethodName = "DisplayMessage",
                ParamInfos = 
                [
                    new ObjectiveEventTarget.ParamInfo()
                    {
                        Type = "System.String",
                        Value = "Approaching spawncamp protection zone",
                        Name = "Text",
                        ParamAttrInfos =
                        [
                            new ObjectiveEventTarget.ParamAttrInfo()
                            {
                                Type = "TextInputModes",
                                Data = "MultiLine",
                            },
                            new ObjectiveEventTarget.ParamAttrInfo()
                            {
                                Type = "System.Int32",
                                Data = "140",
                            }
                        ],
                    },
                    new ObjectiveEventTarget.ParamInfo()
                    {
                        Type = "System.Single",
                        Value = "1",
                        Name = "Duration",
                        ParamAttrInfos =
                        [
                            new ObjectiveEventTarget.ParamAttrInfo()
                            {
                                Type = "MinMax",
                                Data = "(1,9999)",
                            },
                        ],
                    }
                ],
            };

            return objective;
        }
    }
}
