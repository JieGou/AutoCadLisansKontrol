using AutoCadLisansKontrol.DAL;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MaterialDesignDemo.Domain
{
    public class FirmViewModel : INotifyPropertyChanged
    {
        DataAccess dbAccess = new DataAccess();
        private ObservableCollection<AutoCadLisansKontrol.DAL.Firm> _firm;
        public ICommand SaveClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand RefreshClicked { get; set; }
        
        public ObservableCollection<AutoCadLisansKontrol.DAL.Firm> Firm {
            get { return _firm; }
            set
            {
                _firm = value;
                OnPropertyChanged("Firm");
            } }
        
        public FirmViewModel()
        {

            RefreshClicked = new DelegateCommand(RefreshCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            SaveClicked = new DelegateCommand(SaveCommand);
            Firm = new ObservableCollection<AutoCadLisansKontrol.DAL.Firm>(dbAccess.ListFirm());
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveCommand() {
            foreach(var item in Firm)
            { 
                dbAccess.UpsertFirm(item);
            }
            RefreshCommand();
        }
        public void AddItemCommand()
        {
            Firm.Add(new AutoCadLisansKontrol.DAL.Firm());
        }
        public void RefreshCommand()
        {
           Firm= new ObservableCollection<AutoCadLisansKontrol.DAL.Firm>(dbAccess.ListFirm());
        }
    }
}
