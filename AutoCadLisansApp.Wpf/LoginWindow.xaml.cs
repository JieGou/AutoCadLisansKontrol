using MaterialDesignColors.WpfExample;
using MaterialDesignColors.WpfExample.Domain;
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
using System.Windows.Shapes;

namespace LicenseController
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LicenseController.autocad.masterkey.ws.Service1Client client = new LicenseController.autocad.masterkey.ws.Service1Client();
        string password = "";
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
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
                if (result)
                {
                    var newForm = new MainWindow(); //create your new form.
                    newForm.Show(); //show the new form.
                    this.Close(); //only if you want to close the current form.

                    notificationContent.Content = "Success";
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
