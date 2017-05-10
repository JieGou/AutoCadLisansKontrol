using LicenseController.ViewModel;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignDemo;
using MaterialDesignThemes.Wpf;
using MaterialDesignDemo.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using LicenseController.Model;

namespace MaterialDesignColors.WpfExample
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public LicenseController.autocad.masterkey.ws.Service1Client client = new LicenseController.autocad.masterkey.ws.Service1Client();
        string password = "";
        public Login()
        {
            InitializeComponent();
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                password = ((PasswordBox)sender).Password;
        }
        private void SnackbarMessage_HideSnackClick(object sender, RoutedEventArgs e)
        {
            Notification.IsActive = false;
            notificationContent.Content = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = client.Login(NameTextBox.Text, password);

                Notification.IsActive = true;
                if (result!=null)
                {

                    var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
                    mainwindowviewmodel.DemoItem = new DemoItem("Home", new Home { DataContext = new Home() });

                    notificationContent.Content = "Success";
                    GlobalVariable.getInstance().user = result;
                }
                else
                {
                    notificationContent.Content = "Wrong Username or password";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Login Operation");
            }
           
        }
    }
}

