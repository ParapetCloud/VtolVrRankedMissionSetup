using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.VTM;
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

                string? forceEquipment = scenarioMode.ActiveMode.ForcedEquipment?[vehicle];

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
            byte primaryColor = (byte)(primary ? 160 : 127);
            byte secondaryColor = (byte)(primary ? 80 : 0);

            return team == Team.Allied ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, secondaryColor, secondaryColor, primaryColor)) : new SolidColorBrush(Windows.UI.Color.FromArgb(255, primaryColor, secondaryColor, secondaryColor));
        }
    }
}
