using System.Runtime.InteropServices;

namespace VideoPlayer
{
  public static class Win32Defines
  {
    /*
     * Windows Styles
     */
    public static long WS_OVERLAPPED       = 0x00000000L;
    public static long WS_POPUP            = 0x80000000L;
    public static long WS_CHILD            = 0x40000000L;
    public static long WS_MINIMIZE         = 0x20000000L;
    public static long WS_VISIBLE          = 0x10000000L;
    public static long WS_DISABLED         = 0x08000000L;
    public static long WS_CLIPSIBLINGS     = 0x04000000L;
    public static long WS_CLIPCHILDREN     = 0x02000000L;
    public static long WS_MAXIMIZE         = 0x01000000L;
    public static long WS_CAPTION          = 0x00C00000L; /* WS_BORDER | WS_DLGFRAME  */
    public static long WS_BORDER           = 0x00800000L;
    public static long WS_DLGFRAME         = 0x00400000L;
    public static long WS_VSCROLL          = 0x00200000L;
    public static long WS_HSCROLL          = 0x00100000L;
    public static long WS_SYSMENU          = 0x00080000L;
    public static long WS_THICKFRAME       = 0x00040000L;
    public static long WS_GROUP            = 0x00020000L;
    public static long WS_TABSTOP          = 0x00010000L;
    public static long WS_MINIMIZEBOX      = 0x00020000L;
    public static long WS_MAXIMIZEBOX      = 0x00010000L;

    public static long WS_OVERLAPPEDWINDOW = WS_OVERLAPPED  |
                                             WS_CAPTION     |
                                             WS_SYSMENU     |
                                             WS_THICKFRAME  |
                                             WS_MINIMIZEBOX |
                                             WS_MAXIMIZEBOX ;


    public static int CW_USEDEFAULT        = (Int32.MinValue);


    /* constants for the biCompression field */
    public static long BI_RGB        =      0L;
    public static long BI_RLE8       =      1L;
    public static long BI_RLE4       =      2L;
    public static long BI_BITFIELDS  =      3L;
    public static long BI_JPEG       =      4L;
    public static long BI_PNG        =      5L;


    /*
     * Class styles
     */
    public static uint CS_VREDRAW          = 0x0001;
    public static uint CS_HREDRAW          = 0x0002;
    public static uint CS_DBLCLKS          = 0x0008;
    public static uint CS_OWNDC            = 0x0020;
    public static uint CS_CLASSDC          = 0x0040;
    public static uint CS_PARENTDC         = 0x0080;
    public static uint CS_NOCLOSE          = 0x0200;
    public static uint CS_SAVEBITS         = 0x0800;
    public static uint CS_BYTEALIGNCLIENT  = 0x1000;
    public static uint CS_BYTEALIGNWINDOW  = 0x2000;
    public static uint CS_GLOBALCLASS      = 0x4000;

    /*
    * WM_ACTIVATE state values
    */
    public static uint WA_INACTIVE    = 0;
    public static uint WA_ACTIVE      = 1;
    public static uint WA_CLICKACTIVE = 2;

    /*
     * Window Messages
     */
    public static uint WM_NULL             = 0x0000;
    public static uint WM_CREATE           = 0x0001;
    public static uint WM_DESTROY          = 0x0002;
    public static uint WM_MOVE             = 0x0003;
    public static uint WM_SIZE             = 0x0005;
    public static uint WM_ACTIVATE         = 0x0006;
    public static uint WM_SETFOCUS         = 0x0007;
    public static uint WM_KILLFOCUS        = 0x0008;
    public static uint WM_ENABLE           = 0x000A;
    public static uint WM_SETREDRAW        = 0x000B;
    public static uint WM_SETTEXT          = 0x000C;
    public static uint WM_GETTEXT          = 0x000D;
    public static uint WM_GETTEXTLENGTH    = 0x000E;
    public static uint WM_PAINT            = 0x000F;
    public static uint WM_CLOSE            = 0x0010;
    public static uint WM_QUIT             = 0x0012;
    public static uint WM_ERASEBKGND       = 0x0014;
    public static uint WM_SYSCOLORCHANGE   = 0x0015;
    public static uint WM_SHOWWINDOW       = 0x0018;
    public static uint WM_WININICHANGE     = 0x001A;
    public static uint WM_DEVMODECHANGE    = 0x001B;
    public static uint WM_ACTIVATEAPP      = 0x001C;
    public static uint WM_FONTCHANGE       = 0x001D;
    public static uint WM_TIMECHANGE       = 0x001E;
    public static uint WM_CANCELMODE       = 0x001F;
    public static uint WM_SETCURSOR        = 0x0020;
    public static uint WM_MOUSEACTIVATE    = 0x0021;
    public static uint WM_CHILDACTIVATE    = 0x0022;
    public static uint WM_QUEUESYNC        = 0x0023;


