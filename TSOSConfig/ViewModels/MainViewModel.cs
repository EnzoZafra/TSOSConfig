using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using TSOSConfig.HelperClasses;
using TSOSConfig.Models;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace TSOSConfig.ViewModels
{
    class MainViewModel : Observable
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        public ICommand SaveCommand { get; private set; }
        public ICommand CreateNewCommand { get; private set; }
        public FileModel File { get { return Get<FileModel>(); } set { Set(value); } }

        public MainViewModel()
        {
            CreateNewCommand = new ActionCommand(CreateNew, () => true);
            SaveCommand = new ActionCommand(Save, () => true);
        }

        private void CreateNew()
        {
            ConfigureViewModel.Instance.DatabaseList = new ObservableCollection<DatabaseModel>();
            ConfigureViewModel.Instance.Database = new DatabaseModel();
            ConfigureViewModel.Instance.NewFile = true;
            ConfigureViewModel.Instance.Configuration = new ConfigurationModel();
        }

        private void Save()
        {
            string xmlstring = UpdatePreview();
            XmlDocument xmldocument = GetXmlDocument(xmlstring);
            SaveDocument(xmldocument);
            ConfigureViewModel.Instance.NewFile = false;
            Refresh();
        }

        public void Refresh()
        {
            ConfigureViewModel.Instance.NewFile = false;
            ConfigureViewModel.Instance.DatabaseList = new ObservableCollection<DatabaseModel>();
            ConfigureViewModel.Instance.Database = new DatabaseModel();
            ConfigureViewModel.Instance.Configuration = new ConfigurationModel();
            File = new FileModel();
        }

        public string UpdatePreview()
        {
            // For testing
            File = new FileModel
            {
                Database = ConfigureViewModel.Instance.Database,
                Configuration = ConfigureViewModel.Instance.Configuration
            };

            var preview = new StringBuilder();
            preview.Append("<TSOS>");
            preview.Append("<CONFIGURATION RecurringTime=\"");
            preview.Append(File.Configuration.RecurringTime)
                .Append("\" MailServer=\"")
                .Append(File.Configuration.MailServer)
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
            System.IO.TextWriter systemwriter = new System.IO.StringWriter(builder);
            var xmlwriter = new XmlTextWriter(systemwriter) { Formatting = Formatting.Indented };
            xmlDocument.Save(xmlwriter);
            xmlwriter.Close();
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
            }
            return document;
        }

        private static string GetDatabaseStrings(IEnumerable<DatabaseModel> databaselist)
        {
            var stringbuilder = new StringBuilder();
            foreach (DatabaseModel database in databaselist)
            {
                stringbuilder.Append("<DATABASE Name = \"").Append(database.DatabaseName).Append("\" ");
                stringbuilder.Append("Customer=\"").Append(database.CustomerName).Append("\"/> ");
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
    }
}
