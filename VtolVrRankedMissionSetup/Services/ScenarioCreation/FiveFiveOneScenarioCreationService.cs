using System.Collections.Generic;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using Microsoft.Extensions.DependencyInjection;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;
using System.Linq;
using VtolVrRankedMissionSetup.VT.Methods;
using System;
using VtolVrRankedMissionSetup.VTS.Events;
using VtolVrRankedMissionSetup.VTS.Objectives;
using VtolVrRankedMissionSetup.VTS.UnitFields;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using VtolVrRankedMissionSetup.VTM;
using System.Numerics;
using System.Buffers.Text;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.Services.ScenarioCreation
{
    [Service(ServiceLifetime.Singleton)]
    public class FiveFiveOneScenarioCreationService : ScenarioCreationService
    {
        /// <summary>
        /// The time before the points are awarded. If all players are dead at the end, a tie is awarded
        /// </summary>
        private static TimeSpan TieTimespan = TimeSpan.FromSeconds(15);
        /// <summary>
        /// Distance in meters away from the RTB point to start the round
        /// </summary>
        private const double StartDist = 1500;
        /// <summary>
        /// How many points a team needs to win a match
        /// </summary>
        private const int FirstToWins = 3;

        /// <summary>
        /// How long before the KotH control point activates
        /// </summary>
        private static TimeSpan ControlPointActivationDelay = TimeSpan.FromMinutes(10);
        /// <summary>
        /// How long a team must solely control the control point to win
        /// </summary>
        private static TimeSpan ControlPointRequiredHoldTime = TimeSpan.FromMinutes(2);
        /// <summary>
        /// The radius of the control point
        /// </summary>
        private const double ControlRadius = 10 * Units.NauticalMiles;

        public FiveFiveOneScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService) : base(scenarioMode, layoutService) { }

        public override void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.SetUpScenario(scenario, teamABases, teamBBases);

            scenario.CampaignID = "551";
            scenario.ScenarioDescription = $"Ringtail's 5-5-1 with King of the Hill Stalemate resolution.\nControl point radius is {ControlRadius / Units.NauticalMiles:0.#} NM ({ControlRadius / Units.Kilometers:0.##} km) and will activate after {ControlPointActivationDelay.TotalMinutes:0} minutes";

            scenario.Briefing =
            [
                new BriefingNote()
                {
                    Text = $"5 vs 5, 1 life each.\nLast team with a player alive wins.\nIf said last player dies within {TieTimespan.TotalSeconds:0.#} seconds of the other team, then it's a draw.\nFirst to {FirstToWins} wins",
                },
                new BriefingNote()
                {
                    Text = $"Some people (who won't be named) have been taking too long to find the other team. So as an insentive: a control point will activate after {ControlPointActivationDelay.TotalMinutes:0} minutes.\nThe control point will only tick up if your team is the only one on it.",
                },
                new BriefingNote()
                {
                    Text = "Join us at https://discord.gg/ecSSndCbVc",
                },
            ];

            scenario.Conditionals = new ConditionalCollection();
            scenario.GlobalValues = new GlobalValueCollection();
            scenario.Waypoints = new WaypointCollection();
            scenario.ConditionalActions = new();
            scenario.EventSequences = new SequenceCollection();
            scenario.TimedEventGroups = new TimedEventGroupCollection();
            scenario.TriggerEvents = new();

            int objectiveCount = 0;
            List<Objective> AlliedObjectives = [];
            List<Objective> EnemyObjectives = [];

            BaseInfo baseA = teamABases[0];
            BaseInfo baseB = teamBBases[0];

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(GetLayout(baseA, true), baseA.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(GetLayout(baseB, true), baseB.Prefab.Prefab);

            Waypoint bullseye = scenario.Waypoints.CreateWaypoint("Bullseye", (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            scenario.AlliedRTB = scenario.Waypoints.CreateWaypoint("Team A RTB", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA));
            scenario.EnemyRTB = scenario.Waypoints.CreateWaypoint("Team B RTB", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB));

            MultiplayerSpawn[] spawns = scenario.Units!
                .Select(u => (u as MultiplayerSpawn)!)
                .Where(u => u != null)
                .ToArray();

            IUnitSpawner[] teamASpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Allied")).ToArray();
            IUnitSpawner[] teamBSpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Enemy")).ToArray();

            foreach (MultiplayerSpawn spawn in spawns)
                ((MultiplayerSpawnFields)spawn.UnitFields!).LimitedLives = true;

            List<Waypoint> teamABaseWaypoints = [];

            foreach (BaseInfo b in teamABases)
            {
                if (string.IsNullOrWhiteSpace(b.Layout) && b != baseA)
                    continue;

                teamABaseWaypoints.Add(scenario.Waypoints.CreateWaypoint(b.DisplayName, b.Prefab.GlobalPos));
            }

            List<Waypoint> teamBBaseWaypoints = [];

            foreach (BaseInfo b in teamBBases)
            {
                if (string.IsNullOrWhiteSpace(b.Layout) && b != baseB)
                    continue;

                teamBBaseWaypoints.Add(scenario.Waypoints.CreateWaypoint(b.DisplayName, b.Prefab.GlobalPos));
            }

            //////////////////////////////////////////////////////////////
            // Points
            //////////////////////////////////////////////////////////////
            GlobalValue teamAWins = scenario.GlobalValues.CreateGlobalValue("teamAWins");
            GlobalValue teamBWins = scenario.GlobalValues.CreateGlobalValue("teamBWins");
            GlobalValue totalTies = scenario.GlobalValues.CreateGlobalValue("ties");
            GlobalValue targetWins = scenario.GlobalValues.CreateGlobalValue("targetWins", FirstToWins);

            EventSequence teamATieCheck = scenario.EventSequences.CreateSequence("Team A Victory?", false);
            EventSequence teamBTieCheck = scenario.EventSequences.CreateSequence("Team B Victory?", false);

            const int tiePointThreshold = FirstToWins - 1;
            ConditionalAction teamATie = scenario.ConditionalActions.CreateConditionalAction(
                "Give tie points to A",
                () => teamAWins < tiePointThreshold,
                [new EventTarget(() => teamAWins.IncrementValue())],
                [],
                [],
                [/* doNothing */]);

            ConditionalAction teamBTie = scenario.ConditionalActions.CreateConditionalAction(
                "Give tie points to B",
                () => teamBWins < tiePointThreshold,
                [new EventTarget(() => teamBWins.IncrementValue())],
                [],
                [],
                [/* doNothing */]);

            ConditionalAction grantTeamAPointsOrTie = scenario.ConditionalActions.CreateConditionalAction(
                "Give Points to A or tie",
                () => teamASpawns.NumAlive() > 0,
                [
                    new EventTarget(() => teamAWins.IncrementValue()),
                    new EventTarget(() => GameSystem.DisplayMessage("Team A Victory!\nPull chutes", 10))
                ],
                [],
                [],
                [
                    new EventTarget(() => teamATie.FireConditionalAction()),
                    new EventTarget(() => teamBTie.FireConditionalAction()),
                    new EventTarget(() => totalTies.IncrementValue()),
                ]);

            ConditionalAction grantTeamBPointsOrTie = scenario.ConditionalActions.CreateConditionalAction(
                "Give Points to B or tie",
                () => teamBSpawns.NumAlive() > 0,
                [
                    new EventTarget(() => teamBWins.IncrementValue()),
                    new EventTarget(() => GameSystem.DisplayMessage("Team B Victory!\nPull chutes", 10)),
                ],
                [],
                [],
                [
                    new EventTarget(() => teamATie.FireConditionalAction()),
                    new EventTarget(() => teamBTie.FireConditionalAction()),
                    new EventTarget(() => totalTies.IncrementValue()),
                ]);

            Objective teamAWinObjective = CreateObjectiveForWin(objectiveCount++, AlliedObjectives.Count, bullseye, teamAWins, targetWins, teamBWins, totalTies);
            AlliedObjectives.Add(teamAWinObjective);

            Objective teamBWinObjective = CreateObjectiveForWin(objectiveCount++, EnemyObjectives.Count, bullseye, teamBWins, targetWins, teamAWins, totalTies);
            EnemyObjectives.Add(teamBWinObjective);

            //////////////////////////////////////////////////////////////
            // Standard 551
            //////////////////////////////////////////////////////////////

            GlobalValue sortieAbortable = scenario.GlobalValues.CreateGlobalValue("sortieAbortable", 1);

            Conditional whenTeamAIsDead = scenario.Conditionals.CreateCondition(() => teamASpawns.NumAlive() == 0);
            Conditional whenTeamBIsDead = scenario.Conditionals.CreateCondition(() => teamBSpawns.NumAlive() == 0);

            EventSequence abortSortieDelay = scenario.EventSequences.CreateSequence("abort sortie", false);

            ConditionalAction teamAResetOrPoint = scenario.ConditionalActions.CreateConditionalAction(
                "Team A won or Sortie Reset",
                () => sortieAbortable == 1,
                [
                    new EventTarget(() => abortSortieDelay.Restart()),
                ],
                [],
                [],
                [
                    new EventTarget(() => teamATieCheck.Restart())
                ]);

            ConditionalAction teamBResetOrPoint = scenario.ConditionalActions.CreateConditionalAction(
                "Team B won or Sortie Reset",
                () => sortieAbortable == 1,
                [
                    new EventTarget(() => abortSortieDelay.Restart()),
                ],
                [],
                [],
                [
                    new EventTarget(() => teamBTieCheck.Restart())
                ]);

            // use conditional for this instead of destroy so the count of remaining players is not visible
            Objective teamAKillObjective = CreateObjectiveForKill(objectiveCount++, AlliedObjectives.Count, bullseye, whenTeamBIsDead, sortieAbortable);
            teamAKillObjective.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamAResetOrPoint.FireConditionalAction())];
            AlliedObjectives.Add(teamAKillObjective);

            Objective teamBKillObjective = CreateObjectiveForKill(objectiveCount++, EnemyObjectives.Count, bullseye, whenTeamAIsDead, sortieAbortable);
            teamBKillObjective.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamBResetOrPoint.FireConditionalAction())];
            EnemyObjectives.Add(teamBKillObjective);

            EventSequence damageDealt = scenario.EventSequences.CreateSequence("Damage Dealt");
            damageDealt.WhileLoop = true;
            damageDealt.Events =
                [
                    new Event(
                        "Resetable",
                        TimeSpan.Zero,
                        scenario.Conditionals.CreateCondition(() => sortieAbortable == 1),
                        []),
                    new Event(
                        "When Damaged",
                        TimeSpan.Zero,
                        scenario.Conditionals.CreateCondition(() => teamASpawns.AnyGetsDamagedBy(teamBSpawns) || teamBSpawns.AnyGetsDamagedBy(teamASpawns)),
                        [
                            new EventTarget(() => sortieAbortable.SetValue(0)),
                        ]),
                    new Event(
                        "Reset",
                        TimeSpan.FromSeconds(1),
                        null,
                        [
                            new EventTarget(() => damageDealt.Restart()),
                        ]),
                ];

            //////////////////////////////////////////////////////////////
            // KotH
            //////////////////////////////////////////////////////////////
            GlobalValue teamAKotHTime = scenario.GlobalValues.CreateGlobalValue("teamAKotHTime");
            GlobalValue teamBKotHTime = scenario.GlobalValues.CreateGlobalValue("teamBKotHTime");
            GlobalValue targetKotHTime = scenario.GlobalValues.CreateGlobalValue("targetKotHTime", (int)Math.Round(ControlPointRequiredHoldTime.TotalSeconds));

            Objective teamAKotH = CreateForKotH(objectiveCount++, AlliedObjectives.Count, bullseye, teamAKotHTime, targetKotHTime);
            teamAKotH.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamATieCheck.Restart())];
            AlliedObjectives.Add(teamAKotH);

            Objective teamBKotH = CreateForKotH(objectiveCount++, EnemyObjectives.Count, bullseye, teamBKotHTime, targetKotHTime);
            teamBKotH.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamBTieCheck.Restart())];
            EnemyObjectives.Add(teamBKotH);

            ConditionalAction KothPoints = scenario.ConditionalActions.CreateConditionalAction(
                "KotH Points",
                () => teamASpawns.AnyNearWaypoint(bullseye, ControlRadius) && teamBSpawns.AnyNearWaypoint(bullseye, ControlRadius),
                [/* doNothing */],
                [() => teamASpawns.AnyNearWaypoint(bullseye, ControlRadius), () => teamBSpawns.AnyNearWaypoint(bullseye, ControlRadius)],
                [[new EventTarget(() => teamAKotHTime.IncrementValue())], /**/ [new EventTarget(() => teamBKotHTime.IncrementValue())]],
                [/* doNothing */]);

            EventSequence kothCheck = scenario.EventSequences.CreateSequence("KotH check", false);
            kothCheck.Events =
            [
                new Event("Check", TimeSpan.Zero, null, [new EventTarget(() => KothPoints.FireConditionalAction())]),
                new Event("Reset", TimeSpan.FromSeconds(1), null, [new EventTarget(() => kothCheck.Restart())]),
            ];

            //////////////////////////////////////////////////////////////
            // Collision
            //////////////////////////////////////////////////////////////

            Folder collisionFolder = scenario.EventSequences.CreateFolder("Collisions");

            for (int i = 0; i < spawns.Length - 1; ++i)
            {
                IUnitSpawner unit = spawns[i];
                CreateCollisionForAircaraft(scenario, unit, spawns[(i + 1)..], collisionFolder);
            }

            //////////////////////////////////////////////////////////////
            // Start/Cleanup
            //////////////////////////////////////////////////////////////

            // At least one person alive on each team, and at least one team has no one at the runway
            Conditional atLeast1OnEachSideSpawned = scenario.Conditionals.CreateCondition(() =>
                teamASpawns.NumAlive() > 0 &&
                teamBSpawns.NumAlive() > 0);

            Conditional oneTeamTakeoff = scenario.Conditionals.CreateCondition(() =>
                (
                    teamABaseWaypoints.All(waypoint => !teamASpawns.AnyNearWaypoint(waypoint, StartDist)) ||
                    teamBBaseWaypoints.All(waypoint => !teamBSpawns.AnyNearWaypoint(waypoint, StartDist))
                ) &&
                teamASpawns.NumAlive() > 0 &&
                teamBSpawns.NumAlive() > 0);

            teamAKillObjective.CompleteEvent.EventInfo.EventTargets =
            [
                ..teamAKillObjective.CompleteEvent.EventInfo.EventTargets,
                new EventTarget(() => teamAKotH.CancelObjective()),
            ];
            teamAKotH.CompleteEvent.EventInfo.EventTargets =
            [
                ..teamAKotH.CompleteEvent.EventInfo.EventTargets,
                new EventTarget(() => teamAKillObjective.CancelObjective()),
            ];

            teamBKillObjective.CompleteEvent.EventInfo.EventTargets =
            [
                ..teamBKillObjective.CompleteEvent.EventInfo.EventTargets,
                new EventTarget(() => teamBKotH.CancelObjective()),
            ];
            teamBKotH.CompleteEvent.EventInfo.EventTargets =
            [
                ..teamBKotH.CompleteEvent.EventInfo.EventTargets,
                new EventTarget(() => teamBKillObjective.CancelObjective()),
            ];

            List<EventTarget> setLives100 = [];
            foreach (MultiplayerSpawn spawn in spawns)
                setLives100.Add(new EventTarget(() => spawn.SetLives(100)));

            List<EventTarget> setLives0 = [];
            foreach (MultiplayerSpawn spawn in spawns)
                setLives0.Add(new EventTarget(() => spawn.SetLives(0)));

            EventSequence startSortie = scenario.EventSequences.CreateSequence("Start Sortie", false);
            startSortie.Events =
            [
                new Event(
                    "spawned",
                    TimeSpan.FromSeconds(1),
                    atLeast1OnEachSideSpawned,
                    [
                    ]),
                new Event(
                    "start",
                    TimeSpan.FromSeconds(1),
                    oneTeamTakeoff,
                    [
                        new EventTarget(() => teamAKillObjective.ResetObjective()),
                        new EventTarget(() => teamBKillObjective.ResetObjective()),
                        new EventTarget(() => teamAKillObjective.BeginObjective()),
                        new EventTarget(() => teamBKillObjective.BeginObjective()),
                        ..setLives0,
                    ]),
                new Event(
                    "koth",
                    ControlPointActivationDelay,
                    null,
                    [
                        new EventTarget(() => teamAKotH.BeginObjective()),
                        new EventTarget(() => teamBKotH.BeginObjective()),
                        new EventTarget(() => kothCheck.Restart()),
                    ]),
            ];

            EventSequence endSortie = scenario.EventSequences.CreateSequence("Reset Sortie");
            endSortie.Events =
            [
                new Event(
                    "reset",
                    TimeSpan.FromSeconds(1),
                    null,
                    [
                        new EventTarget(() => teamBKotHTime.ResetValue()),
                        new EventTarget(() => teamAKotHTime.ResetValue()),
                        new EventTarget(() => teamAKillObjective.CancelObjective()),
                        new EventTarget(() => teamBKillObjective.CancelObjective()),
                        new EventTarget(() => teamAKotH.CancelObjective()),
                        new EventTarget(() => teamBKotH.CancelObjective()),
                        new EventTarget(() => kothCheck.Stop()),
                        new EventTarget(() => startSortie.Restart()),
                        new EventTarget(() => sortieAbortable.SetValue(1)),
                        //new EventTarget(() => GameSystem.DisplayMessage("Reset", 5)),
                        ..setLives100,
                    ]),
            ];

            Conditional everyoneIsDead = scenario.Conditionals.CreateCondition(() => spawns.NumAlive() == 0);

            teamATieCheck.Events =
            [
                new Event("delay", TimeSpan.Zero, null,
                [
                    new EventTarget(() => GameSystem.DisplayMessage("It's not over yet, keep flying", 5)),
                    new EventTarget(() => teamBKotH.FailObjective()),
                    new EventTarget(() => teamBKillObjective.FailObjective()),
                ]),
                new Event("Still alive?", TieTimespan, null, [new EventTarget(() => grantTeamAPointsOrTie.FireConditionalAction())]),
                new Event("reset for next sortie", TimeSpan.Zero, everyoneIsDead, [new EventTarget(() => endSortie.Restart())]),
            ];

            teamBTieCheck.Events =
            [
                new Event("delay", TimeSpan.Zero, null,
                [
                    new EventTarget(() => GameSystem.DisplayMessage("It's not over yet, keep flying", 5)),
                    new EventTarget(() => teamAKotH.FailObjective()),
                    new EventTarget(() => teamAKillObjective.FailObjective()),
                ]),
                new Event("Still alive?", TieTimespan, null, [new EventTarget(() => grantTeamBPointsOrTie.FireConditionalAction())]),
                new Event("reset for next sortie", TimeSpan.Zero, everyoneIsDead, [new EventTarget(() => endSortie.Restart())]),
            ];

            abortSortieDelay.Events =
            [
                new Event(
                    "Pull chutes",
                    TimeSpan.Zero,
                    null,
                    [
                        new EventTarget(() => GameSystem.DisplayMessage("Pull chutes to abort the sortie!", 5)),
                        new EventTarget(() => teamBKotHTime.ResetValue()),
                        new EventTarget(() => teamAKotHTime.ResetValue()),
                        new EventTarget(() => teamAKillObjective.CancelObjective()),
                        new EventTarget(() => teamBKillObjective.CancelObjective()),
                        new EventTarget(() => teamAKotH.CancelObjective()),
                        new EventTarget(() => teamBKotH.CancelObjective()),
                        new EventTarget(() => kothCheck.Stop()),
                    ]),
                new Event(
                    "Wait for everyone to be dead",
                    TimeSpan.Zero,
                    scenario.Conditionals.CreateCondition(() => teamASpawns.NumAlive() == 0 && teamBSpawns.NumAlive() == 0),
                    [
                        new EventTarget(() => endSortie.Restart()),
                    ]),
            ];

            scenario.AlliedObjectives = AlliedObjectives.ToArray();
            scenario.EnemyObjectives = EnemyObjectives.ToArray();

            AddSpectatorSeats(scenario);
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

            double size = worldToPreview(ControlRadius, map);
            Ellipse control = new()
            {
                Stroke = new SolidColorBrush(Colors.Cyan),
                Height = size,
                Width = size,
                StrokeThickness = 1,
            };

            Canvas.SetLeft(control, bullseyeMapLocation.X - (size / 2));
            Canvas.SetTop(control, bullseyeMapLocation.Y - (size / 2));

            canvas.Children.Add(outer);
            canvas.Children.Add(inner);
            canvas.Children.Add(control);
        }

        protected override void PreviewBase(Canvas canvas, BaseInfo bs, VTMapCustom map, Team team, bool primary)
        {
            base.PreviewBase(canvas, bs, map, team, primary);

            string layout = GetLayout(bs, primary);
            if (string.IsNullOrWhiteSpace(layout))
                return;

            AirbaseLayoutConfig airbaseConfig = layoutService.GetConfig(layout, bs.Prefab.Prefab);

            if (airbaseConfig == null)
                return;

            Vector2 baseLocation = worldToPreview(bs.Prefab.GlobalPos, map);

            float rotation = airbaseConfig.Aircraft[0].Rotation.Y + bs.Prefab.Rotation.Y;
            Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(MathHelpers.DegToRad(rotation));
            Vector2 lineEnd = baseLocation + Vector2.Transform(new Vector2(0, -30), rotationMatrix);

            SolidColorBrush teamColor = GetTeamColor(team, primary);

            Line spawnDirectionLine = new()
            {
                Stroke = teamColor,
                StrokeThickness = 3,
                X1 = baseLocation.X,
                Y1 = baseLocation.Y,
                X2 = lineEnd.X,
                Y2 = lineEnd.Y,
            };

            Canvas.SetZIndex(spawnDirectionLine, -1);

            canvas.Children.Add(spawnDirectionLine);
        }

        protected override string GetAircraftGroup(Team team, AircraftConfig aircraft)
        {
            return aircraft.Spawns[0].Type switch
            {
                AircraftType.F26 => "Foxtrot",
                AircraftType.F16 => "Foxtrot",
                AircraftType.T55 => "Foxtrot",
                AircraftType.F22 => "Golf",
                AircraftType.F45 => "Golf",
                AircraftType.F24 => "Echo",
                _ => "Xray",
            };
        }

        private void AddSpectatorSeats(CustomScenario scenario)
        {
            List<IUnitSpawner> units = scenario.Units!.ToList();
            MultiplayerSpawn specA1 = AddSpectator("spectator A1", Team.Allied, units);
            MultiplayerSpawn specA2 = AddSpectator("spectator A2", Team.Allied, units);
            MultiplayerSpawn specB1 = AddSpectator("spectator B3", Team.Enemy, units);
            MultiplayerSpawn specB2 = AddSpectator("spectator B4", Team.Enemy, units);
            MultiplayerSpawn[] specs = [specA1, specA2, specB1, specB2];


            EventSequence kill = scenario.EventSequences!.CreateSequence("Kill spectators");

            Conditional anySpectatorSPawned = scenario.Conditionals!.CreateCondition(() => specs.NumAlive() > 0);

            kill.Events = [
                new Event("Kill", TimeSpan.Zero, anySpectatorSPawned,
                [
                    new EventTarget(() => specA1.DestroyVehicle()),
                    new EventTarget(() => specA2.DestroyVehicle()),
                    new EventTarget(() => specB1.DestroyVehicle()),
                    new EventTarget(() => specB2.DestroyVehicle()),
                ]),
                new Event("Restart", TimeSpan.Zero, anySpectatorSPawned,
                [
                    new EventTarget(() => kill.Restart()),
                ]),
            ];

            scenario.Units = units.ToArray();
        }

        private MultiplayerSpawn AddSpectator(string name, Team team, List<IUnitSpawner> spectatorSeats)
        {
            MultiplayerSpawn spawn = new(team, name)
            {
                UnitInstanceID = spectatorSeats.Count,
                GlobalPosition = Vector3.Zero,
                Rotation = Vector3.Zero,
            };

            spawn.MultiplayerSpawnFields.SelectableAltSpawn = false;
            spawn.MultiplayerSpawnFields.UnitGroup = $"{team}:Sierra";
            spawn.MultiplayerSpawnFields.Vehicle = AircraftType.AV42;
            spawn.MultiplayerSpawnFields.Equipment = "";
            spawn.MultiplayerSpawnFields.Slots = 0;
            spawn.MultiplayerSpawnFields.LimitedLives = false;
            spawn.MultiplayerSpawnFields.SlotLabel = "I'll kill you if you use this";

            spectatorSeats.Add(spawn);
            return spawn;
        }

        private static Objective CreateObjectiveForWin(int objectiveId, int orderId, Waypoint waypoint, GlobalValue current, GlobalValue target, GlobalValue enemyScore, GlobalValue ties)
        {
            return new Objective()
            {
                ObjectiveName = "Win",
                ObjectiveInfo = $"You {current} - {enemyScore} Them\nTies: {ties}\nWin {FirstToWins} rounds.\nTies will grant you a point unless you already have {FirstToWins - 1}",
                ObjectiveID = objectiveId,
                OrderID = orderId,
                Required = true,
                Waypoint = waypoint,
                AutoSetWaypoint = false,
                StartMode = ObjectiveStartMode.Immediate,
                ObjectiveType = ObjectiveType.Global__Value,
                Fields = new GlobalValueObjectiveFields()
                {
                    CurrentValue = current,
                    TargetValue = target,
                },
            };
        }

        private static Objective CreateObjectiveForKill(int objectiveId, int orderId, Waypoint waypoint, Conditional success, GlobalValue sortieResetable)
        {
            return new Objective()
            {
                ObjectiveName = "Happy Hunting",
                ObjectiveInfo = $"Kill all enemies\n(hint: there are 5 of them)\nSortie Abortable: {sortieResetable}",
                ObjectiveID = objectiveId,
                OrderID = orderId,
                Required = false,
                Waypoint = waypoint,
                AutoSetWaypoint = false,
                StartMode = ObjectiveStartMode.Triggered,
                ObjectiveType = ObjectiveType.Conditional,
                Fields = new ConditionalObjectiveFields()
                {
                    SuccessConditional = success,
                },
            };
        }

        private static Objective CreateForKotH(int objectiveId, int orderId, Waypoint waypoint, GlobalValue current, GlobalValue target)
        {
            return new Objective()
            {
                ObjectiveName = "King of the Hill",
                ObjectiveInfo = $"Control the bullseye for {ControlPointRequiredHoldTime.TotalMinutes:0} minutes\nTime will only tick up if your team is the only one on the point\nControl point radius is {ControlRadius / Units.NauticalMiles:0.##} NM ({ControlRadius / Units.Kilometers:0.##} km)",
                ObjectiveID = objectiveId,
                OrderID = orderId,
                Required = false,
                Waypoint = waypoint,
                AutoSetWaypoint = true,
                StartMode = ObjectiveStartMode.Triggered,
                ObjectiveType = ObjectiveType.Global__Value,
                Fields = new GlobalValueObjectiveFields()
                {
                    CurrentValue = current,
                    TargetValue = target,
                }
            };
        }

        private void CreateCollisionForAircaraft(CustomScenario scenario, IUnitSpawner unit, IUnitSpawner[] spawners, Folder folder)
        {
            foreach (IUnitSpawner otherUnit in spawners)
            {
                EventSequence collisionEvents = scenario.EventSequences!.CreateSequence($"[{unit.UnitInstanceID}] vs [{otherUnit.UnitInstanceID}]", true);

                collisionEvents.Folder = folder;

                Conditional collidesWith = scenario.Conditionals!.CreateCondition(() =>
                    unit.SCC_NearWaypoint(otherUnit, 8) &&
                    (
                        otherUnit.SCC_IsUsingAltNumber(0) ||
                        otherUnit.SCC_IsUsingAltNumber(1) ||
                        otherUnit.SCC_IsUsingAltNumber(2) ||
                        otherUnit.SCC_IsUsingAltNumber(3)
                    )
                );

                collisionEvents.Events =
                [
                    new Event(
                        "Kill",
                        TimeSpan.Zero,
                        collidesWith,
                        [
                            new EventTarget(() => unit.DestroyVehicle()),
                            new EventTarget(() => otherUnit.DestroyVehicle())
                        ]
                    ),
                ];
            }
        }
    }
}
