using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace VtolVrRankedMissionSetup.Services.ScenarioCreation
{
    [Service(ServiceLifetime.Singleton)]
    public class HSScenarioCreationService : ScenarioCreationService
    {
        public HSScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService) : base(scenarioMode, layoutService) { }

        public override void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.SetUpScenario(scenario, teamABases, teamBBases);

            BaseInfo baseA = teamABases[0];
            BaseInfo baseB = teamBBases[0];

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(baseA.Layout ?? "HS", baseA.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(baseB.Layout ?? "HS", baseB.Prefab.Prefab);

            List<Waypoint> waypoints = [];
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
            scenario.Waypoints.Waypoints = waypoints.ToArray();

            List<Objective> objectives = [];
            objectives.Add(CreateObjectiveForKill(objectiveCount++, objectives.Count, teamBBase));
            objectives.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectives.Count));
            scenario.Objectives = objectives.ToArray();

            List<Objective> objectivesb = [];
            objectivesb.Add(CreateObjectiveForKill(objectiveCount++, objectivesb.Count, teamABase));
            objectivesb.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectivesb.Count));
            scenario.ObjectivesOpfor = objectivesb.ToArray();
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
