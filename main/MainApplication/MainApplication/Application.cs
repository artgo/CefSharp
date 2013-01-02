﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient
{
    ///<summary>
    /// Represents an Application of the sort that AppDirect distributes 
    ///</summary>
    [Serializable]
    public class Application
    {
        private ICommand _launchApp;
        private string _urlString;
        private Uri _uri;

        public string Id { get; set; } 
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int AlertCount { get; set; }
        public bool IsLocalApp { get; set; }
        
        [XmlIgnore]
        public Uri Uri
        {
            get
            {
                if (_uri == null)
                {
                    _uri = new Uri(UrlString);
                }
                return _uri;
            }
        }

        public string UrlString
        {
            get
            {
                if (_urlString == null)
                {
                    _urlString = String.Empty;
                }

                return _urlString;
            }
            set
            {
                _urlString = value;
                _uri = new Uri(UrlString);
            }
        }

        public ICommand LaunchApp
        {
            get
            {
                if (_launchApp == null)
                {
                    _launchApp = new RelayCommand<object>(param => Launch(),
                        null);
                }
                return _launchApp;
            }
        } 

        private void Launch()
        {
            System.Diagnostics.Process.Start("iexplore.exe", Uri.AbsoluteUri);
        }
    }
}
