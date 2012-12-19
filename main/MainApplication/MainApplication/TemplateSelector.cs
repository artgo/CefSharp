using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MainApplication
{
    public class TemplateSelector: DataTemplateSelector
    {
        public DataTemplate AppDataTemplate { get; set; }
        public DataTemplate AppDataWithAlertTemplate { get; set; }
        public DataTemplate EmptyDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var app = item as Application;

            if (app == null)
            {
                throw new Exception("SelectTemplate called on an object that is not an application");
            }

            if (app.Name == null)
            {
                return EmptyDataTemplate;
            }

            if (app.AlertCount > 0)
            {
                return AppDataWithAlertTemplate;
            }

            return AppDataTemplate;
        }
    }
}
