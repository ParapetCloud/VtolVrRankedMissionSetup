using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using Microsoft.Extensions.DependencyInjection;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;
using VtolVrRankedMissionSetup.VTS.Components;
using System.Linq;
using VtolVrRankedMissionSetup.VT.Methods;

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

            int objectiveCount = 0;

            scenario.Waypoints = new WaypointCollection();

            Waypoint bullseye = scenario.Waypoints.CreateWaypoint("Bullseye", (baseA.Prefab.GlobalPos + baseB.Prefab.GlobalPos) / 2.0f);
            scenario.Waypoints.Bullseye = bullseye;
            scenario.Waypoints.BullseyeB = bullseye;

            scenario.AlliedRTB = scenario.Waypoints.CreateWaypoint("Team A RTB", MathHelpers.BaseToWorld(airbaseAConfig.Waypoints.Rtb, baseA));
            scenario.EnemyRTB = scenario.Waypoints.CreateWaypoint("Team B RTB", MathHelpers.BaseToWorld(airbaseBConfig.Waypoints.Rtb, baseB));

            scenario.AlliedObjectives = [CreateObjectiveForWin(objectiveCount++, 0, bullseye)];
            scenario.EnemyObjectives = [CreateObjectiveForWin(objectiveCount++, 0, bullseye)];

            IEnumerable<MultiplayerSpawn> spawns = scenario.Units!
                .Select(u => (u as MultiplayerSpawn)!)
                .Where(u => u != null);

            IEnumerable<MultiplayerSpawn> teamASpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Allied"));
            IEnumerable<MultiplayerSpawn> teamBSpawns = spawns.Where(mp => mp.MultiplayerSpawnFields.UnitGroup.StartsWith("Enemy"));

            scenario.Conditionals = new ConditionalCollection();

            Conditional whenTeamAStartsLiving = scenario.Conditionals.CreateCondition(CreateConditionalComponentForLiving(teamASpawns));
            Conditional whenTeamAIsDead = scenario.Conditionals.CreateCondition(CreateConditionalComponentForDead(teamASpawns));

            Conditional whenTeamBStartsLiving = scenario.Conditionals.CreateCondition(CreateConditionalComponentForLiving(teamBSpawns));
            Conditional whenTeamBIsDead = scenario.Conditionals.CreateCondition(CreateConditionalComponentForDead(teamBSpawns));

            scenario.EventSequences = new SequenceCollection();

            Sequence teamADead = scenario.EventSequences.CreateSequence("Team A Dead?");
            teamADead.Events =
            [
                new Event("Wait for spawn", 5, whenTeamAStartsLiving),
                new Event("Show text on death", 0, whenTeamAIsDead, [new EventTarget("Display Message", () => VT.Methods.System.DisplayMessage("Team B Victory!///nPull chutes", 10))]),
                new Event("Reset Sequence", 240, null, [new EventTarget("Restart", () => Event_Sequences.Restart())]),
            ];

            Sequence teamBDead = scenario.EventSequences.CreateSequence("Team B Dead?");
            teamBDead.Events =
            [
                new Event("Wait for spawn", 5, whenTeamBStartsLiving),
                new Event("Show text on death", 0, whenTeamBIsDead, [new EventTarget("Display Message", () => VT.Methods.System.DisplayMessage("Team A Victory!///nPull chutes", 10))]),
                new Event("Reset Sequence", 240, null, [new EventTarget("Restart", () => Event_Sequences.Restart())]),
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

        private static IComponent CreateConditionalComponentForLiving(IEnumerable<MultiplayerSpawn> mpSpawns)
        {
            return new SCCUnitListComponent()
            { 
                MethodName = "SCC_NumAlive",
                IsNot = false,
                UnitList = mpSpawns.ToArray(),
                MethodParameters = [
                    new MethodParameter("Greater_Than"),
                    new MethodParameter("0"),
                ]
            };
        }

        private static IComponent CreateConditionalComponentForDead(IEnumerable<MultiplayerSpawn> mpSpawns)
        {
            return new SCCUnitListComponent()
            {
                MethodName = "SCC_NumAlive",
                IsNot = false,
                UnitList = mpSpawns.ToArray(),
                MethodParameters = [
                    new MethodParameter("Equals"),
                    new MethodParameter("0"),
                ]
            };
        }
    }
}
