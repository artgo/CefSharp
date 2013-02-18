using System.Runtime.InteropServices;

namespace AppDirect.WindowsClient.InteropAPI.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct RectWin
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width { get { return Right - Left; } }
        public int Height { get { return Bottom - Top; } }
    }
}