using System;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class ContactInfoWS
    {
        [XmlElementAttribute("address")]
        public AddressWS address { get; set; }

        [XmlElementAttribute("phoneNumber")]
        public string phoneNumber { get; set; }
    }
}