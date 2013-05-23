using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AppDirect.WindowsClient.InteropAPI
{
    public class ControlWrapper
    {
        public ControlWrapper(Control control)
        {
            _control = control;
        }

        private Control _control;

        private int _desiredOffset = 100;
        private Size _allowedSize;

        public Control Control { get { return _control; } }

        public event System.EventHandler DesiredOffsetChanged;
        public event System.EventHandler AllowedSizeChanged;

        protected virtual void OnDesiredOffsetChanged()
        {
            if (DesiredOffsetChanged != null) { DesiredOffsetChanged(this, EventArgs.Empty); }
        }

        protected virtual void OnAllowedSizeChanged()
        {
            if (AllowedSizeChanged != null) { AllowedSizeChanged(this, EventArgs.Empty); }
        }

        public int DesiredOffset
        {
            get { return _desiredOffset; }
            set
            {
                if (_desiredOffset != value)
                {
                    _desiredOffset = value;
                    OnDesiredOffsetChanged();
                }
            }
        }

        public Size AllowedSize
        {
            get { return _allowedSize; }
            set
            {
                if (_allowedSize != value)
                {
                    _allowedSize = value;
                    OnAllowedSizeChanged();
                }
            }
        }
    }
}
