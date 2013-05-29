using System;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    public class CoordsPackager
    {
        private const int BitsToShift = 16;
        private const int BitMask = 0xFFFF;

        public MessageParams PackParams(RECT rect)
        {
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

        public RECT UnpackParams(IntPtr lParam, IntPtr wParam)
        {
            var p1 = (uint)wParam;
            var p2 = (uint)lParam;
            var result = new RECT
                {
                    Left = (int) (p1 >> BitsToShift),
                    Top = (int) (p1 & BitMask),
                    Right = (int) (p2 >> BitsToShift),
                    Bottom = (int) (p2 & BitMask)
                };
            return result;
        }
    }
}
