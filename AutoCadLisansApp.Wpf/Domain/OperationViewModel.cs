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

        public ICommand RunComputerDetectClicked { get; set; }

        public ICommand RunAutoCadDetectClicked { get; set; }

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
            RunComputerDetectClicked = new DelegateCommand(RunComputerDetectCommand);
            RunAutoCadDetectClicked = new DelegateCommand(RunAutoCadDetectCommand);
            RefreshClicked = new DelegateCommand(RefreshCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            SaveClicked = new DelegateCommand(SaveCommand);
            Operation = new ObservableCollection<AutoCadLisansKontrol.DAL.Operation>(dbAccess.ListOperation());
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
            Operation = new ObservableCollection<AutoCadLisansKontrol.DAL.Operation>(dbAccess.ListOperation());
        }

        public void RunComputerDetectCommand()
        {
            
        }

        public void RunAutoCadDetectCommand()
        {

        }
    }
}
