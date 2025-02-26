using System;
using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.Services
{
    public class ScenarioCreationService
    {
        protected readonly ScenarioModeService scenarioMode;
        protected readonly AirbaseLayoutService layoutService;

        public ScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService)
        {
            this.scenarioMode = scenarioMode;
            this.layoutService = layoutService;
        }

        public virtual void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            List<IUnitSpawner> spawners = [];

            foreach (BaseInfo baseInfo in teamABases)
            {
                PopulateAirbase(baseInfo, spawners, Team.Allied);
            }

            foreach (BaseInfo baseInfo in teamBBases)
            {
                PopulateAirbase(baseInfo, spawners, Team.Enemy);
            }

            scenario.Units = spawners.ToArray();
        }

        protected virtual void PopulateAirbase(BaseInfo baseInfo, List<IUnitSpawner> spawners, Team team)
        {
            if (baseInfo.Layout == null)
                return;

            AirbaseLayoutConfig layoutConfig = layoutService.GetConfig(baseInfo.Layout, baseInfo.Prefab.Prefab);

            AddAircraftToBase(baseInfo, layoutConfig.F26, "F/A-26B", spawners, team == Team.Allied ? "Allied:Alpha" : "Enemy:Zulu");
            AddAircraftToBase(baseInfo, layoutConfig.F45, "F-45A", spawners, team == Team.Allied ? "Allied:Bravo" : "Enemy:Yankee");
            AddAircraftToBase(baseInfo, layoutConfig.F24, "EF-24G", spawners, team == Team.Allied ? "Allied:Charlie" : "Enemy:Xray", 2);
            AddAircraftToBase(baseInfo, layoutConfig.T55, "T-55", spawners, team == Team.Allied ? "Allied:Delta" : "Enemy:Whiskey", 2);
        }

        protected virtual void AddAircraftToBase(BaseInfo baseInfo, AircraftConfig[]? aircrafts, string vehicle, List<IUnitSpawner> spawners, string unitGroup, int slots = 0)
        {
            if (aircrafts == null)
                return;

            string[] unitParts = unitGroup.Split(':');

            for (int i = 0; i < aircrafts.Length; ++i)
            {
                AircraftConfig aircraft = aircrafts[i];

                Vector3 location = MathHelpers.BaseToWorld(aircraft.Location, baseInfo);
                Vector3 rotation = baseInfo.Prefab.Rotation + aircraft.Rotation;
                MathHelpers.ClampRotation(ref rotation);

                MultiplayerSpawn spawn = new(Enum.Parse<Team>(unitParts[0]), $"{unitParts[1]} 1-{i + 1}")
                {
                    UnitInstanceID = spawners.Count,
                    GlobalPosition = location,
                    Rotation = rotation,
                };

                spawn.MultiplayerSpawnFields.UnitGroup = unitGroup;
                spawn.MultiplayerSpawnFields.Vehicle = vehicle;
                spawn.MultiplayerSpawnFields.Equipment = scenarioMode.ActiveMode.DefaultEquipment[vehicle];
                spawn.MultiplayerSpawnFields.Slots = slots;

                spawners.Add(spawn);
            }
        }
    }
}
