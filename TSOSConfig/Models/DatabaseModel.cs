using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSOSConfig.Models
{
    public class DatabaseModel : Observable
    {
        public string DatabaseName { get { return Get<string>(); } set { Set(value); } }
        public string CustomerName { get { return Get<string>(); } set { Set(value); } }

        public override string ToString()
        {
            return DatabaseName + " -- " + CustomerName;
        }
    }
}
