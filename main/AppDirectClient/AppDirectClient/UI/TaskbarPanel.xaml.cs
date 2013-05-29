using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Common.Log;
using AppDirect.WindowsClient.InteropAPI;
using AppDirect.WindowsClient.InteropAPI.Internal;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace AppDirect.WindowsClient.UI
{
    /// <summary>
    /// Interaction logic for AllButtons.xaml
    /// </summary>
    public partial class TaskbarPanel : ITaskbarControl
    {
        private const int MainIconLargeSize = 30;
        private const int MainIconSmallSize = 20;
        private readonly ILogger _log;
        private readonly MainViewModel _mainViewModel;

        public const int DeskbandInitialSize = 40;
        public const int DefaultPanelMargins = 20;
        public ILatch InitializeMainWindowLatch;

        public volatile MainWindow ApplicationWindow;

        public TaskbarIconsSize CurrentIconSize { get; set; }

        public TaskbarPanelViewModel ViewModel { get; set; }

        private ITaskbarHost _taskBarHost;
        private int _allowedHeight;
        private int _allowedWidth;

        public TaskbarPanel(ILatch latch, ILogger logger, MainViewModel mainViewModel)
        {
            _log = logger;
            _mainViewModel = mainViewModel;
            InitializeComponent();
            InitializeMainWindowLatch = latch;

            ViewModel = new TaskbarPanelViewModel(_mainViewModel.MyApplications.Where(a => a.Application.PinnedToTaskbar).ToList());
            DataContext = ViewModel;
        }

        public void SetTaskBarHost(ITaskbarHost host)
        {
            _taskBarHost = host;
        }

        public void InitializeButtons()
        {
            foreach (var applicationViewModel in ViewModel.PinnedApps.Where(a => a.PinnedToTaskbarNotifier))
            {
                AddButton(applicationViewModel);
            }
        }

        public void LayoutIcons(int allowedWidth, int allowedHeight)
        {
            var helper = ServiceLocator.TaskbarHelper;
            
            SetIconSize(helper.TaskBarIconsSize);

            double mrgn = Math.Max(MainButton.Margin.Left, MainButton.Margin.Top);

            if (helper.TaskBarPosition.IsVertical())
            {
                Width = allowedWidth;
                ButtonContainer.Orientation = Orientation.Vertical;
                MainButton.Margin = new Thickness(0, mrgn, 0, mrgn);
            }
            else
            {
                Height = allowedHeight;
                ButtonContainer.Orientation = Orientation.Horizontal;
                MainButton.Margin = new Thickness(mrgn, 0, mrgn, 0);
            }

            NotifyTaskbarOfChange();
        }

        public void PinToTaskbarClickHandler(object sender, EventArgs eventArgs)
        {
            var clickedApp = Helper.GetApplicationViewModelFromContextMenuClick(sender);

            var clickedItem = (MenuItem)sender;

            //Item is checked at this point if when it was clicked it was NOT checked
            if (clickedItem.IsChecked && !ViewModel.PinnedApps.Contains(clickedApp))
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

        public void UninstallAppClickHandler(object sender, EventArgs e)
        {
            var clickedApp = Helper.GetApplicationViewModelFromContextMenuClick(sender);

            try
            {
                _mainViewModel.RemoveApp(clickedApp);
            }
            catch (Exception ex)
            {
                _log.ErrorException("Error during uninstallation of app", ex);

                MessageBox.Show(ex.Message);
            }
        }

        private void AddButton(ApplicationViewModel application)
        {
            var taskbarButton = new TaskbarButton(application, CurrentIconSize);

            taskbarButton.PinToTaskbarClickNotifier += PinToTaskbarClickHandler;
            taskbarButton.UninstallClickNotifier += UninstallAppClickHandler;

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
            if (_taskBarHost != null)
            {
                _taskBarHost.SetDesiredOffset(GetCurrentDimension());
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

        private void RemoveAllAppButtons()
        {
            if (ButtonContainer.Children == null || ButtonContainer.Children.Count <= 1)
            {
                return;
            }

            ButtonContainer.Children.RemoveRange(1, ButtonContainer.Children.Count - 1);

            if (ButtonContainer.Orientation == Orientation.Horizontal)
            {
                Width = MainButton.Width;
            }
            else
            {
                Height = MainButton.Height;
            }

            NotifyTaskbarOfChange();
        }

        public void RemoveAppButton(object sender, EventArgs e)
        {
            var applicationViewModel = sender as ApplicationViewModel;
            ViewModel.RemovePinnedApp(applicationViewModel);
            Helper.PerformInUiThread(() => RemoveButton(applicationViewModel));
        }

        public void RemoveAllAppButtons(object sender, EventArgs e)
        {
            ViewModel.RemoveAllPinnedApps();
            Helper.PerformInUiThread(RemoveAllAppButtons);
        }

        public void AddAppButton(object sender, EventArgs e)
        {
            var applicationViewModel = sender as ApplicationViewModel;
            if (!ViewModel.PinnedApps.Contains(applicationViewModel))
            {
                ViewModel.AddPinnedApp(applicationViewModel);
                Helper.PerformInUiThread(() => AddButton(applicationViewModel));
            }
        }

        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
            (new Thread(() =>
                {
                    InitializeMainWindowLatch.Wait();

                    Helper.PerformInUiThread(() =>
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
                        });
                })).Start();
        }

        private void SetIconSize(TaskbarIconsSize newIconsSize)
        {
            CurrentIconSize = newIconsSize;

            Helper.PerformInUiThread(() =>
                {
                    foreach (var taskbarButton in ButtonContainer.Children.OfType<TaskbarButton>())
                    {
                        taskbarButton.SetIconSize(newIconsSize);
                    }

                    SetMainButtonIconSize(newIconsSize);

                    NotifyTaskbarOfChange();
                });
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

        private void MenuItemExitClick(object sender, RoutedEventArgs e)
        {
            ServiceLocator.TaskbarApi.RemoveTaskbarWindow();
            System.Windows.Application.Current.Shutdown();
        }

        public void SetAllowedSize(int allowedWidth, int allowedHeight)
        {
            if (_allowedHeight != allowedHeight || _allowedWidth != allowedWidth)
            {
                _allowedWidth = allowedWidth;
                _allowedHeight = allowedHeight;
                LayoutIcons(allowedWidth, allowedHeight);
            }
        }
    }
}