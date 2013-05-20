using System;
using System.Drawing;

namespace TaskBarControl
{
    public class MessageParams
    {
        public IntPtr LParam { get; set; }
        public IntPtr WParam { get; set; }
    }

    public class CoordsPackager
    {
        private const int BitsToShift = 16;
        private const int BitMask = 0xFFFF;

        public MessageParams PackParams(Rectangle rect)
        {
            if (rect == null)
            {
                throw new ArgumentNullException("rect");
            }

            // C++ would unpack it like this:
            //const unsigned int p1 = (unsigned int)wParam;
            //const unsigned int p2 = (unsigned int)lParam;
            //buttonsRect.left = p1 >> 16;
            //buttonsRect.top = p1 & 0xFFFF;
            //buttonsRect.right = p2 >> 16;
            //buttonsRect.bottom = p2 & 0xFFFF;
            var result = new MessageParams
                {
                    WParam = (IntPtr) ((((uint) rect.Left) << BitsToShift) | (((uint) rect.Top) & BitMask)),
                    LParam = (IntPtr) ((((uint) rect.Right) << BitsToShift) | (((uint) rect.Bottom) & BitMask))
                };

            return result;
        }

        public Rectangle UnpackParams(IntPtr lParam, IntPtr wParam)
        {
            var p1 = (uint)wParam.ToInt32();
            var p2 = (uint)lParam.ToInt32();

            return new Rectangle(
                (int) (p1 >> BitsToShift),
                (int) (p1 & BitMask),
                (int) (p2 >> BitsToShift),
                (int) (p2 & BitMask));
        }
    }
}
