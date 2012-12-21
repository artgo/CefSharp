using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MainApplication
{
    public class LoginObject
    {
        public string UserName { get; set; }

        [XmlIgnore]
        public string Password { get; set; }

        public string AuthToken { get; set; }
    }
}
