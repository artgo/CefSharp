using System;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class ParameterWS
    {
		[XmlElementAttribute("name")]
		public string name { get; set; }

        [XmlElementAttribute("value")]
		public string value { get; set; }
    }
}