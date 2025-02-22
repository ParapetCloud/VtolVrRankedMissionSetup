using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using VtolVrRankedMissionSetup.Step;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;
using VtolVrRankedMissionSetup.VTS;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VtolVrRankedMissionSetup
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public VTMapCustom? Map { get; set; }
        public CustomScenario? Scenario { get; set; }
        public ObservableCollection<BaseInfo> TeamABases { get; set; }
        public ObservableCollection<BaseInfo> TeamBBases { get; set; }

        private nint Hwnd;
        private PlaceEverythingStep placeEverything;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            Hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            placeEverything = new();
            TeamABases = new();
            TeamBBases = new();

            InitializeComponent();
        }

        private async void OnOpenClicked(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpen = new();

            WinRT.Interop.InitializeWithWindow.Initialize(fileOpen, Hwnd);
            fileOpen.ViewMode = PickerViewMode.List;
            fileOpen.FileTypeFilter.Add(".vtm");

            StorageFile file = await fileOpen.PickSingleFileAsync();

            if (file == null)
            {
                ShowDialog("Can't load map", "You didn't pick one");
                return;
            }

            Map = VTSerializer.DeserializeFromFile<VTMapCustom>(file.Path);

            if (Map.StaticPrefabs == null)
            {
                ShowDialog("Can't load map", $"Map '{Map.MapID}' does not have any bases");
                return;
            }

            if (Map.StaticPrefabs.Count(prefab => prefab.Prefab.StartsWith("airbase")) < 2)
            {
                ShowDialog("Can't load map", $"Map '{Map.MapID}' does not have enough bases");
                return;
            }

            Scenario = new CustomScenario(Map)
            {
                ScenarioName = $"BVR {Map.MapID}",
                ScenarioID = $"BVR {Map.MapID}",
                CampaignOrderIdx = 0,
            };

            TeamABases.Clear();
            TeamBBases.Clear();

            foreach (BaseInfo baseInfo in Scenario.Bases)
            {
                TeamABases.Add(baseInfo);
            }
        }

        private async void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            if (Scenario == null)
            {
                ShowDialog("Couldn't save scenario", "You must open a map first");
                return;
            }

            BaseInfo? teamABase = TeamABases.FirstOrDefault();

            if (teamABase == null)
            {
                ShowDialog("Couldn't save scenario", "Allied Team doesn't have a base");
                return;
            }

            BaseInfo? teamBBase = TeamBBases.FirstOrDefault();

            if (teamBBase == null)
            {
                ShowDialog("Couldn't save scenario", "Enemy Team doesn't have a base");
                return;
            }

            placeEverything.Start(Scenario, teamABase, teamBBase);

            FileSavePicker fileSave = new();

            WinRT.Interop.InitializeWithWindow.Initialize(fileSave, Hwnd);
            fileSave.FileTypeChoices.Add("VTOL Scenario", [".vts"]);

            StorageFile file = await fileSave.PickSaveFileAsync();

            if (file == null)
                return;

            string name = file.DisplayName;

            Scenario.ScenarioName = name;
            Scenario.ScenarioID = name;

            VTSerializer.SerializeToFile(Scenario, file.Path);
        }

        private void ShowDialog(string title, string content)
        {
            ContentDialog d = new()
            {
                XamlRoot = MainGrid.XamlRoot,
                Title = title,
                Content = content,
                CloseButtonText = "OK",
            };

            _ = d.ShowAsync();
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null, bool forceUpdate = false)
        {
            if (!forceUpdate && Equals(storage, value))
                return false;

            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        private async void TeamADragDrop(object sender, DragEventArgs e)
        {
            uint id = (uint)await e.DataView.GetDataAsync("vtbase");

            BaseInfo baseInfo = TeamBBases.First(b => b.Prefab.Id == id);

            baseInfo.BaseTeam = Team.Allied;

            TeamBBases.Remove(baseInfo);
            TeamABases.Add(baseInfo);
        }

        private async void TeamBDragDrop(object sender, DragEventArgs e)
        {
            uint id = (uint)await e.DataView.GetDataAsync("vtbase");

            BaseInfo baseInfo = TeamABases.First(b => b.Prefab.Id == id);

            baseInfo.BaseTeam = Team.Enemy;

            TeamABases.Remove(baseInfo);
            TeamBBases.Add(baseInfo);
        }

        private void TeamListDragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = e.DataView.Contains("vtbase") ?
                Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move :
                Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
        }

        private void BaseInfoDragStarting(object sender, DragItemsStartingEventArgs e)
        {
            e.Data.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            e.Data.SetData("vtbase", ((BaseInfo)e.Items[0]).Prefab.Id);
        }
    }
}
