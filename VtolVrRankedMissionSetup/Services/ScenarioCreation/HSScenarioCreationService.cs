using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using Microsoft.Extensions.DependencyInjection;
using VtolVrRankedMissionSetup.VTS.Events;

namespace VtolVrRankedMissionSetup.Services.ScenarioCreation
{
    [Service(ServiceLifetime.Singleton)]
    public class HSScenarioCreationService : ScenarioCreationService
    {
        public HSScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService) : base(scenarioMode, layoutService) { }

        public override void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.SetUpScenario(scenario, teamABases, teamBBases);

            scenario.CampaignID = "Headless Server";
            scenario.ScenarioDescription = "24/7 BVR. Each team gets 8x F/A-26b, 2x EF-24, 3x T-55, 1x F-45";

            BaseInfo baseA = teamABases[0];
            BaseInfo baseB = teamBBases[0];

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(baseA.Layout ?? "HS", baseA.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(baseB.Layout ?? "HS", baseB.Prefab.Prefab);

            int objectiveCount = 0;

            scenario.Waypoints = new WaypointCollection();

            Waypoint bullseye = scenario.Waypoints.CreateWaypoint("Bullseye", (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            Waypoint teamABase = scenario.Waypoints.CreateWaypoint("TeamARTB", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA));
            scenario.AlliedRTB = teamABase;

            scenario.Waypoints.CreateWaypoint("spawncamp_A", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Protection, baseA));

            foreach (Vector3 wp in airbaseAConfig.Waypoints.Perimeter)
            {
                scenario.Waypoints.CreateWaypoint("airbase_A", MathHelpers.BaseToWorld(wp, baseA));
            }

            Waypoint teamBBase = scenario.Waypoints.CreateWaypoint("TeamBRTB", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB));
            scenario.EnemyRTB = teamBBase;

            scenario.Waypoints.CreateWaypoint("spawncamp_B", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Protection, baseB));

            foreach (Vector3 wp in airbaseBConfig.Waypoints.Perimeter)
            {
                scenario.Waypoints.CreateWaypoint("airbase_B", MathHelpers.BaseToWorld(wp, baseB));
            }

            List<Objective> objectives = [];
            objectives.Add(CreateObjectiveForKill(objectiveCount++, objectives.Count, teamBBase));
            objectives.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectives.Count));
            scenario.AlliedObjectives = objectives.ToArray();

            List<Objective> objectivesb = [];
            objectivesb.Add(CreateObjectiveForKill(objectiveCount++, objectivesb.Count, teamABase));
            objectivesb.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectivesb.Count));
            scenario.EnemyObjectives = objectivesb.ToArray();
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

            objective.StartEvent = new ObjectiveEvent("Start Event", [new EventTarget("Display spawncamp warning", () => VT.Methods.System.DisplayMessage("Approaching spawncamp protection zone", 1))]);

            return objective;
        }
    }
}
