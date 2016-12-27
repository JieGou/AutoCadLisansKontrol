using AutoCadLisansKontrol.Controller;
using AutoCadLisansKontrol.DAL;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignDemo.Domain;
using MaterialDesignDemo.Model;
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
        DataAccess dbAccess = new DataAccess();
        private string ip;
        public Computer()
        {
            InitializeComponent();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ip = (string)(((Button)sender).CommandParameter);

            var localcomputer = (ComputerModel)grdComputer.SelectedItem;
            if (localcomputer.Id == 0)
            {
                var userviewmodel = (ComputerViewModel)this.DataContext;
                userviewmodel.Computers.Remove(localcomputer);

                userviewmodel.ExecutedComputer = userviewmodel.Computers.Count;
                userviewmodel.TotalComputer = userviewmodel.Computers.Count;
                return;
            }
            ShowDialog();

        }
       
        private void ShowDialog()
        {        
            var userviewmodel = (ComputerViewModel)this.DataContext;
            MessageBoxResult result = MessageBox.Show("Are you sure to want to delete ?", "Delete Computer", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    try
                    {

                        dbAccess.DeleteComputer(ip);

                        var removeditem = userviewmodel.Computers.SingleOrDefault(x => x.Ip.Contains(ip));

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
    }
}
