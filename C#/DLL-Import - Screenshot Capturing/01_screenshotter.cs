using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SkiaSharp;

namespace MyProgram;

public partial class ScreenshotService
{
    public static void Main()
    {
        var ScreenShot = CaptureScreen();
        SaveBitmapAsPng(ScreenShot, $@"D:\Screenshot-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png");
    }

    public static SKBitmap CaptureScreen()
    {
        // Create a compatible DC and a BitMap object that we want to copy.
        var screenDC = GetDC(IntPtr.Zero);
        var memoryDC = CreateCompatibleDC(screenDC);
        var bitmapDC = CreateCompatibleBitmap(screenDC, Screen.Wide, Screen.High);
        _ = SelectObject(memoryDC, bitmapDC);

        // Create the new SkiaSharp bitmap and copy the pixels to it.
        var skiaBitmap = new SKBitmap(Screen.Wide, Screen.High, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
        var bitmapInfo = new BITMAPINFO { bmiHeader = new BITMAPINFOHEADER { biSize = 40, biWidth = Screen.Wide, biHeight = -Screen.High, biPlanes = 1, biBitCount = 32, biCompression = BI_RGB } };

        // Draw into our memory DC using GetDC.
        _ = BitBlt(memoryDC, 0, 0, Screen.Wide, Screen.High, screenDC, 0, 0, SRCCOPY);
        _ = GetDIBits(memoryDC, bitmapDC, 0, (uint)Screen.High, skiaBitmap.GetPixels(), ref bitmapInfo, DIB_RGB_COLORS);

        // Clean up
        _ = DeleteObject(bitmapDC);
        _ = DeleteDC(memoryDC);
        _ = ReleaseDC(IntPtr.Zero, screenDC);

        return skiaBitmap;
    }

    public static void SaveBitmapAsPng(SKBitmap bitmap, string path)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
    }

    // Low Level APIs
    // --------------

    [LibraryImport("user32.dll")]
    private static partial IntPtr GetDC(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    private static partial int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [LibraryImport("gdi32.dll")]
    private static partial IntPtr CreateCompatibleDC(IntPtr hdc);

    [LibraryImport("gdi32.dll")]
    private static partial IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [LibraryImport("gdi32.dll")]
    private static partial IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool BitBlt(IntPtr destDC, int x, int y, int width, int height, IntPtr hdcSource, int xSrc, int ySrc, int rasterOp);

    [LibraryImport("gdi32.dll")]
    private static partial IntPtr DeleteDC(IntPtr hDc);

    [LibraryImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteObject(IntPtr hObject);

    [LibraryImport("gdi32.dll")]
    private static partial int GetDIBits(IntPtr hdc, IntPtr hbmp, uint startScan, uint scanLines, IntPtr lpvBits, ref BITMAPINFO lpbmi, uint usage);

    private const int SRCCOPY = 0x00CC0020;
    private const int BI_RGB = 0;
    private const int DIB_RGB_COLORS = 0;

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
    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public int bmiColors;
    }

    public static partial class Screen
    {
        [LibraryImport("user32.dll")]
        public static partial int GetSystemMetrics(int nIndex);

        public static readonly int Wide = GetSystemMetrics(0);
        public static readonly int High = GetSystemMetrics(1);
    }
}
