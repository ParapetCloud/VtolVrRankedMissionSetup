using System.Collections.Generic;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using Microsoft.Extensions.DependencyInjection;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;
using System.Linq;
using VtolVrRankedMissionSetup.VT.Methods;
using System;
using VtolVrRankedMissionSetup.VTS.Events;
using Microsoft.UI.Composition.Scenes;

namespace VtolVrRankedMissionSetup.Services.ScenarioCreation
{
    [Service(ServiceLifetime.Singleton)]
    public class FiveFiveOneKotHScenarioCreationService : ScenarioCreationService
    {
        public FiveFiveOneKotHScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService) : base(scenarioMode, layoutService) { }

        public override void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.SetUpScenario(scenario, teamABases, teamBBases);

            scenario.CampaignID = "551";
            scenario.ScenarioDescription = "Ringtail's 5-5-1";
            scenario.Conditionals = new ConditionalCollection();

            BaseInfo baseA = teamABases[0];
            BaseInfo baseB = teamBBases[0];

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(baseA.Layout ?? "551", baseA.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(baseB.Layout ?? "551", baseB.Prefab.Prefab);

            IEnumerable<MultiplayerSpawn> spawns = scenario.Units!
                .Select(u => (u as MultiplayerSpawn)!)
                .Where(u => u != null);

            IEnumerable<IUnitSpawner> teamASpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Allied"));
            IEnumerable<IUnitSpawner> teamBSpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Enemy"));

            //////////////////////////////////////////////////////////////
            // Global Values
            //////////////////////////////////////////////////////////////
            scenario.GlobalValues = new();

            GlobalValue teamATime = scenario.GlobalValues.CreateGlobalValue("teamATime");
            GlobalValue teamBTime = scenario.GlobalValues.CreateGlobalValue("teamBTime");
            GlobalValue targetTime = scenario.GlobalValues.CreateGlobalValue("targetTime", 300);

            //////////////////////////////////////////////////////////////
            // Waypoints
            //////////////////////////////////////////////////////////////
            scenario.Waypoints = new WaypointCollection();

            Waypoint bullseye = scenario.Waypoints.CreateWaypoint("Bullseye", (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            scenario.AlliedRTB = scenario.Waypoints.CreateWaypoint("Team A RTB", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA));
            scenario.EnemyRTB = scenario.Waypoints.CreateWaypoint("Team B RTB", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB));

            //////////////////////////////////////////////////////////////
            // Objectives
            //////////////////////////////////////////////////////////////
            int objectiveCount = 0;
            scenario.AlliedObjectives = [CreateObjectiveForWin(objectiveCount++, 0, bullseye)];
            scenario.EnemyObjectives = [CreateObjectiveForWin(objectiveCount++, 0, bullseye)];

            //////////////////////////////////////////////////////////////
            // Conditional Actions
            //////////////////////////////////////////////////////////////

            scenario.ConditionalActions = new();
            ConditionalAction KothPoints = scenario.ConditionalActions.CreateConditionalAction(
                "KotH Points",
                () => SCCUnitList.SCC_NumNearWP(teamASpawns, bullseye, 5000) > 0 && SCCUnitList.SCC_NumNearWP(teamBSpawns, bullseye, 5000) > 0,
                [/* doNothing */],
                [() => SCCUnitList.SCC_NumNearWP(teamASpawns, bullseye, 5000) > 0, /*      */ () => SCCUnitList.SCC_NumNearWP(teamBSpawns, bullseye, 5000) > 0],
                [[new EventTarget("", () => VT.Methods.System.IncrementValue(teamATime, 1))], [new EventTarget("", () => VT.Methods.System.IncrementValue(teamBTime, 1))]],
                [/* doNothing */]);

            //////////////////////////////////////////////////////////////
            // Sequences
            //////////////////////////////////////////////////////////////
            scenario.EventSequences = new SequenceCollection();


            Conditional whenTeamAStartsLiving = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamASpawns) > 0);
            Conditional whenTeamAIsDead = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamASpawns) == 0);

            Event_Sequences teamADead = scenario.EventSequences.CreateSequence("Team A Dead?");
            teamADead.Events =
            [
                new Event("Wait for spawn", TimeSpan.FromSeconds(5), whenTeamAStartsLiving),
                new Event("Show text on death", TimeSpan.Zero, whenTeamAIsDead, [new EventTarget("Display Message", () => VT.Methods.System.DisplayMessage("Team B Victory!///nPull chutes", 10))]),
                new Event("Reset Sequence", TimeSpan.FromMinutes(4), null, [new EventTarget("Restart", () => teamADead.Restart())]),
            ];

            Conditional whenTeamBStartsLiving = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamBSpawns) > 0);
            Conditional whenTeamBIsDead = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamBSpawns) == 0);

            Event_Sequences teamBDead = scenario.EventSequences.CreateSequence("Team B Dead?");
            teamBDead.Events =
            [
                new Event("Wait for spawn", TimeSpan.FromSeconds(5), whenTeamBStartsLiving),
                new Event("Show text on death", TimeSpan.Zero, whenTeamBIsDead, [new EventTarget("Display Message", () => VT.Methods.System.DisplayMessage("Team A Victory!///nPull chutes", 10))]),
                new Event("Reset Sequence", TimeSpan.FromMinutes(4), null, [new EventTarget("Restart", () => teamBDead.Restart())]),
            ];

            Event_Sequences kothCheck = scenario.EventSequences.CreateSequence("KotH check", false);
            kothCheck.Events =
            [
                new Event("Check", TimeSpan.FromSeconds(0), null, [new EventTarget("Check control", () => VT.Methods.System.FireConditionalAction(KothPoints))]),
                new Event("Reset", TimeSpan.FromSeconds(1), null, [new EventTarget("Restart", () => kothCheck.Restart())]),
            ];

            //////////////////////////////////////////////////////////////
            // Trigger Events
            //////////////////////////////////////////////////////////////
            scenario.TriggerEvents = new();
            scenario.TriggerEvents.CreateConditionalTriggerEvent(
                "Someone takes off",
                true,
                scenario.Conditionals.CreateCondition(() => SCCUnitGroup.NumAirborne("Allied:Foxtrot") > 0),
                [new EventTarget("enable KotH", () => kothCheck.Begin())]
                );

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

        private static Objective CreateObjectiveForWin(int objectiveId, int orderId, Waypoint waypoint)
        {
            return new Objective()
            {
                ObjectiveName = "Win",
                ObjectiveInfo = "Win",
                ObjectiveID = objectiveId,
                OrderID = orderId,
                Required = true,
                Waypoint = waypoint,
                AutoSetWaypoint = false,
                StartMode = ObjectiveStartMode.Immediate,
                ObjectiveType = ObjectiveType.Conditional,
            };
        }
    }
}
