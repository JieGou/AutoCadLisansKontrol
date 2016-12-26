using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoCadLisansKontrol.Controller;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using AutoCadLisansKontrol.DAL;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections;
using System.Threading;
using MaterialDesignDemo.Model;


namespace MaterialDesignColors.WpfExample.Domain
{
    public class CheckLicenseViewModel : INotifyPropertyChanged
    {
        private bool _isButtonEnable = true;
        public bool IsButtonEnable { get { return _isButtonEnable; } set { _isButtonEnable = value; OnPropertyChanged("IsButtonEnable"); } }
        private string _notificationContent;
        private bool _notificationIsVisible;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }

        private int _executedComputer = 0;
        public int ExecutedComputer { get { return _executedComputer; } set { _executedComputer = value; OnPropertyChanged("ExecutedComputer"); } }

        DataAccess dbAccess = new DataAccess();
        public ICommand buttonClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand LoadDbClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        private int OprId;

        private ObservableCollection<ComputerModel> _computers;

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

        public CheckLicenseViewModel(int oprId)
        {
            buttonClicked = new DelegateCommand(CheckLicenseCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            LoadDbClicked = new DelegateCommand(LoadComputerFromDb);
            SaveClicked = new DelegateCommand(SaveCommand);
            OprId = oprId;
            LoadComputerFromDb();
        }




        public ObservableCollection<ComputerModel> Computers
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
        public AutoCadLisansKontrol.DAL.Firm Firm
        {
            get
            {
                if (OprId == 0) return null;

                var opr = dbAccess.GetOperation(OprId);
                return dbAccess.GetFirm(opr.FirmId);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                //PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void CheckLicenseCommand()
        {
            IsButtonEnable = false;
            _executedComputer = 0;
            NotificationIsVisible = false;
            ProgressBar = Visibility.Visible;
            TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            List<ComputerModel> computers = new List<ComputerModel>();

            System.Action DoInBackground = new System.Action(() =>
            {
                try
                {


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
                computers = computers.DistinctBy(p => p.Ip).ToList();
                Computers = new ObservableCollection<ComputerModel>(computers);
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
            Computers = new ObservableCollection<ComputerModel>(dbAccess.ListComputer(Firm.Id).ConvertAll(x => new ComputerModel { Id = x.Id, Ip = x.Ip, IsComputer = x.IsComputer, IsRootMachine = x.IsRootMachine, IsVisible = x.IsVisible, Name = x.Name, PyshicalAddress = x.PyshicalAddress, FirmId = x.FirmId, Type = x.Type, InsertDate = x.InsertDate }));
            NotificationContent = "Success";
            NotificationIsVisible = true;
            ProgressBar = Visibility.Hidden;
            IsButtonEnable = true;
        }
        public void AddItemCommand()
        {
            Computers = Computers;
            if (Computers == null) Computers = new ObservableCollection<ComputerModel>();
            Computers.Add(new ComputerModel());
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
    }
}

