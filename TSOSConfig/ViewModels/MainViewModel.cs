using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;
using System.Xml.Linq;
using TSOSConfig.HelperClasses;
using TSOSConfig.Models;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace TSOSConfig.ViewModels
{
    class MainViewModel : Observable
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        public Window MainWindow => Application.Current.MainWindow;
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CreateNewCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public FileModel File { get { return Get<FileModel>(); } set { Set(value); } }
        public bool HasSaved;
        public bool Initializing;

        public MainViewModel()
        {
            Initializing = true;
            HasSaved = true;
            ConfigureViewModel.Instance.NewFile = false;
            SetObjects();

            CreateNewCommand = new RelayCommand(CreateNew, () => true);
            SaveCommand = new RelayCommand(Save, CanSave);
            LoadCommand = new RelayCommand(Load, () => true);
        }

        private void CreateNew()
        {
            HasSaved = false;
            if (Initializing)
            {
                ConfigureViewModel.Instance.NewFile = true;
                Initializing = false;
                return;
            }
            if (!HasSaved && ConfigureViewModel.Instance.HasEntry())
            {
                MessageBoxResult messageresult = MessageBox.Show(MainWindow,"You will lose unsaved progress. Continue?", 
                                                            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageresult != MessageBoxResult.Yes) return;
                SetObjects();
            }
        }

        private void SetObjects()
        {
            ConfigureViewModel.Instance.DatabaseList = new ObservableCollection<DatabaseModel>();
            ConfigureViewModel.Instance.Database = new DatabaseModel();
            ConfigureViewModel.Instance.Configuration = new ConfigurationModel();

            File = new FileModel()
            {
                Database = ConfigureViewModel.Instance.Database,
                Configuration = ConfigureViewModel.Instance.Configuration,
                RawXML = string.Empty
            };
        }

        private void Save()
        {
            string xmlstring = UpdatePreview();
            XmlDocument xmldocument = GetXmlDocument(xmlstring);
            SaveDocument(xmldocument);
        }

        private bool CanSave()
        {
            return File.RawXML != string.Empty;
        }

        private void Load()
        {
            Initializing = false;
            if (!HasSaved && ConfigureViewModel.Instance.HasEntry())
            {
                MessageBoxResult messageresult = MessageBox.Show(MainWindow, "You will lose unsaved progress. Continue?",
                                                            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageresult != MessageBoxResult.Yes) return;
            }
            ConfigureViewModel.Instance.DatabaseList = new ObservableCollection<DatabaseModel>();
            var openFileDialog = new OpenFileDialog()
                {
                    Filter = "XML files (*.xml)|*.xml",
                    FilterIndex = 0,
                    RestoreDirectory = true,
                    FileName = null,
                    Title = "Open XML file to be imported"
                };
            if (openFileDialog.ShowDialog() == true)
            {
                var streamReader = new System.IO.StreamReader(openFileDialog.FileName);
                string rawxml = streamReader.ReadToEnd();
                streamReader.Close();
                ParseXML(rawxml);
                HasSaved = false;
            }
        }

        private void ParseXML(string rawxml)
        {
            File.RawXML = rawxml;
            if (rawxml == null) { return; }
            var reader = new StringReader(rawxml);
            var xdoc = XDocument.Load(reader);

            var query = from results in xdoc.Descendants("CONFIGURATION")
                    select new
                    {
                        RecurringTime = results.Attribute("RecurringTime").Value,
                        MailServer = results.Attribute("MailServer").Value,
                        SQLConnectionString = results.Attribute("MySQLConnectionString").Value
                    };
            File.Configuration.RecurringTime = int.Parse(query.FirstOrDefault().RecurringTime);
            File.Configuration.MailServer = query.FirstOrDefault().MailServer;
            File.Configuration.MySQLConnectionString = query.FirstOrDefault().SQLConnectionString;
            foreach (XElement element in xdoc.Root
                                .Element("DATABASES")
                                .Elements("DATABASE"))
            {
                var databaseentry = new DatabaseModel()
                {
                    CustomerName = element.Attribute("Customer").Value,
                    DatabaseName = element.Attribute("Name").Value
                };
                ConfigureViewModel.Instance.DatabaseList.Add(databaseentry);

            }
            ConfigureViewModel.Instance.NewFile = true;
        }

        public string UpdatePreview()
        {
            File.Configuration = ConfigureViewModel.Instance.Configuration;
            File.Database = ConfigureViewModel.Instance.Database;
            var preview = new StringBuilder();
            preview.Append("<TSOS>");
            preview.Append("<CONFIGURATION RecurringTime=\"")
                .Append(File.Configuration.RecurringTime);
            preview.Append("\" MailServer=\"")
                .Append(File.Configuration.MailServer);
            preview.Append("\" MySQLConnectionString=\"")
                .Append(File.Configuration.MySQLConnectionString)
                .Append("\"/>");
            preview.Append("<DATABASES>");
            preview.Append(GetDatabaseStrings(ConfigureViewModel.Instance.DatabaseList));
            preview.Append("</DATABASES> ");
            preview.Append("</TSOS>");

            File.RawXML = XmlToString(GetXmlDocument(preview.ToString()));

            return File.RawXML;
        }

        protected string XmlToString(XmlDocument xmlDocument)
        {
            var builder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings
            { Indent = true, OmitXmlDeclaration = true };
            xmlDocument.Save(XmlWriter.Create(builder, xmlWriterSettings));
            return builder.ToString();
        }

        private static XmlDocument GetXmlDocument(string xmlString)
        {
            var document = new XmlDocument();
            document.LoadXml(xmlString);
            return document;
        }

        public XmlDocument SaveDocument(XmlDocument document)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = null,
                Title = "Save path of the file to be exported"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                document.Save(saveFileDialog.FileName);
                HasSaved = true;
                SetObjects();
                ConfigureViewModel.Instance.NewFile = false;
            }
            return document;
        }

        private static string GetDatabaseStrings(IEnumerable<DatabaseModel> databaselist)
        {
            var stringbuilder = new StringBuilder();
            foreach (DatabaseModel database in databaselist)
            {
                stringbuilder.Append("<DATABASE Name = \"").Append(database.DatabaseName).Append("\" ");
                stringbuilder.Append("Customer=\"").Append(database.CustomerName).Append("\"/>");
            }
            return stringbuilder.ToString();
        }

        public bool IsValidISO(string input)
        {
            var iso88591 = Encoding.GetEncoding("ISO-8859-1");

            byte[] bytes = iso88591.GetBytes(input);
            string result = iso88591.GetString(bytes);
            return string.Equals(input, result);
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (!HasSaved && ConfigureViewModel.Instance.HasEntry())
            {
                MessageBoxResult messageresult = MessageBox.Show(MainWindow, "You will lose unsaved progress. Continue?",
                                                            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (messageresult != MessageBoxResult.Yes) e.Cancel = true;
            }
        }
    }
}
