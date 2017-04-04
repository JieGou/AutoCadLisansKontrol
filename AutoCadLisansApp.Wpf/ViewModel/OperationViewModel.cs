
using MaterialDesignDemo.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MaterialDesignDemo.Domain
{
    public class OperationViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _notificationContent;
        private bool _notificationIsVisible;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }

        private ObservableCollection<LicenseController.autocad.masterkey.ws.Operation> _opr;
        public ICommand SaveClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand RefreshClicked { get; set; }
        private LicenseController.autocad.masterkey.ws.Firm _selectedfirm;
        public LicenseController.autocad.masterkey.ws.Firm SelectedFirm
        {
            get { return _selectedfirm; }
            set
            {
                _selectedfirm = value;
                OnPropertyChanged("SelectedFirm");
            }
        }
        public ObservableCollection<LicenseController.autocad.masterkey.ws.Firm> Firm { get { return new ObservableCollection<LicenseController.autocad.masterkey.ws.Firm>(client.ListFirm()); } }
        public ObservableCollection<LicenseController.autocad.masterkey.ws.Operation> Operation
        {
            get { return _opr; }
            set
            {
                _opr = value;
                OnPropertyChanged("Operation");
            }
        }
        public OperationViewModel(int firmid)
        {
            SelectedFirm = client.GetFirm(firmid);
            RefreshClicked = new DelegateCommand(RefreshCommand);
            AddItemClicked = new DelegateCommand(AddItemCommand);
            SaveClicked = new DelegateCommand(SaveCommand);
            var list = client.ListOperation(firmid);
            foreach (var item in list)
            {
                item.Firm = SelectedFirm;
            }
            Operation = new ObservableCollection<LicenseController.autocad.masterkey.ws.Operation>(list);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveCommand()
        {
            NotificationIsVisible = false;
            try
            {
                foreach (var item in Operation)
                {
                    item.FirmId = SelectedFirm.Id;
                    client.UpsertOperation(item);
                }
                RefreshCommand();
                NotificationIsVisible = true;
                NotificationContent = "Success";
            }
            catch (System.Exception ex)
            {
                NotificationIsVisible = true;
                NotificationContent = ex.Message;
            }
        }
        public void AddItemCommand()
        {
            Operation.Add(new LicenseController.autocad.masterkey.ws.Operation() { FirmId = SelectedFirm.Id,Firm=SelectedFirm });
        }
        public void RefreshCommand()
        {
            var list = client.ListOperation(SelectedFirm.Id);
            foreach (var item in list)
            {
                item.Firm = SelectedFirm;
            }
            Operation = new ObservableCollection<LicenseController.autocad.masterkey.ws.Operation>(list);
        }
    }
}
