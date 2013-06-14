﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Linq;
using AppDirect.WindowsClient.Common.API;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AppDirect.WindowsClient.BrowsersApi
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
    public interface IBrowsersManagerApiChannel : IBrowsersManagerApi, IClientChannel
    {
    }

    [DebuggerStepThroughAttribute()]
    [GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BrowsersManagerApiClient : ClientBase<IBrowsersManagerApi>, IBrowsersManagerApi
    {
        public BrowsersManagerApiClient()
        {
        }

        public BrowsersManagerApiClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public BrowsersManagerApiClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BrowsersManagerApiClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BrowsersManagerApiClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public void DisplayApplication(IApplication application)
        {
            base.Channel.DisplayApplication(application);
        }

        public void DisplayApplicationWithoutSession(IApplication application)
        {
            base.Channel.DisplayApplicationWithoutSession(application);
        }

        public void DisplayApplications(IEnumerable<IApplicationWithState> applications)
        {
            base.Channel.DisplayApplications(applications);
        }

        public void CloseApplication(string appId)
        {
            base.Channel.CloseApplication(appId);
        }

        public void UpdateSession(IAppDirectSession newSession)
        {
            base.Channel.UpdateSession(newSession);
        }

        public void UpdateApplications(IEnumerable<IApplication> applications)
        {
            base.Channel.UpdateApplications(applications);
        }

        public void CloseAllApplicationsAndRemoveSessionInfo()
        {
            base.Channel.CloseAllApplicationsAndRemoveSessionInfo();
        }

        public void CloseBrowserProcess()
        {
            base.Channel.CloseBrowserProcess();
        }

        public IEnumerable<IWindowData> GetOpenWindowDatas()
        {
            return base.Channel.GetOpenWindowDatas();
        }

        public int Ping(int value)
        {
            return base.Channel.Ping(value);
        }
    }
}