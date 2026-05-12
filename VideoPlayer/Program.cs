using VideoPlayer;
using static VideoPlayer.Win32Defines;
using static VideoPlayer.Win32;
using System.Runtime.InteropServices;
using System.Buffers.Binary;

int RENDER_HEIGHT = 720;
int RENDER_WIDTH = 1024;
byte[] globalBuffer = new byte[RENDER_WIDTH * RENDER_HEIGHT * 4];
GCHandle globalBufferHandle = GCHandle.Alloc(globalBuffer, GCHandleType.Pinned);
BITMAPINFO biInfo = new BITMAPINFO();

// temp
int blueOffset = 0;
int greenOffset = 0;
//~~~~~~~~~~~~~~~~~~
bool running = true;
WNDPROC wndProc;
IntPtr CallBack(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
{
  IntPtr result = 0;
  // use var _ when hack to to keep WM_DESTROY fields static and not const so i can use them easily
  switch (msg)
  {
    case var _ when msg == WM_DESTROY:
      Console.WriteLine("VM_DESTROY");
      running = false;
      break;
    case var _ when msg == WM_CLOSE:
      Console.WriteLine("WM_CLOSE");
      running = false;
      break;
    case var _ when msg == WM_ACTIVATEAPP:
      Console.WriteLine("WM_ACTIVATEAPP");
      break;
    case var _ when msg == WM_PAINT:
      Console.WriteLine("WM_PAINT");
      PAINTSTRUCT Paint = new PAINTSTRUCT();
      IntPtr deviceContext = BeginPaint(hWnd, ref Paint);
      int X = Paint.rcPaint.left;
      int Y = Paint.rcPaint.top;
      int Height = Paint.rcPaint.bottom - Paint.rcPaint.top;
      int Width = Paint.rcPaint.right - Paint.rcPaint.left;
      (int height, int width) wSize = Win32GetWindowDimension(hWnd);
      Win32CopyBufferToWindow(deviceContext, wSize.width, wSize.height, globalBufferHandle.AddrOfPinnedObject());
      EndPaint(hWnd, ref Paint);
      break;
    default:
      result = DefWindowProcW(hWnd, msg, wParam, lParam);
      break;
  }
  
  return result;
}

wndProc = CallBack;
IntPtr wndPtr = Marshal.GetFunctionPointerForDelegate(wndProc);
WNDCLASSW classW = new WNDCLASSW();
classW.style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC;
classW.lpfnWndProc = wndPtr;
classW.hInstance = GetModuleHandleW(null);
classW.lpszClassName = "VideoPlayer";

int rawSize = Marshal.SizeOf(classW);
byte[] rawData = new byte[rawSize];
GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

if (RegisterClassW(ref classW) == 0)
{
  Console.WriteLine("REGISTER CLASS ERR");
  Console.ReadKey();
  return 0;
}

IntPtr window = CreateWindowExW(
  0,
  lpClassName: classW.lpszClassName,
  lpWindowName: "VideoPlayer",
  dwStyle: (uint)(WS_OVERLAPPEDWINDOW | WS_VISIBLE),
  x: CW_USEDEFAULT,
  y: CW_USEDEFAULT,
  nWidth: CW_USEDEFAULT,
  nHeight: CW_USEDEFAULT,
  hWndParent: 0,
  hMenu: 0,
  hInstance: 0,
  lpParam: 0
);

if (window == 0)
{
  int lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
  Console.WriteLine($"Window creation error. Win32Err: {lastError}");
  Console.ReadKey();
  return 0;
}

biInfo.bmiHeader = new BITMAPINFOHEADER();
biInfo.bmiHeader.biSize = (uint)Marshal.SizeOf(biInfo.bmiHeader);
biInfo.bmiHeader.biWidth = RENDER_WIDTH;
// negative so bitmap is top to btottom and origin is upper left corner
biInfo.bmiHeader.biHeight = -RENDER_HEIGHT;
biInfo.bmiHeader.biPlanes = 1;
// 8 bits each for Red, Green, Blue and 8 extra padded for alignment on 4B boundaries 
biInfo.bmiHeader.biBitCount = 32;
biInfo.bmiHeader.biCompression = (uint)BI_RGB;

IntPtr deviceContext = GetDC(window);
running = true;
while (running)
{
  MSG Message = new MSG();  
  while (PeekMessageW(ref Message, 0, 0, 0, PM_REMOVE))
  {
    if (Message.message == WM_QUIT)
    {
      Console.WriteLine("Received WM_QUIT!");
      running = false;
    }
    else if (Message.message == WM_PAINT)
      Console.WriteLine("PAINT");
    TranslateMessage(ref Message);
    DispatchMessageW(ref Message);
  }

  (int height, int width) wSize = Win32GetWindowDimension(window);
  Win32CopyBufferToWindow(deviceContext, wSize.width, wSize.height, globalBufferHandle.AddrOfPinnedObject());

  blueOffset++;
  greenOffset += 2;
  RenderWeirdGradientIntoBuffer();
}
return 0;

(int height, int width) Win32GetWindowDimension(IntPtr window)
{
  RECT ClientRect = new RECT();
  GetClientRect(window, ref ClientRect);
  return (ClientRect.bottom - ClientRect.top, ClientRect.right - ClientRect.left);
}

void Win32CopyBufferToWindow(IntPtr deviceContext, int wWidth, int wHeight, IntPtr buffer)
{
  int result = StretchDIBits(
      deviceContext,
      0, 0, wWidth, wHeight,
      0, 0, RENDER_WIDTH, RENDER_HEIGHT,
      buffer,
      ref biInfo,
      DIB_RGB_COLORS,
      SRCCOPY
    );
  if (result == 0)
  {
    Console.WriteLine("Error in StrechDIBits: ", result);
  }
}

void RenderWeirdGradientIntoBuffer()
{
  Span<byte> sp = globalBuffer.AsSpan();
  for (int Y = 0; Y < RENDER_HEIGHT; Y++)
  {
    for (int X = 0; X < RENDER_WIDTH; X++)
    {
      byte Blue = (byte)(X + blueOffset);
      byte Green = (byte)(Y + greenOffset);
      uint val = (((uint)Green << 8) | Blue);
      int offset = (Y * RENDER_WIDTH + X) * 4;
      // not efficient but w/e
      BinaryPrimitives.WriteUInt32LittleEndian(sp.Slice(offset, 4), val);
    }
  }
}