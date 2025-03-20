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

namespace VtolVrRankedMissionSetup.Services.ScenarioCreation
{
    [Service(ServiceLifetime.Singleton)]
    public class FiveFiveOneKotHScenarioCreationService : ScenarioCreationService
    {
        private static TimeSpan TieTimespan = TimeSpan.FromSeconds(15);
        private const double StartDist = 1500;
        private const int FirstToWins = 3;

        private static TimeSpan ControlPointDelay = TimeSpan.FromMinutes(1);
        private static TimeSpan ControlTimespan = TimeSpan.FromMinutes(1);
        private const double ControlRadius = 5 * Units.NauticalMiles;

        public FiveFiveOneKotHScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService) : base(scenarioMode, layoutService) { }

        public override void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.SetUpScenario(scenario, teamABases, teamBBases);

            scenario.CampaignID = "551";
            scenario.ScenarioDescription = $"Ringtail's 5-5-1 with King of the Hill Stalemate resolution.\nControl point radius is {ControlRadius / Units.NauticalMiles} NM ({ControlRadius/Units.Kilometers:0.##} km) and will activate after {ControlPointDelay.TotalMinutes:0} minutes";

            scenario.Briefing =
            [
                new BriefingNote()
                {
                    Text = $"5 vs 5, 1 life each.\nLast team with a player alive wins.\nIf said last player dies within 15 seconds of the other team, then it's a draw.\nFirst to {FirstToWins} wins",
                },
                new BriefingNote()
                {
                    Text = $"Some people (who won't be named) have been taking too long to find the other team. So as an insentive: a control point will activate after {ControlPointDelay.TotalMinutes:0} minutes.\nThe control point will only tick up if your team is the only one on it.",
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

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(baseA.Layout ?? "551", baseA.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(baseB.Layout ?? "551", baseB.Prefab.Prefab);

            Waypoint bullseye = scenario.Waypoints.CreateWaypoint("Bullseye", (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            scenario.AlliedRTB = scenario.Waypoints.CreateWaypoint("Team A RTB", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA));
            scenario.EnemyRTB = scenario.Waypoints.CreateWaypoint("Team B RTB", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB));

            IEnumerable<MultiplayerSpawn> spawns = scenario.Units!
                .Select(u => (u as MultiplayerSpawn)!)
                .Where(u => u != null);

            IEnumerable<IUnitSpawner> teamASpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Allied"));
            IEnumerable<IUnitSpawner> teamBSpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Enemy"));

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
                [new EventTarget(() => GameSystem.IncrementValue(teamAWins, 1))],
                [],
                [],
                [/* doNothing */]);

            ConditionalAction teamBTie = scenario.ConditionalActions.CreateConditionalAction(
                "Give tie points to B",
                () => teamBWins < tiePointThreshold,
                [new EventTarget(() => GameSystem.IncrementValue(teamBWins, 1))],
                [],
                [],
                [/* doNothing */]);

            ConditionalAction grantTeamAPointsOrTie = scenario.ConditionalActions.CreateConditionalAction(
                "Give Points to A or tie",
                () => SCCUnitList.SCC_NumAlive(teamASpawns) > 0,
                [
                    new EventTarget(() => GameSystem.IncrementValue(teamAWins, 1)),
                    new EventTarget(() => GameSystem.DisplayMessage("Team A Victory!\nPull chutes", 10))
                ],
                [],
                [],
                [
                    new EventTarget(() => GameSystem.FireConditionalAction(teamATie)),
                    new EventTarget(() => GameSystem.FireConditionalAction(teamBTie)),
                    new EventTarget(() => GameSystem.IncrementValue(totalTies, 1)),
                ]);

            ConditionalAction grantTeamBPointsOrTie = scenario.ConditionalActions.CreateConditionalAction(
                "Give Points to B or tie",
                () => SCCUnitList.SCC_NumAlive(teamBSpawns) > 0,
                [
                    new EventTarget(() => GameSystem.IncrementValue(teamBWins, 1)),
                    new EventTarget(() => GameSystem.DisplayMessage("Team B Victory!\nPull chutes", 10)),
                ],
                [],
                [],
                [
                    new EventTarget(() => GameSystem.FireConditionalAction(teamATie)),
                    new EventTarget(() => GameSystem.FireConditionalAction(teamBTie)),
                    new EventTarget(() => GameSystem.IncrementValue(totalTies, 1)),
                ]);

            Objective teamAWinObjective = CreateObjectiveForWin(objectiveCount++, AlliedObjectives.Count, bullseye, teamAWins, targetWins, teamBWins, totalTies);
            AlliedObjectives.Add(teamAWinObjective);

            Objective teamBWinObjective = CreateObjectiveForWin(objectiveCount++, EnemyObjectives.Count, bullseye, teamBWins, targetWins, teamAWins, totalTies);
            EnemyObjectives.Add(teamBWinObjective);

            //////////////////////////////////////////////////////////////
            // Standard 551
            //////////////////////////////////////////////////////////////
            Conditional whenTeamAIsDead = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamASpawns) == 0);
            Conditional whenTeamBIsDead = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamBSpawns) == 0);

            // use conditional for this instead of destroy so the count of remaining players is not visible
            Objective teamAKillObjective = CreateObjectiveForKill(objectiveCount++, AlliedObjectives.Count, bullseye, whenTeamBIsDead);
            teamAKillObjective.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamATieCheck.Restart())];
            AlliedObjectives.Add(teamAKillObjective);

            Objective teamBKillObjective = CreateObjectiveForKill(objectiveCount++, EnemyObjectives.Count, bullseye, whenTeamAIsDead);
            teamBKillObjective.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamBTieCheck.Restart())];
            EnemyObjectives.Add(teamBKillObjective);

            //////////////////////////////////////////////////////////////
            // KotH
            //////////////////////////////////////////////////////////////
            GlobalValue teamAKotHTime = scenario.GlobalValues.CreateGlobalValue("teamAKotHTime");
            GlobalValue teamBKotHTime = scenario.GlobalValues.CreateGlobalValue("teamBKotHTime");
            GlobalValue targetKotHTime = scenario.GlobalValues.CreateGlobalValue("targetKotHTime", (int)Math.Round(ControlTimespan.TotalSeconds));

            Objective teamAKotH = CreateForKotH(objectiveCount++, AlliedObjectives.Count, bullseye, teamAKotHTime, targetKotHTime);
            teamAKotH.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamATieCheck.Restart())];
            AlliedObjectives.Add(teamAKotH);

            Objective teamBKotH = CreateForKotH(objectiveCount++, EnemyObjectives.Count, bullseye, teamBKotHTime, targetKotHTime);
            teamBKotH.CompleteEvent.EventInfo.EventTargets = [new EventTarget(() => teamBTieCheck.Restart())];
            EnemyObjectives.Add(teamBKotH);

            ConditionalAction KothPoints = scenario.ConditionalActions.CreateConditionalAction(
                "KotH Points",
                () => SCCUnitList.SCC_AnyNearWaypoint(teamASpawns, bullseye, ControlRadius) && SCCUnitList.SCC_AnyNearWaypoint(teamBSpawns, bullseye, ControlRadius),
                [/* doNothing */],
                [() => SCCUnitList.SCC_AnyNearWaypoint(teamASpawns, bullseye, ControlRadius), () => SCCUnitList.SCC_AnyNearWaypoint(teamBSpawns, bullseye, ControlRadius)],
                [[new EventTarget(() => GameSystem.IncrementValue(teamAKotHTime, 1))], /**/ [new EventTarget(() => GameSystem.IncrementValue(teamBKotHTime, 1))]],
                [/* doNothing */]);

            EventSequence kothCheck = scenario.EventSequences.CreateSequence("KotH check", false);
            kothCheck.Events =
            [
                new Event("Check", TimeSpan.Zero, null, [new EventTarget(() => GameSystem.FireConditionalAction(KothPoints))]),
                new Event("Reset", TimeSpan.FromSeconds(1), null, [new EventTarget(() => kothCheck.Restart())]),
            ];

            //////////////////////////////////////////////////////////////
            // Start/Cleanup
            //////////////////////////////////////////////////////////////
            
            // At least one person alive on each team, and at least one team has no one at the runway
            Conditional atLeast1OnEachSideSpawned = scenario.Conditionals.CreateCondition(() =>
                SCCUnitList.SCC_NumAlive(teamASpawns) > 0 &&
                SCCUnitList.SCC_NumAlive(teamBSpawns) > 0);

            Conditional oneTeamTakeoff = scenario.Conditionals.CreateCondition(() =>
                !SCCUnitList.SCC_AnyNearWaypoint(teamASpawns, scenario.AlliedRTB, StartDist) ||
                !SCCUnitList.SCC_AnyNearWaypoint(teamBSpawns, scenario.EnemyRTB, StartDist));

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

            EventSequence startMatch = scenario.EventSequences.CreateSequence("Start Match", false);
            startMatch.Events =
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
                        new EventTarget(() => teamAKillObjective.BeginObjective()),
                        new EventTarget(() => teamBKillObjective.BeginObjective()),
                    ]),
                new Event(
                    "koth",
                    ControlPointDelay,
                    null,
                    [
                        new EventTarget(() => teamAKotH.BeginObjective()),
                        new EventTarget(() => teamBKotH.BeginObjective()),
                        new EventTarget(() => kothCheck.Restart()),
                    ]),
            ];

            EventSequence resetMatch = scenario.EventSequences.CreateSequence("Reset 551");
            resetMatch.Events =
            [
                new Event(
                    "reset",
                    TimeSpan.FromSeconds(1), 
                    null,
                    [
                        new EventTarget(() => GameSystem.ResetValue(teamBKotHTime)),
                        new EventTarget(() => GameSystem.ResetValue(teamAKotHTime)),
                        new EventTarget(() => teamAKillObjective.ResetObjective()),
                        new EventTarget(() => teamBKillObjective.ResetObjective()),
                        new EventTarget(() => teamAKotH.ResetObjective()),
                        new EventTarget(() => teamBKotH.ResetObjective()),
                        new EventTarget(() => kothCheck.Stop()),
                        new EventTarget(() => startMatch.Restart()),
                        new EventTarget(() => GameSystem.DisplayMessage("Reset", 5)),
                    ]),
            ];

            Conditional everyoneIsDead = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(spawns) == 0);

            teamATieCheck.Events =
            [
                new Event("delay", TimeSpan.Zero, null,
                [
                    new EventTarget(() => GameSystem.DisplayMessage("It's not over yet, keep flying", 5)),
                    new EventTarget(() => teamBKotH.FailObjective()),
                    new EventTarget(() => teamBKillObjective.FailObjective()),
                ]),
                new Event("Still alive?", TieTimespan, null, [new EventTarget(() => GameSystem.FireConditionalAction(grantTeamAPointsOrTie))]),
                new Event("reset match", TimeSpan.Zero, everyoneIsDead, [new EventTarget(() => resetMatch.Restart())]),
            ];

            teamBTieCheck.Events =
            [
                new Event("delay", TimeSpan.Zero, null,
                [
                    new EventTarget(() => GameSystem.DisplayMessage("It's not over yet, keep flying", 5)),
                    new EventTarget(() => teamAKotH.FailObjective()),
                    new EventTarget(() => teamAKillObjective.FailObjective()),
                ]),
                new Event("Reset Sequence", TieTimespan, null, [new EventTarget(() => GameSystem.FireConditionalAction(grantTeamBPointsOrTie))]),
                new Event("reset match", TimeSpan.Zero, everyoneIsDead, [new EventTarget(() => resetMatch.Restart())]),
            ];

            scenario.AlliedObjectives = AlliedObjectives.ToArray();
            scenario.EnemyObjectives = EnemyObjectives.ToArray();
        }

        protected override void PopulateAirbase(BaseInfo baseInfo, List<IUnitSpawner> spawners, Team team)
        {
            if (baseInfo.Layout == null)
                return;

            AirbaseLayoutConfig layoutConfig = layoutService.GetConfig(baseInfo.Layout, baseInfo.Prefab.Prefab);

            AddAircraftToBase(baseInfo, layoutConfig.F26, "F/A-26B", spawners, $"{team}:Foxtrot");
            AddAircraftToBase(baseInfo, layoutConfig.F45, "F-45A", spawners, $"{team}:Golf");
            AddAircraftToBase(baseInfo, layoutConfig.F24, "EF-24G", spawners, $"{team}:Echo", 2);
            AddAircraftToBase(baseInfo, layoutConfig.T55, "T-55", spawners, $"{team}:Foxtrot", 1);
            AddAircraftToBase(baseInfo, layoutConfig.F16, "F-16", spawners, $"{team}:Foxtrot", 1);
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

        private static Objective CreateObjectiveForKill(int objectiveId, int orderId, Waypoint waypoint, Conditional success)
        {
            return new Objective()
            {
                ObjectiveName = "Happy Hunting",
                ObjectiveInfo = "Kill all enemies\n(hint: there are 5 of them)",
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
                ObjectiveInfo = $"Control the bullseye for {ControlTimespan.TotalMinutes:0} minutes\nTime will only tick up if your team is the only one on the point\nControl point radius is {ControlRadius / Units.NauticalMiles:0.##} NM ({ControlRadius / Units.Kilometers:0.##} km)",
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
    }
}
