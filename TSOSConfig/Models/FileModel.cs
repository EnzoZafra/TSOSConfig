using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSOSConfig.Models
{
    public class FileModel : Observable
    {
        public DatabaseModel Database { get { return Get<DatabaseModel>(); } set { Set(value); } }
        public ConfigurationModel Configuration { get { return Get<ConfigurationModel>(); } set { Set(value); } }
        public string RawXML { get { return Get<string>(); } set { Set(value); } }
    }
}
