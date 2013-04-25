using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class UserWS
    {
        [XmlElementAttribute("id", Order = 1)]
        public string id { get; set; }

        [XmlElementAttribute("openId", Order = 2)]
        public string openId { get; set; }

        [XmlElementAttribute("email", Order = 3)]
        public string email { get; set; }

        [XmlElementAttribute("firstName", Order = 4)]
        public string firstName { get; set; }

        [XmlElementAttribute("lastName", Order = 5)]
        public string lastName { get; set; }

        [XmlElementAttribute("language", Order = 6)]
        public string language { get; set; }

        [XmlElementAttribute("status", Order = 7)]
        public UserStatusWS? status { get; set; }

        [XmlElementAttribute("contact", Order = 8)]
        public ContactInfoWS contact { get; set; }

        [XmlArrayItem("membership", IsNullable = false)]
        [XmlArray("memberships", IsNullable = false, Order = 9)]
        public List<MembershipWS> memberships = new List<MembershipWS>();
    }
}