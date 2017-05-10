
using LicenseController.Model;
using MaterialDesignDemo.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MaterialDesignDemo.Domain
{
    public class FirmViewModel : BaseViewModel, INotifyPropertyChanged
    {




        private string _notificationContent;
        private bool _notificationIsVisible;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }



        private ObservableCollection<LicenseController.autocad.masterkey.ws.FirmDTO> _firm;
        public ICommand DeleteClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand RefreshClicked { get; set; }

        public ObservableCollection<LicenseController.autocad.masterkey.ws.FirmDTO> Firm
        {
            get { return _firm; }
            set
            {
                _firm = value;
                OnPropertyChanged("Firm");
            }
        }

        public FirmViewModel()
        {
            try
            {

                RefreshClicked = new DelegateCommand(RefreshCommand);
                AddItemClicked = new DelegateCommand(AddItemCommand);
                SaveClicked = new DelegateCommand(SaveCommand);
                var firms =client.FirmList(GlobalVariable.getInstance().user.Id);
                Firm = new ObservableCollection<LicenseController.autocad.masterkey.ws.FirmDTO>(firms);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                foreach (var item in Firm)
                {
                    item.UserId = GlobalVariable.getInstance().user.Id;
                    client.FirmUpsert(item);
                }
                RefreshCommand();
            }
            catch (System.Exception ex)
            {
                NotificationIsVisible = true;
                NotificationContent = ex.Message;
                return;
            }
            NotificationIsVisible = true;
            NotificationContent = "Success";
        }
        public void AddItemCommand()
        {
            Firm.Add(new LicenseController.autocad.masterkey.ws.FirmDTO());
        }
        public void RefreshCommand()
        {
            NotificationIsVisible = false;
            Firm = new ObservableCollection<LicenseController.autocad.masterkey.ws.FirmDTO>(client.FirmList(GlobalVariable.getInstance().user.Id));
            NotificationIsVisible = true;
            NotificationContent = "Success";
        }


    }

}
