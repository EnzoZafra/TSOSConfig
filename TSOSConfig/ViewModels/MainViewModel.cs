using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using TSOSConfig.Models;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace TSOSConfig.ViewModels
{
    class MainViewModel : Observable
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        public Window Owner { get { return Get<Window>(); } set { Set(value); } }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CreateNewCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public FileModel File { get { return Get<FileModel>(); } set { Set(value); } }

        public MainViewModel()
        {
            CreateNew();
            CreateNewCommand = new RelayCommand(CreateNew, () => true);
            SaveCommand = new RelayCommand(Save, CanSave);
            LoadCommand = new RelayCommand(Load, () => true);
        }

        private void CreateNew()
        {
            ConfigureViewModel.Instance.NewFile = false;
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
            }
        }

        private void ParseXML(string rawxml)
        {
            File.RawXML = rawxml;
            if (rawxml == null) { return; }
            rawxml = rawxml.Replace("\r\n", " ");

            // Need to get a shorter regular expession.
            var match = Regex.Match(rawxml, 
                "(?:[\\s\\S]*?(?=RecurringTime))(?\'configuration\'[\\s\\S]*?(?=\\ \\/>))(?:[\\s\\S]*?(?=<DATABASE\\ Name))(?\'databases\'[\\s\\S]*?(?=\\ \\ \\ <\\/DATABASES>))");
            if (!match.Success)
            {
                return;
            }
            string configuration = match.Groups["configuration"].Value;
            string databases = match.Groups["databases"].Value;             
            
            // Parse configuration object
            string regex1 =
                @"(?:RecurringTime=)(?'recurringtime'[\s\S]*?(?=\ ))(?:\ MailServer=)(?'mailserver'.*$)";
            var splitconfiguration = Regex.Match(configuration.Replace("\"",string.Empty), regex1);

            File.Configuration.RecurringTime = int.Parse(splitconfiguration.Groups["recurringtime"].Value);
            File.Configuration.MailServer = splitconfiguration.Groups["mailserver"].Value;
            // Parse database objects

            foreach (Match element in Regex.Matches(databases, @"(?'match'[\s\S]*?(?=\/>))(\/>)"))
            {
                var databasematch = element.Groups["match"].Value;
                string regex2 = @"(?:<DATABASE\ Name=)(?'dbname'[\s\S]*?(?=\ ))(?:\ Customer=)(?'custname'.*$)";
                Match match2;

                if ((match2 = Regex.Match(databasematch.Replace("\"", string.Empty), regex2)).Success)
                {
                    var databaseentry = new DatabaseModel()
                    {
                        CustomerName = match2.Groups["custname"].Value,
                        DatabaseName = match2.Groups["dbname"].Value
                    };
                    ConfigureViewModel.Instance.DatabaseList.Add(databaseentry);
                }
            }
            ConfigureViewModel.Instance.NewFile = true;
        }

        public string UpdatePreview()
        {
            File.Configuration = ConfigureViewModel.Instance.Configuration;
            File.Database = ConfigureViewModel.Instance.Database;
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
                ConfigureViewModel.Instance.NewFile = false;
                CreateNew();
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
    }
}
