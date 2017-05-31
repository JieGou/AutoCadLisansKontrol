using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MaterialDesignDemo.ViewModel;
using AutoCadLisansKontrol.Controller;
using MaterialDesignDemo.Model;
using LicenseController.autocad.masterkey.ws;
using LicenseController.Model;
using AutoCadLisansKontrol.DAL;

namespace MaterialDesignColors.WpfExample.Domain
{
    public class CheckLicenseViewModel : BaseViewModel, INotifyPropertyChanged
    {

        private bool _isButtonEnable = true;
        public bool IsButtonEnable { get { return _isButtonEnable; } set { _isButtonEnable = value; OnPropertyChanged("IsButtonEnable"); } }

        private bool _autoSave = true;
        public bool AutoSave { get { return _autoSave; } set { _autoSave = value; OnPropertyChanged("AutoSave"); } }

        private string _notificationContent;
        private bool _notificationIsVisible = false;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }

        private int _executedComputer = 0;
        public int ExecutedComputer { get { return _executedComputer; } set { _executedComputer = value; OnPropertyChanged("ExecutedComputer"); } }

        private int _totalComputer = 0;
        public int TotalComputer { get { return _totalComputer; } set { _totalComputer = value; OnPropertyChanged("TotalComputer"); } }

        public bool _isremote = true;
        public bool IsRemote { get { return _isremote; } set { _isremote = value; OnPropertyChanged("IsRemote"); } }


        public ICommand RunClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        public ICommand CancelClicked { get; set; }
        public ICommand SaveOutputClicked { get; set; }
        public ICommand ExporttoExcelClicked { get; set; }

