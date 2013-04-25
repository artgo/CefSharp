using System;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class CompanyWS
    {
        [XmlElementAttribute("id", Order = 1)]
        public string id { get; set; }

        [XmlElementAttribute("name", Order = 2)]
        public string name { get; set; }

        [XmlElementAttribute("contact", IsNullable = false, Order = 3)]
        public ContactInfoWS contact { get; set; }
    }
}