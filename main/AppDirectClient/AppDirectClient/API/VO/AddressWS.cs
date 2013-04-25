using System;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class AddressWS
    {
        [XmlElementAttribute("street1")]
        public string street1 { get; set; }

        [XmlElement("street2")]
        public string street2 { get; set; }

        [XmlElementAttribute("city")]
        public string city { get; set; }

        [XmlElementAttribute("state")]
        public string state { get; set; }

        [XmlElementAttribute("zip")]
        public string zip { get; set; }

        [XmlElementAttribute("country")]
        public string country { get; set; }
    }
}