using System;
using System.Collections.Generic;

namespace AppDirect.WindowsClient.Common.API
{
    [Serializable]
    public class InitData : IInitData
    {
        public override IAppDirectSession Session { get; set; }

        public override IEnumerable<IApplication> Applications { get; set; }
    }
}