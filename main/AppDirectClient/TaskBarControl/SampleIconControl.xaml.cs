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

        public static readonly DependencyProperty DesiredWidthProperty =
            DependencyProperty.Register("DesiredWidth", typeof(int), typeof(SampleIconControl), new PropertyMetadata(100));
	 
	    [Bindable(true)]
        public int DesiredWidth
	    {
            get { return (int)this.GetValue(DesiredWidthProperty); }
            set { this.SetValue(DesiredWidthProperty, value); }
	    }

        public static readonly DependencyProperty DesiredHeightProperty = 
            DependencyProperty.Register("DesiredHeight", typeof(int), typeof(SampleIconControl), new PropertyMetadata(100));

        [Bindable(true)]
        public int DesiredHeight
        {
            get { return (int)this.GetValue(DesiredHeightProperty); }
            set { this.SetValue(DesiredHeightProperty, value); }
        }
    }
}
