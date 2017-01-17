using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections;
using System.Threading;
using MaterialDesignDemo.ViewModel;
using AutoCadLisansKontrol.Controller;
using MaterialDesignDemo;

namespace MaterialDesignColors.WpfExample.Domain
{
    public class CheckLicenseViewModel : BaseViewModel, INotifyPropertyChanged
    {

        private bool _isButtonEnable = true;
        public bool IsButtonEnable { get { return _isButtonEnable; } set { _isButtonEnable = value; OnPropertyChanged("IsButtonEnable"); } }
        private string _notificationContent;
        private bool _notificationIsVisible;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }

        private int _executedComputer = 0;
        public int ExecutedComputer { get { return _executedComputer; } set { _executedComputer = value; OnPropertyChanged("ExecutedComputer"); } }

        public ICommand RunClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        private int OprId;
        private int FirmId;
        private string _userName;
        private string _password;

        public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged("UserName"); } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged("Password"); } }

        private ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.CheckLicense> _checkLicenses;

        private Visibility _progressbar = Visibility.Hidden;
        public Visibility ProgressBar
        {
            get { return _progressbar; }
            set
            {
                _progressbar = value;
                OnPropertyChanged("ProgressBar");
            }
        }

        public CheckLicenseViewModel(int oprId,int firmId)
        {
            RunClicked = new RelayCommand(param => CheckLicenseCommand(param));
            SaveClicked = new DelegateCommand(SaveCommand);
            OprId = oprId;
            FirmId = firmId;
        }




        public ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.CheckLicense> CheckLicenses
        {
            get
            {
                return _checkLicenses;
            }
            set
            {
                _checkLicenses = value;
                OnPropertyChanged("CheckLicenses");
            }

        }
        public MaterialDesignDemo.autocad.masterkey.ws.Firm Firm
        {
            get
            {
                if (OprId == 0) return null;

                var opr = client.GetOperation(OprId);
                return client.GetFirm(opr.FirmId);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                //PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void CheckLicenseCommand(object param)
        {
            StartNotification();
             _executedComputer = 0;
           
            TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            
            List<MaterialDesignDemo.autocad.masterkey.ws.Computer> computers=client.ListComputer(FirmId).ToList();
            if (computers.Count == 0)
            {
                EndNotification("Firm of Operation does not contain any computer!");
                return;
            }
            var checklicense = new ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.CheckLicense>();
            System.Action DoInBackground = new System.Action(() =>
            {   
                try
                {
                    foreach (var comp in computers)
                    {
                        checklicense.Add(LicenseDetection.Execute(comp, UserName, Password,OprId));
                    }

                    ProgressBar = Visibility.Hidden;
                }
                catch (System.Exception ex)
                {
                    NotificationIsVisible = true;
                    NotificationContent = ex.Message;
                    return;
                }

            });

            System.Action DoOnUiThread = new System.Action(() =>
            {
                if (NotificationIsVisible == true)
                    return;
                CheckLicenses = checklicense;

                NotificationIsVisible = true;
                NotificationContent = "Success";
                IsButtonEnable = true;
            });

            // start the background task
            var t1 = Task.Factory.StartNew(() => DoInBackground());
            // when t1 is done run t1..on the Ui thread.
            var t2 = t1.ContinueWith(t => DoOnUiThread(), _uiScheduler);
            //I assume BitmapFromUri is the slow step.

            //Now that we have our bitmap, now go to the main thread.

        }
        public void LoadComputerFromDb()
        {

            NotificationIsVisible = false;
            IsButtonEnable = false;
            ProgressBar = Visibility.Visible;
            var computers = new ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.Computer>(client.ListComputer(Firm.Id).ToList());
            NotificationContent = "Success";
            NotificationIsVisible = true;
            ProgressBar = Visibility.Hidden;
            IsButtonEnable = true;
        }


        public void SaveCommand()
        {
            try
            {
                IsButtonEnable = false;
                ProgressBar = Visibility.Visible;

                NotificationIsVisible = false;




                NotificationIsVisible = true;
                NotificationContent = "Success";
                IsButtonEnable = true;
                ProgressBar = Visibility.Hidden;
            }
            catch (System.Exception ex)
            {
                NotificationIsVisible = true;
                NotificationContent = ex.Message;
            }

        }
        public void StartNotification()
        {
            IsButtonEnable = false;
            ProgressBar = Visibility.Visible;
            NotificationIsVisible = false;
        }
        public void EndNotification(string content)
        {
            NotificationIsVisible = true;
            NotificationContent = content;
            IsButtonEnable = true;
            ProgressBar = Visibility.Hidden;

        }
    }
}