    /* DIB color table identifiers */

    public static uint DIB_RGB_COLORS      = 0; /* color table in RGBs */
    public static uint DIB_PAL_COLORS      = 1; /* color table in palette indices */


    /* Ternary raster operations */
    public static uint SRCCOPY             = (uint)0x00CC0020; /* dest = source                   */
    public static uint SRCPAINT            = (uint)0x00EE0086; /* dest = source OR dest           */
    public static uint SRCAND              = (uint)0x008800C6; /* dest = source AND dest          */
    public static uint SRCINVERT           = (uint)0x00660046; /* dest = source XOR dest          */
    public static uint SRCERASE            = (uint)0x00440328; /* dest = source AND (NOT dest )   */
    public static uint NOTSRCCOPY          = (uint)0x00330008; /* dest = (NOT source)             */
    public static uint NOTSRCERASE         = (uint)0x001100A6; /* dest = (NOT src) AND (NOT dest) */
    public static uint MERGECOPY           = (uint)0x00C000CA; /* dest = (source AND pattern)     */
    public static uint MERGEPAINT          = (uint)0x00BB0226; /* dest = (NOT source) OR dest     */
    public static uint PATCOPY             = (uint)0x00F00021; /* dest = pattern                  */
    public static uint PATPAINT            = (uint)0x00FB0A09; /* dest = DPSnoo                   */
    public static uint PATINVERT           = (uint)0x005A0049; /* dest = pattern XOR dest         */
    public static uint DSTINVERT           = (uint)0x00550009; /* dest = (NOT dest)               */
    public static uint BLACKNESS           = (uint)0x00000042; /* dest = BLACK                    */
    public static uint WHITENESS           = (uint)0x00FF0062; /* dest = WHITE                    */

    /*
    * PeekMessage() Options
    */
    public static uint PM_NOREMOVE         = 0x0000;
    public static uint PM_REMOVE           = 0x0001;
    public static uint PM_NOYIELD          = 0x0002;

  }

  delegate IntPtr WNDPROC(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public struct WNDCLASSW
  {
    public uint style;
    public IntPtr lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string lpszMenuName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string lpszClassName;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int left;
    public int top;
    public int right;
    public int bottom;
  }
  [StructLayout(LayoutKind.Sequential)]
  public struct PAINTSTRUCT
  {
    public IntPtr hdc;
    public bool fErase;
    public RECT rcPaint;
    public bool fRestore;
    public bool fIncUpdate;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] rgbReserved;
  }
  [StructLayout(LayoutKind.Sequential)]
  public struct BITMAPINFO
  {
    public BITMAPINFOHEADER bmiHeader;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public RGBQUAD[] bmiColors; // not sure if this is right
  }
  [StructLayout(LayoutKind.Sequential)]
  public struct RGBQUAD
  {
    byte rgbBlue;
    byte rgbGreen;
    byte rgbRed;
    byte rgbReserved;
  }
  

  [StructLayout(LayoutKind.Sequential)]
  public struct BITMAPINFOHEADER
  {
    public uint biSize;
    public int biWidth;
    public int biHeight;
    public ushort biPlanes;
    public ushort biBitCount;
    public uint biCompression;
    public uint biSizeImage;
    public int biXPelsPerMeter;
    public int biYPelsPerMeter;
    public uint biClrUsed;
    public uint biClrImportant;
  }
  [StructLayout(LayoutKind.Sequential)]
  public struct MSG
  {
    public IntPtr hwnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
  }
  [StructLayout(LayoutKind.Sequential)]
  public struct POINT
  {
    public int x;
    public int y;
  }
}
