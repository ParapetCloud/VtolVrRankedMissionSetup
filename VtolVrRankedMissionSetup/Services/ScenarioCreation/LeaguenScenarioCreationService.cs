using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;
using VtolVrRankedMissionSetup.VTM;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.VTS.Events;
using VtolVrRankedMissionSetup.VTS.Objectives;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.Services.ScenarioCreation
{
    [Service(ServiceLifetime.Singleton)]
    public class LeaguenScenarioCreationService : ScenarioCreationService
    {
        private const int MaxWarnDistance = 15;

        private static string[] AudioFiles = [
            "bhit_helmet-1",
            "Bonk_Sound_Effect",
            "critical-hit-sounds-effect",
            "halosplash",
            "overwatch-kill-sound",
            "Trident_return3",
            "war-thunder-kill",
        ];

        public LeaguenScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService) : base(scenarioMode, layoutService) { }

        public override void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            if (teamABases.Length < 2 || teamBBases.Length < 2)
                throw new ScenarioCreationException("This scenario mode requires 2 bases for each team");

            //////////////////////////////////////////////////////////////////////
            // Replaced base functionality instead of a call to base.SetUpScenario
            //////////////////////////////////////////////////////////////////////
            alliedGroupCounts.Clear();
            enemyGroupCounts.Clear();
            List<IUnitSpawner> spawners = [];

            AddAircraftToBases(teamABases[0], teamABases[1], spawners, Team.Allied);
            AddAircraftToBases(teamBBases[0], teamBBases[1], spawners, Team.Enemy);

            scenario.Units = spawners.ToArray();

            if (scenarioMode.ActiveMode.WeatherPresets != null)
            {
                List<WeatherPreset> presets = [];

                for (int i = 0; i < scenarioMode.ActiveMode.WeatherPresets.Length; ++i)
                {
                    presets.Add(new WeatherPreset() { Id = i + 8, Data = scenarioMode.ActiveMode.WeatherPresets[i] });
                }

                scenario.WeatherPresets = presets.ToArray();
            }

            //////////////////////////////////////////////////////////////////////
            // End of replaced functionality
            //////////////////////////////////////////////////////////////////////

            scenario.CampaignID = "Ranked Playlist";
            scenario.ScenarioDescription = "Join the discord to link your account and see your stats | https://discord.gg/UVYvpJ4jkf";

            scenario.Briefing = [
                    new BriefingNote() {
                        Text = "Welcome to the Ranked Server. Missile intercepts are off, but aircraft collisions are on. Entering the enemy spawn protection zone initiates a 15-second kill timer. To understand how the ranking system works and view the leaderboard, join the discord: https://discord.gg/UVYvpJ4jkf",
                        ImagePath = "Server Profile RankedBadge.png",
                    },
                ];

            BaseInfo baseA1 = teamABases[0];
            BaseInfo baseA2 = teamABases[1];
            BaseInfo baseB1 = teamBBases[0];
            BaseInfo baseB2 = teamBBases[1];

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(GetLayout(baseA1, 0), baseA1.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(GetLayout(baseB1, 0), baseB1.Prefab.Prefab);

            int objectiveCount = 0;

            scenario.Waypoints = new WaypointCollection();

            Vector3 averageBaseLocation = (baseA1.Prefab.GlobalPos + baseA2.Prefab.GlobalPos + baseB1.Prefab.GlobalPos + baseB2.Prefab.GlobalPos) / 4.0f;

            Waypoint bullseye = scenario.Waypoints.CreateWaypoint("Bullseye", averageBaseLocation);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            Waypoint teamABase = scenario.Waypoints.CreateWaypoint("TeamARTB", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA1));
            scenario.AlliedRTB = teamABase;

            scenario.Waypoints.CreateWaypoint("spawncamp_A", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Protection, baseA1));
            scenario.Waypoints.CreateWaypoint("spawncamp_A", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Protection, baseA2));

            Waypoint teamBBase = scenario.Waypoints.CreateWaypoint("TeamBRTB", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB1));
            scenario.EnemyRTB = teamBBase;

            scenario.Waypoints.CreateWaypoint("spawncamp_B", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Protection, baseB1));
            scenario.Waypoints.CreateWaypoint("spawncamp_B", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Protection, baseB2));

            List<Objective> objectives = [];
            objectives.Add(CreateObjectiveForJoinDiscord(objectiveCount++, objectives.Count, teamBBase));
            objectives.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectives.Count));

            for (int i = MaxWarnDistance; i >= 0; --i)
                objectives.Add(CreateObjectiveForSpawnProtDist(objectiveCount++, objectives.Count, i));

            scenario.AlliedObjectives = objectives.ToArray();

            List<Objective> objectivesb = [];
            objectivesb.Add(CreateObjectiveForJoinDiscord(objectiveCount++, objectivesb.Count, teamABase));
            objectivesb.Add(CreateObjectiveForSpawnProt(objectiveCount++, objectivesb.Count));

            for (int i = MaxWarnDistance; i >= 0; --i)
                objectives.Add(CreateObjectiveForSpawnProtDist(objectiveCount++, objectivesb.Count, i));

            scenario.EnemyObjectives = objectivesb.ToArray();

            scenario.EventSequences = new SequenceCollection();
            List<string> scenarioResources = [];

            Folder audioFolder = scenario.EventSequences.CreateFolder("Kill Audio");

            foreach (string fileName in AudioFiles)
            {
                string filePath = $"HSKillEffects\\{fileName}.mp3";

                scenarioResources.Add(filePath);

                EventSequence playAudio = scenario.EventSequences.CreateSequence(fileName, false);
                playAudio.Folder = audioFolder;
                playAudio.Events = [
                    new Event("play", TimeSpan.Zero, null, [new EventTarget(() => GameSystem.PlayCopilotRadioMessageLowPriority(filePath))]),
                ];
            }

            scenarioResources.Add("Server Profile RankedBadge.png");

            scenario.ResourceManifest = scenarioResources.ToArray();
        }

        public override void GeneratePreview(Canvas canvas, VTMapCustom map, CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.GeneratePreview(canvas, map, scenario, teamABases, teamBBases);

            if (teamABases.Length < 2 || teamBBases.Length < 2)
                return;

            BaseInfo baseA1 = teamABases[0];
            BaseInfo baseA2 = teamABases[1];
            BaseInfo baseB1 = teamBBases[0];
            BaseInfo baseB2 = teamBBases[1];

            Vector3 bullseye = (baseA1.Prefab.GlobalPos + baseA2.Prefab.GlobalPos + baseB1.Prefab.GlobalPos + baseB2.Prefab.GlobalPos) / 4.0f;

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
            Vector2 baseAMapLocation = worldToPreview(baseA1.Prefab.GlobalPos, map);

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

            Vector2 baseBMapLocation = worldToPreview(baseB1.Prefab.GlobalPos, map);

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

        protected override SolidColorBrush GetTeamColor(Team team, int index)
        {
            byte primaryColor = (byte)(index < 2 ? 255 : 127);
            byte secondaryColor = (byte)(index < 2 ? 80 : 0);

            return team == Team.Allied ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, secondaryColor, secondaryColor, primaryColor)) : new SolidColorBrush(Windows.UI.Color.FromArgb(255, primaryColor, secondaryColor, secondaryColor));
        }

        static Objective CreateObjectiveForJoinDiscord(int objectiveId, int orderId, Waypoint waypoint)
        {
            return new Objective()
            {
                ObjectiveName = "Join the Discord",
                ObjectiveInfo = "Join the Discord: https://discord.gg/UVYvpJ4jkf",
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
                StartEvent = new ObjectiveEvent("Start Event", [new EventTarget(() => VT.Methods.GameSystem.DisplayMessage("Approaching spawncamp protection zone", 1))]),
            };

            return objective;
        }

        static Objective CreateObjectiveForSpawnProtDist(int objectiveId, int orderId, int distance)
        {
            string distanceString = distance.ToString();
            Objective objective = new()
            {
                ObjectiveName = distanceString,
                ObjectiveInfo = string.Empty,
                ObjectiveID = objectiveId,
                OrderID = orderId,
                Required = false,
                AutoSetWaypoint = false,
                StartMode = ObjectiveStartMode.Triggered,
                ObjectiveType = ObjectiveType.Conditional,
                Fields = new ConditionalObjectiveFields(),
                StartEvent = new ObjectiveEvent("Start Event", [new EventTarget(() => VT.Methods.GameSystem.DisplayMessage(distanceString, 5))]),
            };

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

        private void AddAircraftToBases(BaseInfo baseInfo1, BaseInfo baseInfo2, List<IUnitSpawner> spawners, Team team)
        {
            string layout1 = GetLayout(baseInfo1, 0);
            if (string.IsNullOrWhiteSpace(layout1))
                throw new ScenarioCreationException($"Layout for base 1 of team {team} can not be nothing");

            string layout2 = GetLayout(baseInfo2, 0);
            if (string.IsNullOrWhiteSpace(layout2))
                throw new ScenarioCreationException($"Layout for base 2 of team {team} can not be nothing");

            AirbaseLayoutConfig layoutConfig1 = layoutService.GetConfig(layout1, baseInfo1.Prefab.Prefab);
            AirbaseLayoutConfig layoutConfig2 = layoutService.GetConfig(layout2, baseInfo2.Prefab.Prefab);

            if (layoutConfig1.Aircraft.Length != layoutConfig2.Aircraft.Length)
                throw new ScenarioCreationException($"Layouts for base 1 and 2 of team {team} are not compatible. The number of aircraft must be the same.");

            for (int i = 0; i < layoutConfig1.Aircraft.Length; ++i)
            {
                AircraftConfig aircraft1 = layoutConfig1.Aircraft[i];
                AircraftConfig aircraft2 = layoutConfig2.Aircraft[i];

                Vector3 location1 = MathHelpers.BaseToWorld(aircraft1.Location, baseInfo1);
                Vector3 rotation1 = baseInfo1.Prefab.Rotation + aircraft1.Rotation;
                MathHelpers.ClampRotation(ref rotation1);

                Vector3 location2 = MathHelpers.BaseToWorld(aircraft2.Location, baseInfo2);
                Vector3 rotation2 = baseInfo2.Prefab.Rotation + aircraft2.Rotation;
                MathHelpers.ClampRotation(ref rotation2);

                string group = GetAircraftGroup(team, layoutConfig1.Aircraft[0]);
                MultiplayerSpawn spawn = CreateAircraft(team, group, aircraft1, location1, rotation1, spawners.Count);
                spawn.MultiplayerSpawnFields.SlotLabel = "Airbase 1";

                List<AltSpawn> altSpawns = [];
                for (int sp = 1; sp < aircraft1.Spawns.Length; ++sp)
                {
                    AltSpawn alt = AddAltSpawn(aircraft1, location1, rotation1, spawn.MultiplayerSpawnFields.UnitGroup, sp, altSpawns);
                    alt.MultiplayerSpawnFields.SlotLabel = "Airbase 1";
                }

                for (int sp = 0; sp < aircraft2.Spawns.Length; ++sp)
                {
                    AltSpawn alt = AddAltSpawn(aircraft2, location2, rotation2, spawn.MultiplayerSpawnFields.UnitGroup, sp, altSpawns);
                    alt.MultiplayerSpawnFields.SlotLabel = "Airbase 2";
                }

                spawn.AltSpawns = altSpawns.ToArray();

                spawners.Add(spawn);
            }
        }
    }
}
