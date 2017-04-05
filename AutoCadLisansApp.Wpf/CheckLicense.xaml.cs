using LicenseController.autocad.masterkey.ws;
using MaterialDesignColors.WpfExample.Domain;
using MaterialDesignDemo.Controller;
using MaterialDesignDemo.Domain;
using MaterialDesignDemo.Model;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            var chechlicenseitem = (LicenseController.autocad.masterkey.ws.CheckLicense)grdCheckList.SelectedItem;

            string output = (string)(((Button)sender).CommandParameter);

            SaveOutput(output);
        }
        private void Sample2_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("SAMPLE 2: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));
        }
        private void Sample1_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {

            if (!Equals(eventArgs.Parameter, true)) return;

            var checklicensemodel = (CheckLicenseViewModel)DataContext;


            if (!string.IsNullOrWhiteSpace(SoftwareBox.Text))
                checklicensemodel.SoftwareList.Add(new Software() { AppName = SoftwareBox.Text.Trim() });

            SoftwareBox.Text = "";
        }

        public void SaveOutput(string content)
        {
            string fileText = content;

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, fileText);
            }
        }


        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            var checklicensemodel = (CheckLicenseViewModel)DataContext;

            var mainwindowviewmodel = Window.GetWindow(this).DataContext as MainWindowViewModel;
            mainwindowviewmodel.DemoItem = new DemoItem("Operation", new MaterialDesignDemo.Operation { DataContext = new OperationViewModel(checklicensemodel.FirmId) });
        }

        private void scrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveExcel();
        }
        public void SaveExcel()
        {
            var checklicensemodel = (CheckLicenseViewModel)DataContext;
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Archivos Excel (*.xls)|*.xls|Todos los Archivos (*.*)|*.*",
                FileName = "Result of Sniff"
            };

            if (dialog.ShowDialog() == true)
            {
                Excel.ExportExcel(checklicensemodel.CheckLicenses.ToList(), dialog.FileName);
            }

        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {

            var checklicensemodel = (CheckLicenseViewModel)DataContext;

            if (checklicensemodel.IsRemote == false)
            {
                NameTextBox.IsEnabled = false;
                PasswordBox.IsEnabled = false;
            }
            else
            {
                NameTextBox.IsEnabled = true;
                PasswordBox.IsEnabled = true;
            }
        }
    }
}
