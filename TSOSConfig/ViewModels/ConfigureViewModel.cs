using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TSOSConfig.HelperClasses;
using TSOSConfig.Models;

namespace TSOSConfig.ViewModels
{
    class ConfigureViewModel : Observable
    {
        public static ConfigureViewModel Instance { get; } = new ConfigureViewModel();

        public ConfigurationModel Configuration {get { return Get<ConfigurationModel>(); } set {Set(value); } }
        public DatabaseModel Database { get { return Get<DatabaseModel>(); } set { Set(value); } }
        public ObservableCollection<DatabaseModel> DatabaseList {get { return Get<ObservableCollection<DatabaseModel>>(); } set {Set(value); } }
        public DatabaseModel SelectedDatabase { get { return Get<DatabaseModel>(); } set { Set(value); } }
        public bool NewFile { get { return Get<bool>(); } set { Set(value); } }

        public ICommand AddCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        public ConfigureViewModel()
        {
            AddCommand = new ActionCommand(Add, () => true);
            DeleteCommand = new ActionCommand(Delete, () => true);
            ClearCommand = new ActionCommand(Clear, () => true);
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


        //public event PropertyChangedEventHandler PropertyChanged;

        //private void RaisePropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