        public int OprId;
        public int FirmId;
        private string _userName;
        private string _password;
        private string[] _keys;
        public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged("UserName"); } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged("Password"); } }

        private ObservableCollection<SoftwareDTO> _softwarelist = new ObservableCollection<SoftwareDTO>();

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

            RunClicked = new DelegateCommand(CheckLicenseCommand);
            SaveClicked = new DelegateCommand(SaveCommandasync);
            CancelClicked = new DelegateCommand(CancelCommand);
            OprId = oprId;
            FirmId = firmId;
            LoadCheckLicenseFromDb();
            UIContext.Initialize();
        }

        public ObservableCollection<SoftwareDTO> SoftwareList
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
        private ObservableCollection<LicenseController.autocad.masterkey.ws.ComputerDTO> _computers;
        public ObservableCollection<LicenseController.autocad.masterkey.ws.ComputerDTO> Computers
        {
            get
            {
                return _computers;
            }
            set
            {
                _computers = value;
                OnPropertyChanged("Computers");
            }

        }
        private ObservableCollection<ControlPointDTO> _checkList;
        public ObservableCollection<ControlPointDTO> CheckList
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
        public LicenseController.autocad.masterkey.ws.OperationDTO Operation
        {
            get
            {
                if (OprId == 0) return null;

                return client.OperationGet(OprId);
            }
        }
        public LicenseController.autocad.masterkey.ws.FirmDTO Firm
        {
            get
            {
                LicenseController.autocad.masterkey.ws.FirmDTO firm = null;
                try
                {

                    if (OprId == 0) return null;
                    firm = client.FirmGet(FirmId);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "Login Operation");
                }
                return firm;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private CancellationTokenSource Canceller { get; set; }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void CheckLicenseCommand()
        {

            switch (IsRemote)
            {
                case true:
                    CheckLicenseRemoteCommand();
                    break;
                case false:
                    CheckLicenseLocalCommand();
                    break;

                default:
                    break;
            }

        }
        async Task ExecuteAsync()
        {

        }
        public void CheckLicenseLocalCommand()
        {

            StartNotification();

            _executedComputer = 0;
            Computers = new ObservableCollection<LicenseController.autocad.masterkey.ws.ComputerDTO>();
            var checklicense = new MTObservableCollection<CheckLicenseModel>();

            var localcomp = ComputerDetection.ExecuteLocal();
            localcomp.FirmId = FirmId;
            Computers.Add(localcomp);



            System.Action DoInBackground = new System.Action(() =>
        {
            try
            {
                var cmpid = client.ComputerUpsert(localcomp);


                foreach (var soft in SoftwareList)
                {
                    checklicense.Add(new CheckLicenseModel() { ComputerId = cmpid, Name = localcomp.Name, Ip = localcomp.Ip, FirmId = FirmId, IsProgress = true, OperationId = OprId, App = soft });
                }

                CheckLicenses = checklicense;
                TotalComputer = CheckLicenses.Count;

                var counter = CheckLicenses.Count;
                foreach (var chc in CheckLicenses)
                {
                    LicenseDetection detect = new LicenseDetection();
                    var tempchc = new CheckLicenseModel();
                    System.Action ChildDoInBackground = new System.Action(() =>
                       {
                           tempchc = detect.ExecuteWMIForOneApp(chc, UserName, Password, OprId, CheckList.ToList(), IsRemote);

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
                        counter--;
                    });
                    Canceller = new CancellationTokenSource();
                    var t3 = Task.Factory.StartNew(() =>
                    {
                        using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                        {
                            ChildDoInBackground();
                        }
                    }, Canceller.Token);
                    //, TaskCreationOptions.None, UIContext.Current
                    // when t1 is done run t1..on the Ui thread.
                    var t4 = t3.ContinueWith(t =>
                {
                    using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                    {
                        ChildDoOnUiThread();
                    }
                }, Canceller.Token);

                    //, TaskContinuationOptions.OnlyOnFaulted, UIContext.Current
                }
                while (counter != 0)
                {

                }
                if (AutoSave)
                    SaveCommandsync();
            }
            catch (System.Exception ex)
            {
                EndNotification(ex.Message);
                return;
            }
        });

            // start the background task
            Canceller = new CancellationTokenSource();
            var t1 = Task.Factory.StartNew(() =>
            {
                using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                {
                    DoInBackground();
                }
            }, Canceller.Token);
        }
        public void CheckLicenseRemoteCommand()
        {
            StartNotification();

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password) && IsRemote == true)
            {
                EndNotification("UserName or Password is empty");
                return;
            }
            _executedComputer = 0;



            System.Action DoInBackground = new System.Action(() =>
            {
                try
                {
                    Computers = new ObservableCollection<LicenseController.autocad.masterkey.ws.ComputerDTO>(client.ComputerList(FirmId).ToList());
                    var checklicense = new MTObservableCollection<CheckLicenseModel>();


                    if (Computers.Count == 0 && IsRemote == true)
                    {
                        EndNotification("Firm of Operation does not contain any computer!");
                        return;
                    }

                    foreach (var item in Computers)
                    {
                        foreach (var sft in SoftwareList)
                        {
                            checklicense.Add(new CheckLicenseModel() { ComputerId = item.Id, Name = item.Name, Ip = item.Ip, FirmId = item.FirmId, IsProgress = true, OperationId = OprId, App = sft });
                        }
                    }

                    CheckLicenses = checklicense;
                    TotalComputer = CheckLicenses.Count;

                    var counter = CheckLicenses.Count;
                    foreach (var chc in CheckLicenses)
                    {
                        LicenseDetection detect = new LicenseDetection();
                        var tempchc = new CheckLicenseModel();
                        System.Action ChildDoInBackground = new System.Action(() =>
                        {
                            tempchc = detect.ExecuteWMIForOneApp(chc, UserName, Password, OprId, CheckList.ToList(), IsRemote);

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
                            counter--;
                        });
                        Canceller = new CancellationTokenSource();
                        var t3 = Task.Factory.StartNew(() =>
                        {
                            using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                            {

                                ChildDoInBackground();

                            }
                        }, Canceller.Token);
                        //, TaskCreationOptions.None, UIContext.Current
                        // when t1 is done run t1..on the Ui thread.
                        var t4 = t3.ContinueWith(t =>
                        {
                            using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                            {
                                ChildDoOnUiThread();
                            }
                        }, Canceller.Token);

                        //, TaskContinuationOptions.OnlyOnFaulted, UIContext.Current
                    }
                    while (counter != 0)
                    {

                    }
                    if (AutoSave)
                        SaveCommandsync();
                }
                catch (System.Exception ex)
                {
                    EndNotification(ex.Message);
                    return;
                }
            });

            // start the background task
            Canceller = new CancellationTokenSource();
            var t1 = Task.Factory.StartNew(() =>
            {
                using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                {
                    DoInBackground();
                }
            }, Canceller.Token);
            // when t1 is done run t1..on the Ui thread.


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
                    try
                    {
                        _softwarelist = new ObservableCollection<SoftwareDTO>(client.GetAllApplication().ToList());
                        CheckList = new ObservableCollection<ControlPointDTO>(client.GetControlPoint().ToList());
                        var checklist = Converter.Convert<CheckLicenseModel, CheckLicenseDTO>(client.CheckLicenseList(OprId).ToList());
                        checklist.ForEach(x => x.App = _softwarelist.Where(y => y.Id == x.AppId).FirstOrDefault<SoftwareDTO>());
                        checklicense = new ObservableCollection<CheckLicenseModel>(checklist);
                        TotalComputer = checklist.Count;
                        ExecutedComputer = checklist.Count;
                    }
                    catch (System.Exception ex)
                    {
                        var message = ex.Message;
                    }
                    EndNotification("Load Finish..");
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


        }
        public void CancelCommand()
        {
            EndNotification("");



            try
            {
                TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var checklicense = new ObservableCollection<CheckLicenseModel>();
                System.Action ChildDoInBackground = new System.Action(() =>
                {
                    foreach (var item in CheckLicenses)
                    {
                        item.IsProgress = false;
                    }
                    Canceller.Cancel();

                });


                var t1 = Task.Factory.StartNew(() => ChildDoInBackground());
                // when t1 is done run t1..on the Ui thread.
            }
            catch (System.Exception ex)
            {
                EndNotification(ex.Message);
                return;
            }

        }
        public void SaveCommandsync()
        {

            var counter = CheckLicenses.Count;
            StartNotification();


            //client.DeleteAllLicenseBaseOperationid(OprId);
            foreach (var item in CheckLicenses)
            {
                counter--;
                try
                {


                    client.CheckLicenseUpsert(new LicenseController.autocad.masterkey.ws.CheckLicenseDTO()
                    {
                        CheckDate = System.DateTime.Now,
                        ComputerId = item.ComputerId,
                        FirmId = item.FirmId,
                        Id = item.Id,
                        Ip = item.Ip,
                        Name = item.Name,
                        OperationId = item.OperationId,
                        Output = item.Output,
                        UpdateDate = System.DateTime.Now,
                        State = item.State,
                        Installed = item.Installed,
                        Uninstalled = item.Uninstalled,
                        AppId = item.App.Id,
                        InstallDate = item.InstallDate,
                        UnInstallDate = item.UnInstallDate,
                        IsFound = item.IsFound,
                        LogId = item.LogId
                    });
                }
                catch (System.Exception ex)
                {
                    EndNotification(ex.Message);
                }

                if (counter == 0)
                    EndNotification("Results is saved.");

            }


            // when t1 is done run t1..on the Ui thread.




        }
        public void SaveCommandasync()
        {

            var counter = CheckLicenses.Count;
            StartNotification();

            System.Action ChildDoInBackground = new System.Action(() =>
            {
                //client.DeleteAllLicenseBaseOperationid(OprId);
                foreach (var item in CheckLicenses)
                {
                    counter--;
                    try
                    {
                        client.CheckLicenseUpsert(new LicenseController.autocad.masterkey.ws.CheckLicenseDTO()
                        {
                            CheckDate = System.DateTime.Now,
                            ComputerId = item.ComputerId,
                            FirmId = item.FirmId,
                            Id = item.Id,
                            Ip = item.Ip,
                            Name = item.Name,
                            OperationId = item.OperationId,
                            Output = item.Output,
                            UpdateDate = System.DateTime.Now,
                            State = item.State,
                            Installed = item.Installed,
                            Uninstalled = item.Uninstalled,
                            AppId = item.App.Id,
                            InstallDate = item.InstallDate,
                            UnInstallDate = item.UnInstallDate,
                            IsFound = item.IsFound,
                            LogId = item.LogId
                        });
                    }
                    catch (System.Exception ex)
                    {
                        EndNotification(ex.Message);
                    }

                    if (counter == 0)
                        EndNotification("Results is saved.");

                }
            });


            Canceller = new CancellationTokenSource();

            var t1 = Task.Factory.StartNew(() =>
            {
                using (Canceller.Token.Register(Thread.CurrentThread.Abort))
                {
                    ChildDoInBackground();
                }
            }, Canceller.Token);
            // when t1 is done run t1..on the Ui thread.


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
