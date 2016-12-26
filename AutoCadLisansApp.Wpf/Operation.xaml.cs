using AutoCadLisansKontrol.DAL;
using MaterialDesignColors.WpfExample;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignDemo.Domain;
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
    /// Interaction logic for Operation.xaml
    /// </summary>
    public partial class Operation : UserControl
    {

        DataAccess dbaccess = new DataAccess();
        public Operation()
        {

            InitializeComponent();
        }

        private void CheckLicenseButton_Click(object sender, RoutedEventArgs e)
        {
            int opr = (int)(((Button)sender).CommandParameter);
            
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = new DemoItem("Operation", new Operation { DataContext = new OperationViewModel() });
        }

        private void SnackbarMessage_HideSnackClick(object sender, RoutedEventArgs e)
        {
            var userviewmodel = (OperationViewModel)DataContext;
            userviewmodel.NotificationIsVisible = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            
            var localoperation = (AutoCadLisansKontrol.DAL.Operation)grdOperation.SelectedItem;
            if (localoperation.Id == 0)
            {
                var userviewmodel = (OperationViewModel)this.DataContext;
                userviewmodel.Operation.Remove(localoperation);
                return;
            }
            ShowDialog(localoperation);
        }
        private void ShowDialog(AutoCadLisansKontrol.DAL.Operation opr)
        {


            var userviewmodel = (OperationViewModel)this.DataContext;
            MessageBoxResult result = MessageBox.Show("Are you sure to want to delete Operation\"?", "Delete Firm", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    try
                    {
                        dbaccess.DeleteOperation(opr);

                        var removeditem = userviewmodel.Operation.SingleOrDefault(x => x.Id == opr.Id);

                        userviewmodel.Operation.Remove(removeditem);
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

        private void Chip_Click(object sender, RoutedEventArgs e)
        {

            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = new DemoItem("Firma", new MaterialDesignDemo.Firm { DataContext = new FirmViewModel() });
        }
    }
}
