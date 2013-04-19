using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class MembershipWS
    {
        [XmlElementAttribute("company")]
        public CompanyWS company { get; set; }

        [XmlArrayItem("role", IsNullable = false)]
        [XmlArray("roles")]
        public List<Role> roles = new List<Role>();
    }
}