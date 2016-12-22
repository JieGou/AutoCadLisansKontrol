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
            
            ShowDialog();
            
        }
        

        private void ShowDialog()
        {
            var userviewmodel = (FirmViewModel)this.DataContext;
            MessageBoxResult result = MessageBox.Show("Would you like to greet the world with a \"Hello, world\"?", "My App", MessageBoxButton.YesNoCancel);
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

                        MessageBox.Show(ex.InnerException.InnerException.Message, "Delete Operation");
                    }
                   
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }
    }
}
