using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            DataContext = ViewModel;

            ApplicationWindow.ViewModel.ApplicationAddedNotifier += AddAppButton;
            ApplicationWindow.ViewModel.ApplicationRemovedNotifier += RemoveAppButton;
            ApplicationWindow.PinToTaskbarClickNotifier += PinToTaskbarClickHandler;
        }

        public void InitializeButtons(TaskbarPosition taskbarPosition, TaskbarIconsSize taskbarIconsSize)
        {
            CurrentIconSize = taskbarIconsSize;

            foreach (var applicationViewModel in ViewModel.PinnedApps)
            {
                AddButton(applicationViewModel);
            }

            SetMainButtonIconSize(taskbarIconsSize);
            PositionChanged(taskbarPosition);
        }

        private void PinToTaskbarClickHandler(object sender, EventArgs eventArgs)
        {
            var clickedApp = Helper.GetApplicationViewModelFromContextMenuClick(sender);

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

        private void AddButton(ApplicationViewModel application)
        {
            var taskbarButton = new TaskbarButton(application, CurrentIconSize);

            taskbarButton.PinToTaskbarClickNotifier += PinToTaskbarClickHandler;
            taskbarButton.UninstallClickNotifier += ApplicationWindow.UninstallAppClick;

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

        public int GetCurrentDimension()
        {
            double totalSize = DefaultPanelMargins;
            var isHorizontal = ButtonContainer.Orientation == Orientation.Horizontal;

            totalSize += isHorizontal ? MainButton.Width : MainButton.Height;

            foreach (var taskbarButton in ButtonContainer.Children.OfType<TaskbarButton>())
            {
                totalSize += isHorizontal ? taskbarButton.Width : taskbarButton.Height;
            }

            Width = isHorizontal ? totalSize : MainButton.Height;
            Height = isHorizontal ? MainButton.Height : totalSize;

            return (int)totalSize;
        }

        private void RemoveButton(ApplicationViewModel applicationViewModel)
        {
            var btn = ButtonContainer.Children.OfType<TaskbarButton>().FirstOrDefault(b => b.Id == applicationViewModel.Application.Id);

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
            var application = sender as ApplicationViewModel;
            ViewModel.RemovePinnedApp(application);
            RemoveButton(application);
        }

        private void AddAppButton(object sender, EventArgs e)
        {
            var applicationViewModel = sender as ApplicationViewModel;
            ViewModel.AddPinnedApp(applicationViewModel);
            AddButton(applicationViewModel);
        }

        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationWindow.Visibility != Visibility.Visible || !ApplicationWindow.Topmost)
            {
                ApplicationWindow.SetPosition();
                ApplicationWindow.Show();
                ApplicationWindow.Topmost = true;
            }
            else
            {
                ApplicationWindow.Hide();
            }
        }

        public void HeightChanged(int newHeight)
        {
            //Nothing to do as long as the window gets moved correctly
        }

        public void PositionChanged(TaskbarPosition newPosition)
        {
            bool isVertical = newPosition.IsVertical();

            if (isVertical == (ButtonContainer.Orientation == Orientation.Vertical))
            {
                return;
            }

            if (isVertical && ButtonContainer.Orientation != Orientation.Vertical)
            {
                ButtonContainer.Orientation = Orientation.Vertical;
            }
            else
            {
                ButtonContainer.Orientation = Orientation.Horizontal;
            }

            var widthTemp = Width;
            Width = Height;
            Height = widthTemp;

            var marginHorizontal = MainButton.Margin.Left;
            var marginVertical = MainButton.Margin.Top;

            MainButton.Margin = new Thickness(marginVertical, marginHorizontal, marginVertical, marginHorizontal);
            NotifyTaskbarOfChange();
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
            ShutdownHelper.Instance.Shutdown();
        }
    }
}
