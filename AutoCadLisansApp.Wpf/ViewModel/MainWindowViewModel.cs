using System.Linq;
using System.Windows.Controls;
using MaterialDesignDemo;
using MaterialDesignDemo.Domain;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace MaterialDesignColors.WpfExample.Domain
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _selectedIndex=0;
        public int SelectedIndex { get { return _selectedIndex; } set { _selectedIndex = value; OnPropertyChanged("SelectedIndex"); } }
        private ObservableCollection<DemoItem> _demoitems;
        private DemoItem _demoitem;
        public MainWindowViewModel()
        {
            DemoItems = new ObservableCollection<DemoItem>
            {
                new DemoItem("Home", new Home()),
                //new DemoItem("Operation", new Operation { DataContext = new OperationViewModel()}),
                new DemoItem("Firma", new Firm { DataContext = new FirmViewModel()}),
                //new DemoItem("Bilgisayar", new Computer { DataContext = new ComputerViewModel()}),
         
                //new DemoItem("Palette", new PaletteSelector { DataContext = new PaletteSelectorViewModel() }),
                //new DemoItem("Buttons & Toggles", new Buttons()),
                //new DemoItem("Fields", new TextFields()),
                //new DemoItem("Pickers", new Pickers { DataContext = new PickersViewModel()}),
                //new DemoItem("Sliders", new Sliders()),
                //new DemoItem("Chips", new Chips()),
                //new DemoItem("Typography", new Typography()),
                //new DemoItem("Cards", new Cards()),
                //new DemoItem("Icon Pack", new IconPack { DataContext = new IconPackViewModel() }),
                //new DemoItem("Colour Zones", new ColorZones()),
                //new DemoItem("Lists", new Lists { DataContext = new ComputerViewModel()}),
                //new DemoItem("Trees", new Trees { DataContext = new TreesViewModel() }),
                //new DemoItem("Expander", new Expander()),
                //new DemoItem("Group Boxes", new GroupBoxes()),
                //new DemoItem("Menus & Tool Bars", new MenusAndToolBars()),
                //new DemoItem("Progress Indicators", new Progress()),
                //new DemoItem("Dialogs", new Dialogs { DataContext = new DialogsViewModel()}),
                //new DemoItem("Drawer", new Drawers()),
                //new DemoItem("Snackbar", new Snackbars()),
                //new DemoItem("Transitions", new Transitions()),
                //new DemoItem("Shadows", new Shadows()),
            };
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<DemoItem> DemoItems
        {
            get
            {
                return _demoitems;
            }
            set
            {
                _demoitems = value;
                OnPropertyChanged("DemoItems");
            }
        }
        public DemoItem DemoItem
        {
            get
            {
                return _demoitem;
            }
            set
            {
                _demoitem = value;
                OnPropertyChanged("DemoItem");
            }
        }
    }
}