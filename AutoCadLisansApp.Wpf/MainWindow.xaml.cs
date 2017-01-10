using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;
using System.Windows.Data;
using System;
using System.Globalization;

namespace MaterialDesignColors.WpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        public MainWindow()
        {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(ListBoxItem), ListBoxItem.MouseLeftButtonDownEvent, new RoutedEventHandler(this.MouseLeftButtonDownClassHandler));
            DataContext = new MainWindowViewModel();

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2500);
            }).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher2
                MainSnackbar.MessageQueue.Enqueue("Welcome to Application License Control Tookit");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void MouseLeftButtonDownClassHandler(object sender, RoutedEventArgs e)
        {
            DemoItem item = (DemoItem)DemoItemsListBox.SelectedItem;
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = item;
        }
        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private async void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            var sampleMessageDialog = new SampleMessageDialog
            {
                Message = { Text = ((ButtonBase)sender).Content.ToString() }
            };

            await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }

        private void DemoItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DemoItem item = (DemoItem)DemoItemsListBox.SelectedItem;
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = item;

        }

        private void DemoItemsListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DemoItem item = (DemoItem)DemoItemsListBox.SelectedItem;
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = item;
        }

        private void DemoItemsListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DemoItem item = (DemoItem)DemoItemsListBox.SelectedItem;
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = item;
        }

        private void DemoItemsListBox_Click(object sender, RoutedEventArgs e)
        {
            DemoItem item = (DemoItem)DemoItemsListBox.SelectedItem;
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = item;
        }

        private void DemoItemsListBox_SelectionTouch(object sender, TouchEventArgs e)
        {
            DemoItem item = (DemoItem)DemoItemsListBox.SelectedItem;
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = item;
        }
    }
}
