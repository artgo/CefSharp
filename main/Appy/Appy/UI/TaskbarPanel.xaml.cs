using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for AllButtons.xaml
    /// </summary>
    public partial class TaskbarPanel : ITaskbarInterop   
    {
        public TaskbarPanelViewModel ViewModel { get; set; }
        public const int DeskbandInitialSize = 40;
        public const int DefaultPanelMargins = 20;

        public TaskbarIconsSize CurrentIconSize { get; set; }
        
        private const int MainIconLargeSize = 30;
        private const int MainIconSmallSize = 20;
        
        public MainWindow ApplicationWindow { get; set; }

        public TaskbarPanel(MainWindow mainView)
        {
            InitializeComponent();

            ApplicationWindow = mainView;
            
            ViewModel = new TaskbarPanelViewModel();

            ApplicationWindow.ViewModel.ApplicationAddedNotifier += AddAppButton;
            ApplicationWindow.ViewModel.ApplicationRemovedNotifier += RemoveAppButton;
            ApplicationWindow.PinToTaskbarClickNotifier += PinToTaskbarClickHandler;
        }

        public void InitializeButtons(TaskbarPosition taskbarPosition, TaskbarIconsSize taskbarIconsSize)
        {
            CurrentIconSize = taskbarIconsSize;

            foreach (var application in ViewModel.PinnedApps)
            {
                AddButton(application);
            }

            SetMainButtonIconSize(taskbarIconsSize);
            PositionChanged(taskbarPosition);
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
            var taskbarButton = new TaskbarButton(application, CurrentIconSize);

            ButtonContainer.Children.Add(taskbarButton);

            if (ButtonContainer.Orientation == Orientation.Horizontal)
            {
                Width += taskbarButton.Width;
            }
            else
            {
                Height += taskbarButton.Height;
            }

            NotifyTaskbarOfChange();
        }

        private void NotifyTaskbarOfChange()
        {
            if (TaskbarCallbackEvents != null)
            {
                TaskbarCallbackEvents.ChangeWidth(GetCurrentDimension());
            }
        }

        private int GetCurrentDimension()
        {
            double totalSize = DefaultPanelMargins;
            var isHorizontal = ButtonContainer.Orientation == Orientation.Horizontal;

            totalSize += isHorizontal ? MainButton.Width : MainButton.Height;

            foreach (var taskbarButton in ButtonContainer.Children.OfType<TaskbarButton>())
            {
                totalSize += isHorizontal ? taskbarButton.Width : MainButton.Height;
            }
            return (int) totalSize;
        }

        private void RemoveButton(Application application)
        {
            var btn = ButtonContainer.Children.OfType<TaskbarButton>().FirstOrDefault(b => b.Id == application.Id);

            if (btn != null)
            {
                ButtonContainer.Children.Remove(btn);
                if (ButtonContainer.Orientation == Orientation.Horizontal)
                {
                    Width -= btn.Width;
                }
                else
                {
                    Height -= btn.Height;
                }
                
                NotifyTaskbarOfChange();
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
            if (ApplicationWindow.Visibility != Visibility.Visible || !ApplicationWindow.Topmost)
            {
                ApplicationWindow.Show();
                ApplicationWindow.Topmost = true;
            }
        }

        public void HeightChanged(int newHeight)
        {
            //Nothing to do as long as the window gets moved correctly
        }

        public void PositionChanged(TaskbarPosition newPosition)
        {
            if (newPosition.IsVertical() && ButtonContainer.Orientation != Orientation.Vertical)
            {
                ButtonContainer.Orientation = Orientation.Vertical;
                var widthTemp = Width;
                Width = Height;
                Height = widthTemp;
            }
            else if (!newPosition.IsVertical() && ButtonContainer.Orientation != Orientation.Horizontal)
            {
                ButtonContainer.Orientation = Orientation.Horizontal;
                var widthTemp = Width;
                Width = Height;
                Height = widthTemp;
            }
        }

        public void TaskbarIconsSizeChanged(TaskbarIconsSize newIconsSize)
        {
            CurrentIconSize = newIconsSize;

            Helper.PerformInUiThread(() =>
                {
                    foreach (var taskbarButton in ButtonContainer.Children.OfType<TaskbarButton>())
                    {
                        taskbarButton.ChangeIconSize(newIconsSize);
                    }

                    SetMainButtonIconSize(newIconsSize);

                    NotifyTaskbarOfChange();
                });
        }

        public void Shutdown()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Error(RegistryChangeEventArgs eventArgs)
        {
            throw eventArgs.Exception;
        }

        private void SetMainButtonIconSize(TaskbarIconsSize newIconsSize)
        {
            Helper.PerformInUiThread(() =>
                {
                    MainButton.Height = newIconsSize == TaskbarIconsSize.Small
                                            ? MainIconSmallSize
                                            : MainIconLargeSize;
                    MainButton.Width = newIconsSize == TaskbarIconsSize.Small
                                           ? MainIconSmallSize
                                           : MainIconLargeSize;
                });
        }

        public ITaskbarInteropCallback TaskbarCallbackEvents { get; set; }
        
        private void MenuItemExitClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
