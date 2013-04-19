using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    /// <summary>
    /// Sample input:
    ///     <userinfo>
    ///         <user_id>721dbad9-f567-4790-802a-b2a5ad5a7a58</user_id>
    ///         <name>Sneha Agnihotri</name>
    ///         <given_name>Sneha</given_name>
    ///         <family_name>Agnihotri</family_name>
    ///         <email>sneha.agnihotri@appdirect.com</email>
    ///         <verified>true</verified>
    ///         <company_id>d9785380-6227-11e0-99c3-12313d022285</company_id>
    ///         <locale>en_US</locale>
    ///     </userinfo>
    /// </summary>
    [DataContract(Name = "userinfo")]
    [Serializable]
    public class UserInfoRaw
    {
        [XmlElement("user_id")]
        [DataMember(Name = "user_id")]
        public string User_Id { get; set; }

        [XmlElement("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [XmlElement("given_name")]
        [DataMember(Name = "given_name")]
        public string Given_Name { get; set; }

        [XmlElement("family_name")]
        [DataMember(Name = "family_name")]
        public string Family_Name { get; set; }

        [XmlElement("email")]
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [XmlElement("verified")]
        [DataMember(Name = "verified")]
        public bool Verified { get; set; }

        [XmlElement("company_id")]
        [DataMember(Name = "company_id")]
        public string Company_Id { get; set; }

        // By default will print out to specification http://en.wikipedia.org/wiki/Locale
        [XmlElement("locale")]
        [DataMember(Name = "locale")]
        public string Locale { get; set; }
    }
}