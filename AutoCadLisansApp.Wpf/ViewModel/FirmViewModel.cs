﻿using AutoCadLisansKontrol.DAL;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MaterialDesignDemo.Domain
{
    public class FirmViewModel : INotifyPropertyChanged
    {




        private string _notificationContent;
        private bool _notificationIsVisible;
        public string NotificationContent { get { return _notificationContent; } set { _notificationContent = value; OnPropertyChanged("NotificationContent"); } }
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }


        DataAccess dbAccess = new DataAccess();
        private ObservableCollection<AutoCadLisansKontrol.DAL.Firm> _firm;
        public ICommand DeleteClicked { get; set; }
        public ICommand SaveClicked { get; set; }
        public ICommand AddItemClicked { get; set; }
        public ICommand RefreshClicked { get; set; }

        public ObservableCollection<AutoCadLisansKontrol.DAL.Firm> Firm
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

        public void SaveCommand()
        {
            NotificationIsVisible = false;
            try
            {
                foreach (var item in Firm)
                {
                    dbAccess.UpsertFirm(item);
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
            Firm.Add(new AutoCadLisansKontrol.DAL.Firm());
        }
        public void RefreshCommand()
        {
            NotificationIsVisible = false;
            Firm = new ObservableCollection<AutoCadLisansKontrol.DAL.Firm>(dbAccess.ListFirm());
            NotificationIsVisible = true;
            NotificationContent = "Success";
        }


    }
}
