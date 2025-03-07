using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using Microsoft.Extensions.DependencyInjection;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;
using VtolVrRankedMissionSetup.VTS.Components;
using System.Linq;

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

            Conditional teamAStartsLiving = scenario.Conditionals.CreateCondition();
            teamAStartsLiving.Components = [CreateConditionalForLiving(teamASpawns)];
            teamAStartsLiving.rootComponent = teamAStartsLiving.Components[0];

            Conditional teamAIsDead = scenario.Conditionals.CreateCondition();
            teamAIsDead.Components = [CreateConditionalForDead(teamASpawns)];
            teamAIsDead.rootComponent = teamAIsDead.Components[0];

            Conditional teamBStartsLiving = scenario.Conditionals.CreateCondition();
            teamBStartsLiving.Components = [CreateConditionalForLiving(teamBSpawns)];
            teamBStartsLiving.rootComponent = teamBStartsLiving.Components[0];

            Conditional teamBIsDead = scenario.Conditionals.CreateCondition();
            teamBIsDead.Components = [CreateConditionalForDead(teamBSpawns)];
            teamBIsDead.rootComponent = teamBIsDead.Components[0];

            scenario.EventSequences = new SequenceCollection();

            Sequence teamADead = scenario.EventSequences.CreateSequence();
            teamADead.SequenceName = "Team A Dead?";
            teamADead.StartImmediately = true;
            teamADead.Events =
            [
                CreateEventForAlive(teamAStartsLiving),
                CreateEventForDead(teamAIsDead, "Team B Victory!///nPull chutes"),
                CreateEventForReset(),
            ];

            Sequence teamBDead = scenario.EventSequences.CreateSequence();
            teamBDead.SequenceName = "Team B Dead?";
            teamBDead.StartImmediately = true;
            teamBDead.Events =
            [
                CreateEventForAlive(teamBStartsLiving),
                CreateEventForDead(teamBIsDead, "Team A Victory!///nPull chutes"),
                CreateEventForReset(),
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

        private static IComponent CreateConditionalForLiving(IEnumerable<MultiplayerSpawn> mpSpawns)
        {
            return new SCCUnitListComponent()
            { 
                Id = 0,
                MethodName = "SCC_NumAlive",
                IsNot = false,
                UnitList = mpSpawns.ToArray(),
                MethodParameters = [
                    new MethodParameter("Greater_Than"),
                    new MethodParameter("0"),
                ]
            };
        }

        private static IComponent CreateConditionalForDead(IEnumerable<MultiplayerSpawn> mpSpawns)
        {
            return new SCCUnitListComponent()
            {
                Id = 0,
                MethodName = "SCC_NumAlive",
                IsNot = false,
                UnitList = mpSpawns.ToArray(),
                MethodParameters = [
                    new MethodParameter("Equals"),
                    new MethodParameter("0"),
                ]
            };
        }

        private static Event CreateEventForAlive(Conditional conditional)
        {
            return new Event()
            {
                Conditional = conditional,
                Delay = 5,
                NodeName = "Wait For Spawn",
                EventInfo = new(),
            };
        }

        private static Event CreateEventForDead(Conditional conditional, string victoryText)
        {
            return new Event()
            {
                Conditional = conditional,
                Delay = 0,
                NodeName = "Show Text on death",
                EventInfo = new()
                {
                    EventTargets = 
                    [
                        new EventTarget()
                        {
                            TargetType = "System",
                            TargetID = 0,
                            EventName = "Display Message",
                            MethodName = "DisplayMessage",
                            AltTargetIdx = -1,
                            Params = 
                            [
                                new ParamInfo()
                                {
                                    Type = "System.String",
                                    Value = victoryText,
                                    Name = "Text",
                                    Attrs = 
                                    [
                                        new ParamAttrInfo()
                                        {
                                            Type = "TextInputModes",
                                            Data = "MultiLine",
                                        },
                                        new ParamAttrInfo()
                                        {
                                            Type = "System.Int32",
                                            Data = "140",
                                        },
                                    ],
                                },
                                new ParamInfo()
                                {
                                    Type = "System.Single",
                                    Value = "10",
                                    Name = "Duration",
                                    Attrs =
                                    [
                                        new ParamAttrInfo()
                                        {
                                            Type = "MinMax",
                                            Data = "(1,9999)"
                                        },
                                    ],
                                },
                            ],
                        },
                    ],
                },
            };
        }

        private static Event CreateEventForReset()
        {
            return new Event()
            {
                Delay = 240,
                NodeName = "Reset Sequence",
                EventInfo = new EventInfo()
                {
                    EventTargets = 
                    [
                        new EventTarget()
                        {
                            TargetType = "Event_Sequences",
                            TargetID = 0,
                            EventName = "Restart",
                            MethodName = "Restart",
                            AltTargetIdx = -1,
                        },
                    ],
                },
            };
        }
    }
}
