using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for CheckLicense.xaml
    /// </summary>
    public partial class CheckLicense : UserControl
    {
        public CheckLicense()
        {
            InitializeComponent();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((CheckLicenseViewModel)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }
        private void SnackbarMessage_HideSnackClick(object sender, RoutedEventArgs e)
        {
            var userviewmodel = (CheckLicenseViewModel)DataContext;
            userviewmodel.NotificationIsVisible = false;
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
        {
            var chechlicenseitem = (autocad.masterkey.ws.CheckLicense)grdCheckList.SelectedItem;



        }
        private void Sample2_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("SAMPLE 2: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));
        }
    }
}
