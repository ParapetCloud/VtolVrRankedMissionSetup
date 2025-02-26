using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using VtolVrRankedMissionSetup.VTS;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VtolVrRankedMissionSetup.Controls
{
    public sealed partial class TeamBaseList : UserControl
    {
        public static readonly DependencyProperty TeamProperty = DependencyProperty.Register(
            nameof(Side),
            typeof(Team),
            typeof(TeamBaseList),
            new PropertyMetadata(null));

        public Team Side
        {
            get => (Team)GetValue(TeamProperty);
            set
            {
                SetValue(TeamProperty, value);
                VisualStateManager.GoToState(this, value.ToString(), false);
            }
        }

        public new event EventHandler<DragEventArgs>? Drop;

        public ObservableCollection<BaseInfo> Bases { get; set; }

        public TeamBaseList()
        {
            Bases = new();

            InitializeComponent();

        }

        private void TeamListDrop(object sender, DragEventArgs e)
        {
            Drop?.Invoke(this, e);
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
