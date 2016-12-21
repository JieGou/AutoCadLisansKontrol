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
    /// Interaction logic for Firm.xaml
    /// </summary>
    public partial class Firm : UserControl
    {
        public Firm()
        {
            InitializeComponent();
        }

        private void OperationListButton_Click(object sender, RoutedEventArgs e)
        {
            int firmid = (int)(((Button)sender).CommandParameter);
            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = new DemoItem("Operation", new Operation { DataContext = new OperationViewModel(firmid) });
        }

        private void ComputerButton_Click(object sender, RoutedEventArgs e)
        {
            int FirmId = (int)(((Button)sender).CommandParameter);

            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = new DemoItem("Computer", new Computer { DataContext = new ComputerViewModel(FirmId) });
        }
    }
}
