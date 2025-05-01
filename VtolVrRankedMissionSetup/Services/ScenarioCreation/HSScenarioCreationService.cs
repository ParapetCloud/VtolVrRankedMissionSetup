using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using Microsoft.Extensions.DependencyInjection;
using VtolVrRankedMissionSetup.VTS.Events;
using VtolVrRankedMissionSetup.VTS.Objectives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using VtolVrRankedMissionSetup.VTM;
using Windows.UI.StartScreen;
using VtolVrRankedMissionSetup.VT;

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

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(GetLayout(baseA, true), baseA.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(GetLayout(baseB, true), baseB.Prefab.Prefab);

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

        public override void GeneratePreview(Canvas canvas, VTMapCustom map, CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.GeneratePreview(canvas, map, scenario, teamABases, teamBBases);

            BaseInfo baseA = teamABases.Length > 0 ? teamABases[0] : teamBBases[0];
            BaseInfo baseB = teamBBases.Length > 0 ? teamBBases[0] : teamABases[0];

            Vector3 bullseye = (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f;

            Ellipse outer = new()
            {
                Stroke = new SolidColorBrush(Colors.Green),
                Height = 20,
                Width = 20,
                StrokeThickness = 3,
            };

            Vector2 bullseyeMapLocation = worldToPreview(bullseye, map);
            Canvas.SetLeft(outer, bullseyeMapLocation.X - 10);
            Canvas.SetTop(outer, bullseyeMapLocation.Y - 10);

            Ellipse inner = new()
            {
                Fill = new SolidColorBrush(Colors.Green),
                Height = 5,
                Width = 5,
            };

            Canvas.SetLeft(inner, bullseyeMapLocation.X - 2.5);
            Canvas.SetTop(inner, bullseyeMapLocation.Y - 2.5);

            canvas.Children.Add(outer);
            canvas.Children.Add(inner);

            double warnsize = worldToPreview(15 * Units.NauticalMiles, map);
            double killsize = worldToPreview(13 * Units.NauticalMiles, map);
            Vector2 baseAMapLocation = worldToPreview(baseA.Prefab.GlobalPos, map);

            Ellipse warnA = new()
            {
                Stroke = new SolidColorBrush(Colors.Yellow),
                Height = warnsize,
                Width = warnsize,
                StrokeThickness = 1,
            };

            Canvas.SetLeft(warnA, baseAMapLocation.X - (warnsize / 2));
            Canvas.SetTop(warnA, baseAMapLocation.Y - (warnsize / 2));

            canvas.Children.Add(warnA);

            Ellipse killA = new()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                Height = killsize,
                Width = killsize,
                StrokeThickness = 1,
            };

            Canvas.SetLeft(killA, baseAMapLocation.X - (killsize / 2));
            Canvas.SetTop(killA, baseAMapLocation.Y - (killsize / 2));

            canvas.Children.Add(killA);

            Vector2 baseBMapLocation = worldToPreview(baseB.Prefab.GlobalPos, map);

            Ellipse warnB = new()
            {
                Stroke = new SolidColorBrush(Colors.Yellow),
                Height = warnsize,
                Width = warnsize,
                StrokeThickness = 1,
            };

            Canvas.SetLeft(warnB, baseBMapLocation.X - (warnsize / 2));
            Canvas.SetTop(warnB, baseBMapLocation.Y - (warnsize / 2));

            canvas.Children.Add(warnB);

            Ellipse killB = new()
            {
                Stroke = new SolidColorBrush(Colors.Red),
                Height = killsize,
                Width = killsize,
                StrokeThickness = 1,
            };

            Canvas.SetLeft(killB, baseBMapLocation.X - (killsize / 2));
            Canvas.SetTop(killB, baseBMapLocation.Y - (killsize / 2));

            canvas.Children.Add(killB);
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
                Fields = new ConditionalObjectiveFields(),
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
                ObjectiveType = ObjectiveType.Conditional,
                Fields = new ConditionalObjectiveFields(),
            };

            objective.StartEvent = new ObjectiveEvent("Start Event", [new EventTarget(() => VT.Methods.GameSystem.DisplayMessage("Approaching spawncamp protection zone", 1))]);

            return objective;
        }

        protected override string GetAircraftGroup(Team team, AircraftConfig aircraft)
        {
            if (team == Team.Allied)
            {
                return ((AircraftGroup)((int)AircraftGroup.Alpha + (int)aircraft.Spawns[0].Type)).ToString();
            }
            else
            {
                return ((AircraftGroup)((int)AircraftGroup.Zulu - (int)aircraft.Spawns[0].Type)).ToString();
            }
        }
    }
}
