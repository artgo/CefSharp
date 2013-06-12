using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AppDirect.WindowsClient.UI
{
    ///<summary>
    /// Represents a DataTemplateSelector that determines which DataTemplate to use to display an Application Item in a ListView
    /// </summary>
    public class TemplateSelector : DataTemplateSelector
    {
        public DataTemplate NameDisplayTemplate { get; set; }

        public DataTemplate LongNameDisplayTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var name = item as string;

            if (name == null)
            {
                throw new Exception("SelectTemplate called on an object that is not an application");
            }

            if (name.Length > 15 && name.Contains(' '))
            {
                return LongNameDisplayTemplate;
            }

            return NameDisplayTemplate;
        }
    }
}