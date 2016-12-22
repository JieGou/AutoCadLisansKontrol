using AutoCadLisansKontrol.Controller;
using AutoCadLisansKontrol.DAL;
using MaterialDesignColors.WpfExample.Domain;
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

            ShowDialog();

        }


        private void ShowDialog()
        {
            if (ip == "")
            {
                MessageBox.Show("Data must be saved before delete operation", "Delete Operation");
                return;
            }


            var userviewmodel = (ComputerViewModel)this.DataContext;
            MessageBoxResult result = MessageBox.Show("Would you like to greet the world with a \"Hello, world\"?", "My App", MessageBoxButton.YesNoCancel);
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
