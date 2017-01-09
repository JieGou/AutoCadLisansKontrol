
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
        private MaterialDesignDemo.autocad.masterkey.ws.Service1Client client = new MaterialDesignDemo.autocad.masterkey.ws.Service1Client();
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

                        client.DeleteComputer(id);

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
    }
}
