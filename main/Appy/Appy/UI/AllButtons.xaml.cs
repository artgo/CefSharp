﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            if (ButtonContainer.Orientation == Orientation.Horizontal)
            {
                Width += 40;
            }
            else
            {
                Height += 40;
            }


            if (TaskbarCallbackEvents != null)
            {
                if (ButtonContainer.Orientation == Orientation.Horizontal)
                {
                    TaskbarCallbackEvents.ChangeWidth((int)ButtonContainer.Width);
                }
                else
                {
                    TaskbarCallbackEvents.ChangeWidth((int)ButtonContainer.Height);
                }
            }
        }

        private void RemoveButton(Application application)
        {
            var btn = ButtonContainer.Children.OfType<TaskbarButton>().FirstOrDefault(b => b.Name == application.Id);

            if (btn != null)
            {
                ButtonContainer.Children.Remove(btn);
                if (ButtonContainer.Orientation == Orientation.Horizontal)
                {
                    Width -= 40;
                }
                else
                {
                    Height -= 40;
                }
                
                if (TaskbarCallbackEvents != null)
                {
                    if (ButtonContainer.Orientation == Orientation.Horizontal)
                    {
                        TaskbarCallbackEvents.ChangeWidth((int)ButtonContainer.Width);
                    }
                    else
                    {
                        TaskbarCallbackEvents.ChangeWidth((int)ButtonContainer.Height);
                    }
                }
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
            // TODO: implement
            throw new NotImplementedException();
        }

        public void PositionChanged(TaskbarPosition newPosition)
        {
            var widthTemp = Width;
            Width = Height;
            Height = widthTemp;

            if (newPosition == TaskbarPosition.Bottom || newPosition == TaskbarPosition.Top)
            {
                ButtonContainer.Orientation = Orientation.Horizontal;

            }
            else
            {
                ButtonContainer.Orientation = Orientation.Vertical;
            }
        }

        public void TaskbarIconsSizeChanged(TaskbarIconsSize newIconsSize)
        {
            // TODO: implement
            throw new NotImplementedException();
        }

        public ITaskbarInteropCallback TaskbarCallbackEvents { get; set; }

        private void Cog_click(object sender, RoutedEventArgs e)
        {
            if (ButtonContainer.Orientation == Orientation.Horizontal)
            {
                PositionChanged(TaskbarPosition.Left);
            }
            else
            {
                PositionChanged(TaskbarPosition.Top);
            }
        }
    }
}
