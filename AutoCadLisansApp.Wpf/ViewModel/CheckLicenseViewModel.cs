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
using MaterialDesignDemo.Model;
using Microsoft.Win32;
using System.IO;
using MaterialDesignDemo.Controller;

namespace MaterialDesignColors.WpfExample.Domain
{
    public class CheckLicenseViewModel : BaseViewModel, INotifyPropertyChanged
    {

        private bool _isButtonEnable = true;
        public bool IsButtonEnable { get { return _isButtonEnable; } set { _isButtonEnable = value; OnPropertyChanged("IsButtonEnable"); } }
        private string _notificationContent;
        private bool _notificationIsVisible = false;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }

        private int _executedComputer = 0;
        public int ExecutedComputer { get { return _executedComputer; } set { _executedComputer = value; OnPropertyChanged("ExecutedComputer"); } }


        private int _totalComputer = 0;
        public int TotalComputer { get { return _totalComputer; } set { _totalComputer = value; OnPropertyChanged("TotalComputer"); } }


        public ICommand RunClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        public ICommand CancelClicked { get; set; }
        public ICommand SaveOutputClicked { get; set; }

        private int OprId;
        private int FirmId;
        private string _userName;
        private string _password;
        private string[] _keys;
        public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged("UserName"); } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged("Password"); } }



        private ObservableCollection<Software> _softwarelist = new ObservableCollection<Software> {
            new Software { DisplayName="Autod"},
            new Software { DisplayName="3d"},
            new Software { DisplayName="revit"},
            new Software { DisplayName="ecotect"},
            new Software { DisplayName="square one"},
            new Software { DisplayName="Autoc"}
        };

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

        public CheckLicenseViewModel(int oprId, int firmId)
        {
            RunClicked = new RelayCommand(param => CheckLicenseCommand(param));
            SaveClicked = new DelegateCommand(SaveCommand);
            CancelClicked = new DelegateCommand(CancelCommand);
            OprId = oprId;
            FirmId = firmId;
            LoadCheckLicenseFromDb();
            CheckList = new ObservableCollection<MaterialDesignDemo.Model.CheckList>
            {
                new MaterialDesignDemo.Model.CheckList { Name="Add or Remove Programs",WillChecked=true},
                new MaterialDesignDemo.Model.CheckList { Name="Registry Key",WillChecked=true},
                new MaterialDesignDemo.Model.CheckList { Name="Application Events",WillChecked=true},
                new MaterialDesignDemo.Model.CheckList { Name="File Explorer",WillChecked=true}
            };



        }

        public ObservableCollection<Software> SoftwareList
        {
            get
            {
                return _softwarelist;
            }
            set
            {
                _softwarelist = value;
                OnPropertyChanged("SoftwareList");
            }

        }

        private ObservableCollection<CheckLicenseModel> _checkLicenses;
        public ObservableCollection<CheckLicenseModel> CheckLicenses
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
        private ObservableCollection<CheckList> _checkList;
        public ObservableCollection<CheckList> CheckList
        {
            get
            {
                return _checkList;
            }
            set
            {
                _checkList = value;
                OnPropertyChanged("CheckList");
            }
        }
        public MaterialDesignDemo.autocad.masterkey.ws.Operation Operation
        {
            get
            {
                if (OprId == 0) return null;

                return client.GetOperation(OprId);
            }
        }
        public MaterialDesignDemo.autocad.masterkey.ws.Firm Firm
        {
            get
            {
                if (OprId == 0) return null;
                return client.GetFirm(FirmId);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void CheckLicenseCommand(object param)
        {
            StartNotification();

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                EndNotification("UserName or Password is empty");
                return;
            }
            _executedComputer = 0;

            TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            List<MaterialDesignDemo.autocad.masterkey.ws.Computer> computers = client.ListComputer(FirmId).ToList();
            var checklicense = new MTObservableCollection<CheckLicenseModel>();


            if (computers.Count == 0)
            {
                EndNotification("Firm of Operation does not contain any computer!");
                return;
            }

            foreach (var item in computers)
            {
                checklicense.Add(new CheckLicenseModel() { ComputerId = item.Id, Name = item.Name, Ip = item.Ip, FirmId = item.FirmId, IsProgress = true, OperationId = OprId });
            }
            CheckLicenses = checklicense;
            TotalComputer = CheckLicenses.Count;
            System.Action DoInBackground = new System.Action(() =>
            {
                try
                {
                    foreach (var chc in CheckLicenses)
                    {

                        var tempchc = new CheckLicenseModel();
                        System.Action ChildDoInBackground = new System.Action(() =>
                        {

                            tempchc = LicenseDetection.ExecuteWMI(SoftwareList.ToArray(), chc, UserName, Password, OprId, CheckList.ToList());

                        });

                        System.Action ChildDoOnUiThread = new System.Action(() =>
                        {

                            chc.Output = tempchc.Output;
                            chc.IsProgress = false;
                            ExecutedComputer++;

                            if (ExecutedComputer == TotalComputer)
                            {
                                EndNotification("Operation has finished!!");
                            }

                        });
                        var t3 = Task.Factory.StartNew(() => ChildDoInBackground());
                        // when t1 is done run t1..on the Ui thread.
                        var t4 = t3.ContinueWith(t => ChildDoOnUiThread(), _uiScheduler);


                    }
                }
                catch (System.Exception ex)
                {
                    EndNotification(ex.Message);
                    return;
                }

            });

            System.Action DoOnUiThread = new System.Action(() =>
            {

            });


            // start the background task
            var t1 = Task.Factory.StartNew(() => DoInBackground());
            // when t1 is done run t1..on the Ui thread.
            var t2 = t1.ContinueWith(t => DoOnUiThread(), _uiScheduler);
            //I assume BitmapFromUri is the slow step.

        }
        public void LoadCheckLicenseFromDb()
        {
            StartNotification();

            try
            {
                TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var checklicense = new ObservableCollection<CheckLicenseModel>();
                System.Action ChildDoInBackground = new System.Action(() =>
                {
                    checklicense = new ObservableCollection<CheckLicenseModel>(client.ListCheckLicense(OprId).ToList().ConvertAll(x => new CheckLicenseModel { CheckDate = x.CheckDate, ComputerId = x.ComputerId, FirmId = x.FirmId, Id = x.Id, Ip = x.Ip, IsUnlicensed = x.IsUnlicensed, Name = x.Name, OperationId = x.OperationId, Output = x.Output, UpdateDate = x.UpdateDate, State = x.State, Success = (x.State == true ? true : false) }));
                });

                System.Action ChildDoOnUiThread = new System.Action(() =>
                {
                    CheckLicenses = checklicense;
                });


                var t1 = Task.Factory.StartNew(() => ChildDoInBackground());
                // when t1 is done run t1..on the Ui thread.
                var t2 = t1.ContinueWith(t => ChildDoOnUiThread(), _uiScheduler);
            }
            catch (System.Exception ex)
            {
                EndNotification(ex.Message);
                return;
            }

            EndNotification("");
        }
        public void CancelCommand() {
            EndNotification("");


        }

        public void SaveCommand()
        {
            try
            {
                var counter = CheckLicenses.Count;
                StartNotification();
                TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                System.Action ChildDoInBackground = new System.Action(() =>
                {

                    //client.DeleteAllLicenseBaseOperationid(OprId);
                    foreach (var item in CheckLicenses)
                    {
                        counter--;
                        client.UpsertCheckLicense(new MaterialDesignDemo.autocad.masterkey.ws.CheckLicense()
                        {
                            CheckDate = System.DateTime.Now,
                            ComputerId = item.ComputerId,
                            FirmId = item.FirmId,
                            Id = item.Id,
                            Ip = item.Ip,
                            IsUnlicensed = item.IsUnlicensed,
                            Name = item.Name,
                            OperationId = item.OperationId,
                            Output = item.Output,
                            UpdateDate = System.DateTime.Now,
                            State = item.State,
                            Installed = item.Installed,
                            Uninstalled = item.Uninstalled
                        });

                        if (counter == 0)
                            EndNotification("Results is saved.");

                    }
                });

                System.Action ChildDoOnUiThread = new System.Action(() =>
                {

                });

                var t1 = Task.Factory.StartNew(() => ChildDoInBackground());
                // when t1 is done run t1..on the Ui thread.
                var t2 = t1.ContinueWith(t => ChildDoOnUiThread(), _uiScheduler);

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
            if (content != "")
            {
                NotificationIsVisible = true;
                NotificationContent = content;
            }
            IsButtonEnable = true;
            ProgressBar = Visibility.Hidden;

        }
    }
}
