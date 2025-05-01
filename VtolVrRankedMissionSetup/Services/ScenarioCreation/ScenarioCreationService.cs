using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.Services
{
    public abstract class ScenarioCreationService
    {
        protected readonly ScenarioModeService scenarioMode;
        protected readonly AirbaseLayoutService layoutService;
        private readonly Dictionary<string, int> alliedGroupCounts;
        private readonly Dictionary<string, int> enemyGroupCounts;

        public ScenarioCreationService(ScenarioModeService scenarioMode, AirbaseLayoutService layoutService)
        {
            this.scenarioMode = scenarioMode;
            this.layoutService = layoutService;

            alliedGroupCounts = new();
            enemyGroupCounts = new();
        }

        public virtual void SetUpScenario(CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            List<IUnitSpawner> spawners = [];

            for (int i = 0; i < teamABases.Length; ++i)
            {
                BaseInfo bs = teamABases[i];
                PopulateAirbase(bs, spawners, Team.Allied, i == 0);
            }

            for (int i = 0; i < teamBBases.Length; ++i)
            {
                BaseInfo bs = teamBBases[i];
                PopulateAirbase(bs, spawners, Team.Enemy, i == 0);
            }

            scenario.Units = spawners.ToArray();
        }

        public virtual void GeneratePreview(Canvas canvas, VTMapCustom map, CustomScenario scenario, BaseInfo[] teamABases, BaseInfo[] teamBBases)
        {
            for (int i = 0; i < teamABases.Length; ++i)
            {
                BaseInfo bs = teamABases[i];
                PreviewBase(canvas, bs, map, Team.Allied, i == 0);
            }

            for (int i = 0; i < teamBBases.Length; ++i)
            {
                BaseInfo bs = teamBBases[i];
                PreviewBase(canvas, bs, map, Team.Enemy, i == 0);
            }
        }

        protected virtual void PreviewBase(Canvas canvas, BaseInfo bs, VTMapCustom map, Team team, bool primary)
        {
            SolidColorBrush teamColor = GetTeamColor(team, primary);

            Rectangle baseRepresentation = new()
            {
                Width = 18,
                Height = 18,
                Fill = teamColor,
            };

            Vector2 baseMapLocation = worldToPreview(bs.Prefab.GlobalPos, map);
            Canvas.SetLeft(baseRepresentation, baseMapLocation.X - 9);
            Canvas.SetTop(baseRepresentation, baseMapLocation.Y - 9);

            canvas.Children.Add(baseRepresentation);

            TextBlock textBlock = new()
            {
                FontSize = 14,
                LineHeight = 18,
                Width = 18,
                Height = 18,
                TextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                Text = $"{bs.Prefab.Id}",
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 255, 255)),
            };

            Canvas.SetLeft(textBlock, baseMapLocation.X - 9);
            Canvas.SetTop(textBlock, baseMapLocation.Y - 9);

            canvas.Children.Add(textBlock);
        }

        protected virtual void PopulateAirbase(BaseInfo baseInfo, List<IUnitSpawner> spawners, Team team, bool primary)
        {
            string layout = GetLayout(baseInfo, primary);
            if (string.IsNullOrWhiteSpace(layout))
                return;

            AirbaseLayoutConfig layoutConfig = layoutService.GetConfig(layout, baseInfo.Prefab.Prefab);

            AddAircraftToBase(baseInfo, layoutConfig.Aircraft, spawners, team);
        }

        protected virtual void AddAircraftToBase(BaseInfo baseInfo, AircraftConfig[]? aircrafts, List<IUnitSpawner> spawners, Team team)
        {
            if (aircrafts == null)
                return;

            for (int i = 0; i < aircrafts.Length; ++i)
            {
                AircraftConfig aircraft = aircrafts[i];

                Vector3 location = MathHelpers.BaseToWorld(aircraft.Location, baseInfo);
                Vector3 rotation = baseInfo.Prefab.Rotation + aircraft.Rotation;
                MathHelpers.ClampRotation(ref rotation);

                string group = GetAircraftGroup(team, aircraft);

                MultiplayerSpawn spawn = new(team, $"{group} 1-{GetAndIncrement(team, group) + 1}")
                {
                    UnitInstanceID = spawners.Count,
                    GlobalPosition = location,
                    Rotation = rotation,
                };

                spawn.MultiplayerSpawnFields.UnitGroup = $"{team}:{group}";
                spawn.MultiplayerSpawnFields.Vehicle = aircraft.Spawns[0].Type;
                spawn.MultiplayerSpawnFields.Equipment = scenarioMode.ActiveMode.DefaultEquipment[aircraft.Spawns[0].Type];
                spawn.MultiplayerSpawnFields.Slots = aircraft.Spawns[0].Slots ?? 0;

                string? forceEquipment = scenarioMode.ActiveMode.ForcedEquipment?[aircraft.Spawns[0].Type];

                if (!string.IsNullOrWhiteSpace(forceEquipment))
                {
                    spawn.MultiplayerSpawnFields.ForcedEquipsList = forceEquipment;
                }

                spawners.Add(spawn);
            }
        }

        protected Vector2 worldToPreview(Vector3 world, VTMapCustom map)
        {
            float mapSize = (float)(map.MapSize * 3.0625 * Units.Kilometers);

            return new Vector2(
                (world.X / mapSize) * 640,
                640 - (world.Z / mapSize) * 640
                );
        }

        protected double worldToPreview(double world, VTMapCustom map)
        {
            double mapSize = (map.MapSize * 3.0625) * Units.Kilometers;

            return (world / mapSize) * 640;
        }

        protected virtual SolidColorBrush GetTeamColor(Team team, bool primary)
        {
            byte primaryColor = (byte)(primary ? 255 : 127);
            byte secondaryColor = (byte)(primary ? 80 : 0);

            return team == Team.Allied ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, secondaryColor, secondaryColor, primaryColor)) : new SolidColorBrush(Windows.UI.Color.FromArgb(255, primaryColor, secondaryColor, secondaryColor));
        }

        protected string GetLayout(BaseInfo baseInfo, bool primary) => baseInfo.Layout.ValueOrDefault(primary ? scenarioMode.ActiveMode.PrimaryDefaultLayout : scenarioMode.ActiveMode.SecondaryDefaultLayout!);

        protected abstract string GetAircraftGroup(Team team, AircraftConfig aircraft);

        private int GetAndIncrement(Team team, string group)
        {
            var counts = team == Team.Allied ? alliedGroupCounts : enemyGroupCounts;

            if (!counts.ContainsKey(group))
            {
                counts.Add(group, 1);
                return 0;
            }

            return counts[group]++;
        }
    }
}
