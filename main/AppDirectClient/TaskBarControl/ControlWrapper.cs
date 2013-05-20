using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TaskBarControl
{
    public class ControlWrapper
    {
        public ControlWrapper(Control control)
        {
            _control = control;
        }

        private Control _control;
        private int _desiredWidth = 100;
        private int _desiredHeight = 100;

        public Control Control { get { return _control; } }

        public event System.EventHandler DesiredWidthChanged;
        public event System.EventHandler DesiredHeightChanged;

        protected virtual void OnDesiredWidthChanged()
        {
            if (DesiredWidthChanged != null) { DesiredWidthChanged(this, EventArgs.Empty); }
        }

        protected virtual void OnDesiredHeightChanged()
        {
            if (DesiredHeightChanged != null) { DesiredHeightChanged(this, EventArgs.Empty); }
        }

        public int DesiredWidth
        {
            get { return _desiredWidth; }
            set
            {
                if (_desiredWidth != value)
                {
                    _desiredWidth = value;
                    OnDesiredWidthChanged();
                }
            }
        }
        public int DesiredHeight
        {
            get { return _desiredHeight; }
            set 
            {
                if (_desiredHeight != value)
                {
                    _desiredHeight = value;
                    OnDesiredHeightChanged();
                }
            }
        }
    }
}
