using System.Runtime.InteropServices;
using System.Security;
namespace VideoPlayer
{
  /*/
   *  Win32 Types	Specification	CLR Type
      char, INT8, SBYTE, CHARâ€ 	8-bit signed integer	System.SByte
      short, short int, INT16, SHORT	16-bit signed integer	System.Int16
      int, long, long int, INT32, LONG32, BOOLâ€ , INT	32-bit signed integer	System.Int32
      __int64, INT64, LONGLONG	64-bit signed integer	System.Int64
      unsigned char, UINT8, UCHARâ€ , BYTE	8-bit unsigned integer	System.Byte
      unsigned short, UINT16, USHORT, WORD, ATOM, WCHARâ€ , __wchar_t	16-bit unsigned integer	System.UInt16
      unsigned, unsigned int,  UINT32, ULONG32, DWORD32, ULONG, DWORD, UINT	32-bit unsigned integer	System.UInt32
      unsigned __int64, UINT64, DWORDLONG, ULONGLONG	64-bit unsigned integer	System.UInt64
      float, FLOAT	Single-precision floating point	System.Single
      double, long double, DOUBLE	Double-precision floating point	System.Double
      â€ In Win32 this type is an integer with a specially assigned meaning; in contrast, the CLR provides a specific type devoted to this meaning.
      ATOM -> ushort
   */
  public class Win32
  {
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr CreateWindowExW(
      uint dwExStyle,
      [MarshalAs(UnmanagedType.LPWStr)]
      string lpClassName,
      [MarshalAs(UnmanagedType.LPWStr)]
      string lpWindowName,
      uint dwStyle,
      int x,
      int y,
      int nWidth,
      int nHeight,
      IntPtr hWndParent,
      IntPtr hMenu,
      IntPtr hInstance,
      IntPtr lpParam
    );

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr DefWindowProcW(
      IntPtr hWnd,
      uint Msg,
      UIntPtr wParam,
      IntPtr lParam
    );
    [DllImport("user32.dll", SetLastError = true)]
    public static extern ushort RegisterClassW([In] ref WNDCLASSW lpWndClass);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetModuleHandleW(
      string? lpModuleName
    );

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr BeginPaint(
      IntPtr hWnd,
      ref PAINTSTRUCT lpPaint
    );
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr EndPaint(
     IntPtr hWnd,
     ref PAINTSTRUCT lpPaint
   );

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetClientRect(
      IntPtr hWnd,
      ref RECT lpPaint
    );

    [SuppressUnmanagedCodeSecurity]
    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern int StretchDIBits(
      IntPtr hdc,
      int xDest,
      int yDest,
      int DestWidth,
      int DestHeight,
      int xSrc,
      int ySrc,
      int SrcWidth,
      int SrcHeight,
      IntPtr lpBits,
      ref BITMAPINFO lpbmi,
      uint iUsage,
      uint rop
    );
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetDC(
      IntPtr hWnd
    );
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PeekMessageW(
      ref MSG lpMsg,
      IntPtr hWnd,
      uint wMsgFilterMin,
      uint wMsgFilterMax,
      uint wRemoveMsg
    );

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool TranslateMessage(
      ref MSG lpMSG
    );

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr DispatchMessageW(
      ref MSG lpMSG
    );
  }
}
