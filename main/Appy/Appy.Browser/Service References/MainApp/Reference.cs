﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using AppDirect.WindowsClient.Common.API;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AppDirect.WindowsClient.Browser.MainApp
{
    [GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [DataContractAttribute(Name = "CookieVariant", Namespace = "http://schemas.datacontract.org/2004/07/System.Net")]
    public enum CookieVariant : int
    {
        [EnumMemberAttribute()]
        Unknown = 0,

        [EnumMemberAttribute()]
        Plain = 1,

        [EnumMemberAttribute()]
        Rfc2109 = 2,

        [EnumMemberAttribute()]
        Rfc2965 = 3,

        [EnumMemberAttribute()]
        Default = 2,
    }

    [GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMainApplicationChannel : IMainApplication, IClientChannel
    {
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MainApplicationClient : ClientBase<IMainApplication>, IMainApplication
    {
        public MainApplicationClient()
            : base()
        {
        }

        public MainApplicationClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        public MainApplicationClient(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public MainApplicationClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public MainApplicationClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public void Initialized()
        {
            base.Channel.Initialized();
        }

        public IAppDirectSession GetSession()
        {
            return base.Channel.GetSession();
        }

        public IEnumerable<IApplication> GetMyApps()
        {
            return base.Channel.GetMyApps();
        }
    }
}