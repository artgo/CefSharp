﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
        private int _desiredOffset = 100;

        public Control Control { get { return _control; } }

        public event System.EventHandler DesiredOffsetChanged;

        protected virtual void OnDesiredOffsetChanged()
        {
            if (DesiredOffsetChanged != null) { DesiredOffsetChanged(this, EventArgs.Empty); }
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

        public Size Size
        {
            get { return new Size((int)_control.Width, (int)_control.Height); }
            set { _control.Width = value.Width; _control.Height = value.Height; }
        }
    }
}
