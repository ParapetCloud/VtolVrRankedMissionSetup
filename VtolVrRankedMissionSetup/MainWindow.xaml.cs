using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VtolVrRankedMissionSetup
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        nint Hwnd;
        StorageFile? MapFile;
        VTMapCustom? Map { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        }

        private async void OnOpenClicked(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpen = new();

            WinRT.Interop.InitializeWithWindow.Initialize(fileOpen, Hwnd);
            fileOpen.ViewMode = PickerViewMode.List;
            fileOpen.FileTypeFilter.Add(".vtm");

            StorageFile file = await fileOpen.PickSingleFileAsync();

            if (file == null)
                return;

            MapFile = file;
            Map = VTSerializer.DeserializeFromFile<VTMapCustom>(file.Path);


        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
