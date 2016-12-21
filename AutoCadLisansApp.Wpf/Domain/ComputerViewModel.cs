using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoCadLisansKontrol.Controller;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using AutoCadLisansKontrol.DAL;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections;
using System.Threading;

namespace MaterialDesignColors.WpfExample.Domain
{
    public class ComputerViewModel : INotifyPropertyChanged
    {

        CheckAutoCadLicense license = new CheckAutoCadLicense();
        private int _totalComputer = 0;
        public int TotalComputer { get { return _totalComputer; } set { _totalComputer = value; OnPropertyChanged("TotalComputer"); } }
        private int _executedComputer = 0;
        public int ExecutedComputer { get { return _executedComputer; } set { _executedComputer = value; OnPropertyChanged("ExecutedComputer"); } }

        DataAccess dbAccess = new DataAccess();
        public ICommand buttonClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand LoadDbClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        private int operationId;

        private PagingCollectionView _cview;
        public PagingCollectionView Cview
        {
            get { return _cview; }
            set
            {
                _cview = value;
                OnPropertyChanged("Cview");
            }
        }

        private ObservableCollection<AutoCadLisansKontrol.DAL.Computer> _computers;
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

        public ComputerViewModel()
        {
            buttonClicked = new DelegateCommand(GenerateCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            LoadDbClicked = new DelegateCommand(LoadComputerFromDb);
            SaveClicked = new DelegateCommand(SaveCommand);
        }

        public ComputerViewModel(int opr)
        {
            buttonClicked = new DelegateCommand(GenerateCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            LoadDbClicked = new DelegateCommand(LoadComputerFromDb);
            SaveClicked = new DelegateCommand(SaveCommand);
            operationId = opr;
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

        private void SelectAll(bool select, IEnumerable<AutoCadLisansKontrol.DAL.Computer> models)
        {
            foreach (var model in models)
            {
                model.IsRootMachine = select;
            }
        }

        private ObservableCollection<AutoCadLisansKontrol.DAL.Computer> GenerateDataFromNetwork()
        {

            var list = license.GetComputerInfoOnNetwork();
            return new ObservableCollection<AutoCadLisansKontrol.DAL.Computer>(list);
        }

        public ObservableCollection<AutoCadLisansKontrol.DAL.Computer> Computers
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
                if (Operation == null) return null;
                return dbAccess.GetFirm(Operation.FirmId);
            }
        }
        public Operation Operation
        {
            get
            {
                return dbAccess.GetOperation(operationId);
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
            ProgressBar = Visibility.Visible;
            UpdateData();


        }
        private async void UpdateData()
        {
            //I assume BitmapFromUri is the slow step.

            //Now that we have our bitmap, now go to the main thread.

            Computers = GenerateDataFromNetwork();
            TotalComputer = Computers.Count;


            foreach (var item in Computers)
            {
                await Task.Run(() => new CheckAutoCadLicense().ExecuteComputer(item));


                ExecutedComputer++;
            }

            ProgressBar = Visibility.Hidden;


            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
            {
                foreach (var item in Computers)
                {
                    item.Ip = item.Ip;
                }
            }));


        }
        public void RefreshComputerList()
        {

        }

        public void LoadComputerFromDb()
        {
            Computers = new ObservableCollection<AutoCadLisansKontrol.DAL.Computer>(dbAccess.ListComputer(Operation.FirmId));
        }
        public void AddItemCommand()
        {
            Computers = Computers;
            if (Computers == null) Computers = new ObservableCollection<AutoCadLisansKontrol.DAL.Computer>();
            Computers.Add(new AutoCadLisansKontrol.DAL.Computer());
        }

        public void SaveCommand()
        {
            foreach (var item in Computers)
            {
                dbAccess.UpsertComputer(item);
            }

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

