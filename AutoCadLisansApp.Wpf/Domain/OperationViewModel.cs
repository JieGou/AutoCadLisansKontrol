using AutoCadLisansKontrol.DAL;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MaterialDesignDemo.Domain
{
    public class OperationViewModel : INotifyPropertyChanged
    {
        DataAccess dbAccess = new DataAccess();
        private ObservableCollection<AutoCadLisansKontrol.DAL.Operation> _opr;
        public ICommand SaveClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand RefreshClicked { get; set; }
        private AutoCadLisansKontrol.DAL.Firm _selectedfirm;
        public AutoCadLisansKontrol.DAL.Firm SelectedFirm { get { return _selectedfirm; } set {
                _selectedfirm = value;
                OnPropertyChanged("SelectedFirm");
            } }
        public ObservableCollection<AutoCadLisansKontrol.DAL.Firm> Firm { get { return new ObservableCollection<AutoCadLisansKontrol.DAL.Firm>(dbAccess.ListFirm()); } }
        public ObservableCollection<AutoCadLisansKontrol.DAL.Operation> Operation
        {
            get { return _opr; }
            set
            {
                _opr = value;
                OnPropertyChanged("Operation");
            }
        }

        public OperationViewModel()
        {
            RefreshClicked = new DelegateCommand(RefreshCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            SaveClicked = new DelegateCommand(SaveCommand);
            Operation = new ObservableCollection<AutoCadLisansKontrol.DAL.Operation>(dbAccess.ListOperation());
        }
        public OperationViewModel(int firmid)
        {
            SelectedFirm = dbAccess.GetFirm(firmid);
            RefreshClicked = new DelegateCommand(RefreshCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            SaveClicked = new DelegateCommand(SaveCommand);
            Operation = new ObservableCollection<AutoCadLisansKontrol.DAL.Operation>(dbAccess.ListOperation(firmid));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveCommand()
        {
            foreach (var item in Operation)
            {
                dbAccess.UpsertOperation(item);
            }
            RefreshCommand();
        }
        public void AddItemCommand()
        {
            Operation.Add(new AutoCadLisansKontrol.DAL.Operation());
        }
        public void RefreshCommand()
        {
            SelectedFirm = new AutoCadLisansKontrol.DAL.Firm();
            Operation = new ObservableCollection<AutoCadLisansKontrol.DAL.Operation>(dbAccess.ListOperation());
        }
    }
}
