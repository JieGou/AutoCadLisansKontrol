﻿using LicenseController.Model;
using MaterialDesignColors.WpfExample;
using MaterialDesignColors.WpfExample.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO.IsolatedStorage;

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
            if (App.Current.Properties[2] != null)
            {
                NameTextBox.Text = App.Current.Properties[0].ToString();
                PasswordBox.Password = App.Current.Properties[1].ToString();
                chkrememberme.IsChecked = Convert.ToBoolean(App.Current.Properties[2]);
            }
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
                if (result != null)
                {

                    GlobalVariable.getInstance().user = result;
                    if ((bool)chkrememberme.IsChecked)
                    {
                        App.Current.Properties[0] = NameTextBox.Text;
                        App.Current.Properties[1] = password;
                        App.Current.Properties[2] = chkrememberme.IsChecked;
                    }
                    else
                    {
                        App.Current.Properties[0] = null;
                        App.Current.Properties[1] = null;
                        App.Current.Properties[2] = null;
                    }
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
