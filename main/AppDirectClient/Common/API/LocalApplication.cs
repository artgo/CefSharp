using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.Common.API
{
    [Serializable]
    public class LocalApplication : Application
    {
        private const string _price = "Free";

        public override int AlertCount { get { return 0; } set { } }
        public override bool IsLocalApp { get { return true; } set { } }
        public override string Price { get { return _price; } set { } }
    }
}
