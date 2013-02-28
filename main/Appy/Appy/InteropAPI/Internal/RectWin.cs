using System.Runtime.InteropServices;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class RectWin
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width
        {
            get
            {
                return Right - Left;
            }
            set
            {
                Right = Left + value;
            }
        }

        public int Height
        {
            get
            {
                return Bottom - Top;
            }
            set
            {
                Bottom = Top + value;
            }
        }

        public RectWin() { }
        public RectWin(RECT rect)
        {
            Left = rect.left;
            Top = rect.top;
            Right = rect.right;
            Bottom = rect.bottom;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (GetType() != obj.GetType()))
            {
                return false;
            }

            var r = (RectWin)obj;

            return (this.Left == r.Left) && (this.Right == r.Right) && (this.Top == r.Top) && (this.Bottom == r.Bottom);
        }

        public override int GetHashCode()
        {
            const int hbase = 13;
            return Left * hbase * hbase * hbase + Top * hbase * hbase + Right * hbase + Bottom;
        }
    }
}