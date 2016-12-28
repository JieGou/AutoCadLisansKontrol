using AutoCadLisansKontrol.DAL;
using MaterialDesignColors.WpfExample;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignDemo.Domain;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
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

namespace MaterialDesignDemo
{
    /// <summary>
    /// Interaction logic for Firm.xaml
    /// </summary>
    public partial class Firm : UserControl
    {
        DataAccess dbAccess = new DataAccess();
        int FirmId;
        public Firm()
        {
            InitializeComponent();
        }

        private void OperationListButton_Click(object sender, RoutedEventArgs e)
        {

            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.SelectedIndex = 1;
            int firmid = (int)(((Button)sender).CommandParameter);

            mainwindowviewmodel.DemoItem = new DemoItem("Operation", new Operation { DataContext = new OperationViewModel(firmid) });
        }

        private void ComputerButton_Click(object sender, RoutedEventArgs e)
        {
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.SelectedIndex = 3;
            int FirmId = (int)(((Button)sender).CommandParameter);

            mainwindowviewmodel.DemoItem = new DemoItem("Computer", new MaterialDesignColors.WpfExample.Computer { DataContext = new ComputerViewModel(FirmId) });
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            FirmId = (int)(((Button)sender).CommandParameter);
            var localfirm = (AutoCadLisansKontrol.DAL.Firm)grdfirm.SelectedItem;
            if (localfirm.Id == 0)
            {
                var userviewmodel = (FirmViewModel)this.DataContext;
                userviewmodel.Firm.Remove(localfirm);
                return;
            }
            ShowDialog();

        }


        private void ShowDialog()
        {


            var userviewmodel = (FirmViewModel)this.DataContext;
            MessageBoxResult result = MessageBox.Show("Are you sure to want to delete firm\"?", "Delete Firm", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    try
                    {
                        dbAccess.DeleteFirm(FirmId);

                        var removeditem = userviewmodel.Firm.SingleOrDefault(x => x.Id == FirmId);

                        userviewmodel.Firm.Remove(removeditem);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {

                            if (ex.InnerException.InnerException != null)
                            {
                                MessageBox.Show(ex.InnerException.InnerException.Message, "Delete Firm");
                            }
                            else
                            {
                                MessageBox.Show(ex.InnerException.Message, "Delete Firm");
                            }
                        }
                        else
                            MessageBox.Show(ex.Message, "Delete Firm");
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
            var userviewmodel = (FirmViewModel)this.DataContext;
            userviewmodel.NotificationIsVisible = false;
        }
    }
}
