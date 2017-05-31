
using LicenseController.autocad.masterkey.ws;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignDemo;
using MaterialDesignDemo.Domain;
using MaterialDesignDemo.Model;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaterialDesignColors.WpfExample
{
    /// <summary>
    /// Interaction logic for Computer.xaml
    /// </summary>
    public partial class Computer : UserControl
    {
        private LicenseController.autocad.masterkey.ws.Service1Client client = new LicenseController.autocad.masterkey.ws.Service1Client();
        public Computer()
        {
            InitializeComponent();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            var localcomputer = (ComputerModel)grdComputer.SelectedItem;
            if (localcomputer.Id == 0)
            {
                var userviewmodel = (ComputerViewModel)this.DataContext;
                userviewmodel.Computers.Remove(localcomputer);

                userviewmodel.ExecutedComputer = userviewmodel.Computers.Count;
                userviewmodel.TotalComputer = userviewmodel.Computers.Count;
                return;
            }
            ShowDialog(localcomputer.Id);

        }
       
        private void ShowDialog(int id)
        {        
            var userviewmodel = (ComputerViewModel)this.DataContext;
            MessageBoxResult result = MessageBox.Show("Are you sure to want to delete ?", "Delete Computer", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    try
                    {

                        client.ComputerDelete(id);

                        var removeditem = userviewmodel.Computers.SingleOrDefault(x => x.Id==(id));

                        userviewmodel.Computers.Remove(removeditem);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {

                            if (ex.InnerException.InnerException != null)
                            {
                                MessageBox.Show(ex.InnerException.InnerException.Message, "Delete Operation");
                            }
                            else
                            {
                                MessageBox.Show(ex.InnerException.Message, "Delete Operation");
                            }
                        }
                        else
                            MessageBox.Show(ex.Message, "Delete Operation");
                    }


                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void SnackbarMessage_HideSnackClick(object sender, RoutedEventArgs e)
        {
            var userviewmodel = (ComputerViewModel)DataContext;
            userviewmodel.NotificationIsVisible = false;
        }

        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = new DemoItem("Firma", new MaterialDesignDemo.Firm { DataContext = new FirmViewModel() });
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void grdComputer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        }

        private void scrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        }
        private void CheckLicenseButton_Click(object sender, RoutedEventArgs e)
        {
            var localoperation = (LicenseController.autocad.masterkey.ws.OperationDTO)grdOperationonComputerpage.SelectedItem;
           
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = new DemoItem("CheckLicense", new CheckLicense { DataContext = new CheckLicenseViewModel(localoperation.Id, localoperation.FirmId) });
        }
        private void Sample1_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            
            if (!Equals(eventArgs.Parameter, true)) return;

            var computermodel = (ComputerViewModel)DataContext;


            if (!string.IsNullOrWhiteSpace(OperationBox.Text)) { 
                computermodel.Operation.Add(new OperationDTO() { Name = OperationBox.Text.Trim(),FirmId= computermodel.FirmId });
                OperationViewModel operation = new OperationViewModel(computermodel.FirmId);
                operation.Operation = computermodel.Operation;
                operation.SaveCommand();
            }
        

            OperationBox.Text = "";
        }
    }
}
