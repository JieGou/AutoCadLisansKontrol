using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoCadLisansKontrol.Controller;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using AutoCadLisansKontrol.DAL;

namespace MaterialDesignColors.WpfExample.Domain
{
    public class ComputerViewModel : INotifyPropertyChanged
    {
        DataAccess dbAccess = new DataAccess();
        public ICommand buttonClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand LoadDbClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        private int operationId;
        
        private ObservableCollection<AutoCadLisansKontrol.DAL.Computer> _computers;
        private bool? _isAllItems3Selected;
        

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

        private static ObservableCollection<AutoCadLisansKontrol.DAL.Computer> GenerateDataFromNetwork()
        {

            CheckAutoCadLicense license = new CheckAutoCadLicense();

            return new ObservableCollection<AutoCadLisansKontrol.DAL.Computer>(license.GetComputerInfoOnNetwork());
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
        public void GenerateCommand() {
            Computers = GenerateDataFromNetwork();
        }

        public void LoadComputerFromDb()
        {
            Computers = new ObservableCollection<AutoCadLisansKontrol.DAL.Computer>(dbAccess.ListComputer(Operation.FirmId));
        }
        public void AddItemCommand()
        {
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


        public Visibility SetVisibility(bool isonline) {

            switch (isonline)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Hidden;
                default:
                    return Visibility.Hidden;
            }
        }
        public IEnumerable<string> Foods
        {
            get
            {
                yield return "Burger";
                yield return "Fries";
                yield return "Shake";
                yield return "Lettuce";
            }
        }
    }
}

