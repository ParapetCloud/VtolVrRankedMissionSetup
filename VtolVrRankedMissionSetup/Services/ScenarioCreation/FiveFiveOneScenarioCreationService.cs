﻿using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using Microsoft.Extensions.DependencyInjection;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;
using VtolVrRankedMissionSetup.VTS.Components;
using System.Linq;
using VtolVrRankedMissionSetup.VT.Methods;
using System;
using VtolVrRankedMissionSetup.VTS.Events;

namespace VtolVrRankedMissionSetup.Services.ScenarioCreation
{
    [Service(ServiceLifetime.Singleton)]
    public class FiveFiveOneScenarioCreationService : ScenarioCreationService
    {
        public FiveFiveOneScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService) : base(scenarioMode, layoutService) { }

        public override void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            base.SetUpScenario(scenario, teamABases, teamBBases);

            scenario.CampaignID = "551";
            scenario.ScenarioDescription = "Ringtail's 5-5-1";

            BaseInfo baseA = teamABases[0];
            BaseInfo baseB = teamBBases[0];

            AirbaseLayoutConfig airbaseAConfig = layoutService.GetConfig(baseA.Layout ?? "551", baseA.Prefab.Prefab);
            AirbaseLayoutConfig airbaseBConfig = layoutService.GetConfig(baseB.Layout ?? "551", baseB.Prefab.Prefab);

            // waypoints
            scenario.Waypoints = new WaypointCollection();

            Waypoint bullseye = scenario.Waypoints.CreateWaypoint("Bullseye", (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            scenario.AlliedRTB = scenario.Waypoints.CreateWaypoint("Team A RTB", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA));
            scenario.EnemyRTB = scenario.Waypoints.CreateWaypoint("Team B RTB", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB));

            // objectives
            int objectiveCount = 0;
            scenario.AlliedObjectives = [CreateObjectiveForWin(objectiveCount++, 0, bullseye)];
            scenario.EnemyObjectives = [CreateObjectiveForWin(objectiveCount++, 0, bullseye)];

            // sequences
            scenario.Conditionals = new ConditionalCollection();
            scenario.EventSequences = new SequenceCollection();

            IEnumerable<MultiplayerSpawn> spawns = scenario.Units!
                .Select(u => (u as MultiplayerSpawn)!)
                .Where(u => u != null);

            IEnumerable<IUnitSpawner> teamASpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Allied"));

            Conditional whenTeamAStartsLiving = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamASpawns) > 0);
            Conditional whenTeamAIsDead = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamASpawns) == 0);

            Event_Sequences teamADead = scenario.EventSequences.CreateSequence("Team A Dead?");
            teamADead.Events =
            [
                new Event("Wait for spawn", TimeSpan.FromSeconds(5), whenTeamAStartsLiving),
                new Event("Show text on death", TimeSpan.Zero, whenTeamAIsDead, [new EventTarget("Display Message", () => VT.Methods.System.DisplayMessage("Team B Victory!///nPull chutes", 10))]),
                new Event("Reset Sequence", TimeSpan.FromMinutes(4), null, [new EventTarget("Restart", () => teamADead.Restart())]),
            ];

            IEnumerable<IUnitSpawner> teamBSpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Enemy"));

            Conditional whenTeamBStartsLiving = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamBSpawns) > 0);
            Conditional whenTeamBIsDead = scenario.Conditionals.CreateCondition(() => SCCUnitList.SCC_NumAlive(teamBSpawns) == 0);

            Event_Sequences teamBDead = scenario.EventSequences.CreateSequence("Team B Dead?");
            teamBDead.Events =
            [
                new Event("Wait for spawn", TimeSpan.FromSeconds(5), whenTeamBStartsLiving),
                new Event("Show text on death", TimeSpan.Zero, whenTeamBIsDead, [new EventTarget("Display Message", () => VT.Methods.System.DisplayMessage("Team A Victory!///nPull chutes", 10))]),
                new Event("Reset Sequence", TimeSpan.FromMinutes(4), null, [new EventTarget("Restart", () => teamBDead.Restart())]),
            ];
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
