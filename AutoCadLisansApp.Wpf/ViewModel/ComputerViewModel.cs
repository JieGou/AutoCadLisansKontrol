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
using MaterialDesignDemo.Model;
using System.Threading;

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
        public ICommand CancelClicked { get; set; }
        private int FirmId;



        private ObservableCollection<ComputerModel> _computers;
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
            CancelClicked = new DelegateCommand(CancelCommand);
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

        private void SelectAll(bool select, IEnumerable<ComputerModel> models)
        {
            foreach (var model in models)
            {
                model.IsRootMachine = select;
            }
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
        CancellationTokenSource Canceller = new CancellationTokenSource();
        public void GenerateCommand()
        {
            IsButtonEnable = false;
            _executedComputer = 0;
            _totalComputer = 0;

            StartNotification();

            TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            List<ComputerModel> computers = new List<ComputerModel>();
            Computers = GenerateDataFromNetwork();

            System.Action DoInBackground = new System.Action(() =>
            {
                try
                {
                    TotalComputer = Computers.Count;
                    foreach (var item in Computers)
                    {
                        System.Action ChildDoInBackground = new System.Action(() =>
                        {
                            ComputerDetection.GetAdditionalInfo(item);
                        });

                        System.Action ChildDoOnUiThread = new System.Action(() =>
                        {
                            item.IsProgress = false;

                            ExecutedComputer++;

                            if (ExecutedComputer == TotalComputer)
                            {
                                Computers = new ObservableCollection<ComputerModel>(Computers.DistinctBy(p => p.Ip).ToList());
                                Computers = ComputerDetection.FilterComputer(Computers);
                                EndNotification("Network searching has finished!!");
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

            //Now that we have our bitmap, now go to the main thread.


        }
        private ObservableCollection<ComputerModel> GenerateDataFromNetwork()
        {

            List<ComputerModel> list = ComputerDetection.Execute();
            return new ObservableCollection<ComputerModel>(list);
        }

        public void LoadComputerFromDb()
        {
            try
            {
                StartNotification();
                var computer = new ObservableCollection<ComputerModel>();

                TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                System.Action ChildDoInBackground = new System.Action(() =>
                {
                    computer = new ObservableCollection<ComputerModel>(client.ListComputer(FirmId).ToList().ConvertAll(x => new ComputerModel { FirmId = x.FirmId, Id = x.Id, InsertDate = x.InsertDate, Ip = x.Ip, IsComputer = x.IsComputer, IsRootMachine = x.IsRootMachine, IsVisible = x.IsVisible, Name = x.Name, PyshicalAddress = x.PyshicalAddress, Type = x.Type }));
                    _executedComputer = computer.Count;
                    _totalComputer = computer.Count;
                    EndNotification("");
                });

                System.Action ChildDoOnUiThread = new System.Action(() =>
                {
                    Computers = computer;
                });

                var t1 = Task.Factory.StartNew(() => ChildDoInBackground());
                // when t1 is done run t1..on the Ui thread.
                var t2 = t1.ContinueWith(t => ChildDoOnUiThread(), _uiScheduler);
            }
            catch (System.Exception ex)
            {
                EndNotification(ex.Message);
            }


        }
        public void AddItemCommand()
        {

            Computers = Computers;
            if (Computers == null) Computers = new ObservableCollection<ComputerModel>();
            Computers.Add(new ComputerModel());

            _executedComputer = Computers.Count;
            _totalComputer = Computers.Count;
        }

        public void SaveCommand()
        {
            try
            {
                StartNotification();

                System.Action ChildDoInBackground = new System.Action(() =>
                {
                    client.DeleteAllComputerBaseFormid(FirmId);
                    var counter = Computers.Count;
                    foreach (var item in Computers)
                    {
                        counter--;
                        item.FirmId = FirmId;

                        client.UpsertComputer(new MaterialDesignDemo.autocad.masterkey.ws.Computer { FirmId = item.FirmId, Id = item.Id, InsertDate = item.InsertDate, Ip = item.Ip, IsComputer = item.IsComputer, IsRootMachine = item.IsRootMachine, IsVisible = item.IsVisible, Name = item.Name, PyshicalAddress = item.PyshicalAddress, Type = item.Type });
                        if (counter == 0)
                        {
                            LoadComputerFromDb();
                            EndNotification("Computers is saved");
                        }
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
            catch (System.Exception ex)
            {
                EndNotification(ex.Message);
            }
        }
        public void CancelCommand()
        {
            EndNotification("Thread Aborted");
            Canceller.Cancel();
            LoadComputerFromDb();
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


