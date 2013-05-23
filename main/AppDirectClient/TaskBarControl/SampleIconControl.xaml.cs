using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskBarControl
{
    /// <summary>
    /// Interaction logic for SampleIconControl.xaml
    /// </summary>
    public partial class SampleIconControl : UserControl
    {
        public SampleIconControl()
        {
            InitializeComponent();
        }

        public delegate void OffsetEventHandler(int offset);

        public event OffsetEventHandler OffsetChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double offset = 0;
            if (double.TryParse(TextBoxOffset.Text, out offset))
            {
                if (OffsetChanged != null)
                {
                    OffsetChanged((int)offset);
                }
            }
        }
    }
}
