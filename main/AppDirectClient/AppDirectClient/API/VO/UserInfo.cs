﻿using System;
using System.Globalization;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class UserInfo
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Email { get; set; }

        public bool Verified { get; set; }

        public string CompanyId { get; set; }

        [XmlIgnore]
        public CultureInfo Culture { get; set; }
    }
}