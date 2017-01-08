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
using MaterialDesignDemo.ViewModel;
using AutoCadLisansKontrol.Controller;
using System;
using System.Globalization;

namespace MaterialDesignColors.WpfExample.Domain
{
    public class ComputerViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private bool _isButtonEnable = true;
        public bool IsButtonEnable { get { return _isButtonEnable; } set { _isButtonEnable = value; OnPropertyChanged("IsButtonEnable"); } }
        private string _notificationContent;
        private bool _notificationIsVisible;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }

        private int _totalComputer = 0;
        public int TotalComputer { get { return _totalComputer; } set { _totalComputer = value; OnPropertyChanged("TotalComputer"); } }


        private int _executedComputer = 0;
        public int ExecutedComputer { get { return _executedComputer; } set { _executedComputer = value; OnPropertyChanged("ExecutedComputer"); } }

        public ICommand buttonClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand LoadDbClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        private int FirmId;



        private ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.Computer> _computers;
        private bool? _isAllItems3Selected;
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

        public ComputerViewModel(int firmId)
        {
            buttonClicked = new DelegateCommand(GenerateCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            LoadDbClicked = new DelegateCommand(LoadComputerFromDb);
            SaveClicked = new DelegateCommand(SaveCommand);
            FirmId = firmId;
            LoadComputerFromDb();
        }

        public bool? IsAllItems3Selected
        {
            get { return _isAllItems3Selected; }
            set
            {
                if (_isAllItems3Selected == value) return;

                _isAllItems3Selected = value;

                if (_isAllItems3Selected.HasValue)
                    SelectAll(_isAllItems3Selected.Value, Computers);

                OnPropertyChanged();
            }
        }

        private void SelectAll(bool select, IEnumerable<MaterialDesignDemo.autocad.masterkey.ws.Computer> models)
        {
            foreach (var model in models)
            {
                model.IsRootMachine = select;
            }
        }



        public ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.Computer> Computers
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
        public MaterialDesignDemo.autocad.masterkey.ws.Firm Firm
        {
            get
            {
                if (FirmId == 0) return null;
                return client.GetFirm(FirmId);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                //PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void GenerateCommand()
        {
            IsButtonEnable = false;
            _executedComputer = 0;
            _totalComputer = 0;

            StartNotification();

            TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            List<MaterialDesignDemo.autocad.masterkey.ws.Computer> computers = new List<MaterialDesignDemo.autocad.masterkey.ws.Computer>();

            System.Action DoInBackground = new System.Action(() =>
            {
                try
                {
                    computers = GenerateDataFromNetwork();
                    TotalComputer = computers.Count;
                    foreach (var item in computers)
                    {
                        ComputerDetection.GetAdditionalInfo(item);
                        ExecutedComputer++;
                    }
                    ProgressBar = Visibility.Hidden;
                }
                catch (System.Exception ex)
                {
                    EndNotification(ex.Message);
                    return;
                }

            });

            System.Action DoOnUiThread = new System.Action(() =>
            {
                if (NotificationIsVisible == true)
                    return;
                computers = computers.DistinctBy(p => p.Ip).ToList();
                Computers = new ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.Computer>(computers);
                EndNotification("Success");
            });

            // start the background task
            var t1 = Task.Factory.StartNew(() => DoInBackground());
            // when t1 is done run t1..on the Ui thread.
            var t2 = t1.ContinueWith(t => DoOnUiThread(), _uiScheduler);
            //I assume BitmapFromUri is the slow step.

            //Now that we have our bitmap, now go to the main thread.


        }
        private List<MaterialDesignDemo.autocad.masterkey.ws.Computer> GenerateDataFromNetwork()
        {

            List<MaterialDesignDemo.autocad.masterkey.ws.Computer> list = ComputerDetection.Execute();
            return list;
        }

        public void LoadComputerFromDb()
        {
            try
            {
                StartNotification();
                Computers = new ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.Computer>(client.ListComputer(FirmId).ToList());
                EndNotification("Success");
            }
            catch (System.Exception ex)
            {
                EndNotification(ex.Message);
            }

            _executedComputer = Computers.Count;
            _totalComputer = Computers.Count;
        }
        public void AddItemCommand()
        {

            Computers = Computers;
            if (Computers == null) Computers = new ObservableCollection<MaterialDesignDemo.autocad.masterkey.ws.Computer>();
            Computers.Add(new MaterialDesignDemo.autocad.masterkey.ws.Computer());

            _executedComputer = Computers.Count;
            _totalComputer = Computers.Count;
        }

        public void SaveCommand()
        {
            try
            {
                StartNotification();

                client.DeleteAllComputerBaseFormid(FirmId);

                foreach (var item in Computers)
                {
                    item.FirmId = FirmId;

                    client.UpsertComputer(item);
                }
                LoadComputerFromDb();
                EndNotification("Success");
            }
            catch (System.Exception ex)
            {
                EndNotification(ex.Message);
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
        public Visibility IsVisible(bool isVisible)
        {
            
        }
    }

    public class PagingCollectionView : CollectionView
    {
        private readonly IList _innerList;
        private readonly int _itemsPerPage;

        private int _currentPage = 1;

        public PagingCollectionView(IList innerList, int itemsPerPage)
            : base(innerList)
        {
            this._innerList = innerList;
            this._itemsPerPage = itemsPerPage;
        }

        public override int Count
        {
            get
            {
                if (this._innerList.Count == 0) return 0;
                if (this._currentPage < this.PageCount) // page 1..n-1
                {
                    return this._itemsPerPage;
                }
                else // page n
                {
                    var itemsLeft = this._innerList.Count % this._itemsPerPage;
                    if (0 == itemsLeft)
                    {
                        return this._itemsPerPage; // exactly itemsPerPage left
                    }
                    else
                    {
                        // return the remaining items
                        return itemsLeft;
                    }
                }
            }
        }

        public int CurrentPage
        {
            get { return this._currentPage; }
            set
            {
                this._currentPage = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
            }
        }

        public int ItemsPerPage { get { return this._itemsPerPage; } }

        public int PageCount
        {
            get
            {
                return (this._innerList.Count + this._itemsPerPage - 1)
                    / this._itemsPerPage;
            }
        }

        private int EndIndex
        {
            get
            {
                var end = this._currentPage * this._itemsPerPage - 1;
                return (end > this._innerList.Count) ? this._innerList.Count : end;
            }
        }

        private int StartIndex
        {
            get { return (this._currentPage - 1) * this._itemsPerPage; }
        }

        public override object GetItemAt(int index)
        {
            var offset = index % (this._itemsPerPage);
            return this._innerList[this.StartIndex + offset];
        }

        public void MoveToNextPage()
        {
            if (this._currentPage < this.PageCount)
            {
                this.CurrentPage += 1;
            }
            this.Refresh();
        }

        public void MoveToPreviousPage()
        {
            if (this._currentPage > 1)
            {
                this.CurrentPage -= 1;
            }
            this.Refresh();
        }
    }
}

