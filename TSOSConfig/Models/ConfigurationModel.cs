namespace TSOSConfig.Models
{
    public class ConfigurationModel : Observable
    {
        public int RecurringTime { get { return Get<int>(); } set { Set(value); } }
        public string MailServer {get { return Get<string>(); } set {Set(value); } }
        public string MySQLConnectionString { get { return Get<string>(); } set { Set(value); } }
    }
}
