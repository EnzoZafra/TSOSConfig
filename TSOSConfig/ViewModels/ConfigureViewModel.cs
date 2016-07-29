using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using TSOSConfig.HelperClasses;
using TSOSConfig.Models;

namespace TSOSConfig.ViewModels
{
    class ConfigureViewModel : Observable
    {
        public static ConfigureViewModel Instance { get; } = new ConfigureViewModel();

        public ConfigurationModel Configuration
        {
            get { return Get<ConfigurationModel>(); }
            set { Set(value); }
        }

        public DatabaseModel Database
        {
            get { return Get<DatabaseModel>(); }
            set { Set(value); }
        }

        public ObservableCollection<DatabaseModel> DatabaseList
        {
            get { return Get<ObservableCollection<DatabaseModel>>(); }
            set { Set(value); }
        }

        public DatabaseModel SelectedDatabase
        {
            get { return Get<DatabaseModel>(); }
            set { Set(value); }
        }

        public bool NewFile
        {
            get { return Get<bool>(); }
            set { Set(value); }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand ClearCommand { get; private set; }

        public ConfigureViewModel()
        {
            AddCommand = new RelayCommand(Add, () => true);
            DeleteCommand = new RelayCommand(Delete, () => true);
            ClearCommand = new RelayCommand(Clear, () => true);
        }

        private void Add()
        {
            if (Database != null)
            {
                DatabaseList.Add(Database);
            }

            MainViewModel.Instance.UpdatePreview();
            Database = new DatabaseModel();
        }

        private void Delete()
        {
            if (SelectedDatabase != null)
            {
                DatabaseList.Remove(SelectedDatabase);
                MainViewModel.Instance.UpdatePreview();
            }
        }

        private void Clear()
        {
            DatabaseList = new ObservableCollection<DatabaseModel>();
            MainViewModel.Instance.UpdatePreview();
        }

        // Returns true if one of the fields have a value.
        public bool HasEntry()
        {
            return !(DatabaseList.Count == 0 && string.IsNullOrWhiteSpace(Configuration.MailServer)
                    && Configuration.RecurringTime == 0 && string.IsNullOrWhiteSpace(Database.CustomerName)
                    && string.IsNullOrWhiteSpace(Database.DatabaseName));
        }
    }
}
