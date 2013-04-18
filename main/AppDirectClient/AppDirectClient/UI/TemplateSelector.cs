using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Application = AppDirect.WindowsClient.Common.API.Application;

namespace AppDirect.WindowsClient.UI
{
    ///<summary>
    /// Represents a DataTemplateSelector that determines which DataTemplate to use to display an Application Item in a ListView
    /// </summary>
    public class TemplateSelector: DataTemplateSelector
    {
        public DataTemplate AppDataTemplate { get; set; }
        public DataTemplate AppDataWithLongNameTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var app = item as ApplicationViewModel;

            if (app == null)
            {
                throw new Exception("SelectTemplate called on an object that is not an application");
            }

            if (app.Application.Name.Length > 15 && app.Application.Name.Contains(' '))
            {
                return AppDataWithLongNameTemplate;
            }

            return AppDataTemplate;
        }
    }
}
