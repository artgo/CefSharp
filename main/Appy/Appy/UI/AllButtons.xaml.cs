using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.InteropAPI;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for AllButtons.xaml
    /// </summary>
    public partial class AllButtons : ITaskbarInterop   
    {
        private MainWindow _applicationWindow;
        public TaskbarViewModel ViewModel { get; set; }

        private MainWindow ApplicationWindow
        {
            get
            {
                if (_applicationWindow == null)
                {
                    _applicationWindow = new MainWindow();
                }
                return _applicationWindow;
            }
            set { _applicationWindow = value; }
        }

        public AllButtons()
        {
            InitializeComponent();
            ViewModel = new TaskbarViewModel();

            Left = SystemParameters.WorkArea.Right * .5;
            Top = SystemParameters.WorkArea.Bottom - Height;

            foreach (var application in ViewModel.PinnedApps)
            {
                AddButton(application);
            }

            ApplicationWindow.ApplicationAddedNotifier += AddAppButton;
            ApplicationWindow.ApplicationRemovedNotifier += RemoveAppButton;

            ApplicationWindow.PinToTaskbarClickNotifier += PinToTaskbarClickHandler;
        }

        private void PinToTaskbarClickHandler(object sender, EventArgs eventArgs)
        {
            var clickedApp = Helper.GetClickedAppFromContextMenuClick(sender);

            var clickedItem = (MenuItem)sender;

            //Item is checked at this point if when it was clicked it was NOT checked
            if (clickedItem.IsChecked)
            {
                ViewModel.AddPinnedApp(clickedApp);
                AddButton(clickedApp);
            }
            else
            {
                ViewModel.RemovePinnedApp(clickedApp);
                RemoveButton(clickedApp);
            }
        }

        private void AddButton(Application application)
        {
            ButtonContainer.Children.Add(new TaskbarButton(application));
            Width += 40;
        }

        private void RemoveButton(Application application)
        {
            var btn = ButtonContainer.Children.OfType<TaskbarButton>().FirstOrDefault(b => b.Name == application.Id);

            if (btn != null)
            {
                ButtonContainer.Children.Remove(btn);
                Width -= 40;
            }
        }

        private void RemoveAppButton(object sender, EventArgs e)
        {
            var application = sender as Application;
            ViewModel.RemovePinnedApp(application);
            RemoveButton(application);
        }

        private void AddAppButton(object sender, EventArgs e)
        {
            var application = sender as Application;
            ViewModel.AddPinnedApp(application);
            AddButton(application);
        }

        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationWindow.Visibility == Visibility.Visible)
            {
                ApplicationWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                ApplicationWindow.Show();
            }
        }

        public void HeightChanged(int newHeight)
        {
            throw new NotImplementedException();
        }

        public void PositionChanged(TaskbarPosition newPosition)
        {
            if (newPosition == TaskbarPosition.Bottom || newPosition == TaskbarPosition.Top)
            {
                ButtonContainer.Orientation = Orientation.Horizontal;
            }
            else
            {
                ButtonContainer.Orientation = Orientation.Vertical;
            }
        }

        public ITaskbarInteropCallback TaskbarCallbackEvents { get; set; }
    }
}
