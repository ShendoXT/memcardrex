//Aero glass support for Windows Vista and up
//Based on the free examples provided by Graham O’Neale (http://goneale.com/)
//Shendo 2009 - 2011

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace MemcardRex
{
    class glassSupport
    {
        [DllImport("dwmapi.dll")]public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref margins pMarInset);
        [DllImport("dwmapi.dll")]static extern void DwmIsCompositionEnabled(ref bool pfEnabled);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern int SaveDC(IntPtr hdc);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hdc, int state);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        [DllImport("UxTheme.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref RECT pRect, ref DTTOPTS pOptions);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, int ppvBits, IntPtr hSection, uint dwOffset);

        private const int DTT_COMPOSITED = (int)(1UL << 13);
        private const int DTT_GLOWSIZE = (int)(1UL << 11);

        private const int DT_SINGLELINE = 0x00000020;
        private const int DT_CENTER = 0x00000001;
        private const int DT_VCENTER = 0x00000004;
        private const int DT_NOPREFIX = 0x00000800;

        private const int SRCCOPY = 0x00CC0020;

        private const int BI_RGB = 0;
        private const int DIB_RGB_COLORS = 0;

        public const int HTCLIENT = 0x1;
        public const int HTCAPTION = 0x2;
        public const int WM_NCHITTEST = 0x84;
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        public struct margins
        {
            public int left;
            public int right;
            public int top;
            public int bottom;
        }

        private struct POINTAPI
        {
            public int x;
            public int y;
        };

        private struct DTTOPTS
        {
            public uint dwSize;
            public uint dwFlags;
            public uint crText;
            public uint crBorder;
            public uint crShadow;
            public int iTextShadowType;
            public POINTAPI ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public int fApplyOverlay;
            public int iGlowSize;
            public IntPtr pfnDrawTextCallback;
            public int lParam;
        };

        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        };

        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        };

        private struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        };

        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        };

        //Check if the Aero glass is available and enabled
        public bool isGlassSupported()
        {
            bool glassSupported = false;

            //Check if the OS supports DWM (only Windows Vista and 7)
            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor <= 1)
            {
                //Check if the DWM is enabled
                DwmIsCompositionEnabled(ref glassSupported);
            }

            return glassSupported;
        }

        //Draw text on the glass surface
        public void DrawTextOnGlass(IntPtr hwnd, String text, Font font, Rectangle ctlrct, int iglowSize)
        {
            if (isGlassSupported())
            {
                RECT rc = new RECT();
                RECT rc2 = new RECT();

                rc.left = ctlrct.Left;
                rc.right = ctlrct.Right;
                rc.top = ctlrct.Top;
                rc.bottom = ctlrct.Bottom;

                rc2.left = 6;
                rc2.top = 5;
                rc2.right = rc.right - rc.left;
                rc2.bottom = rc.bottom - rc.top;

                IntPtr destdc = GetDC(hwnd);
                IntPtr Memdc = CreateCompatibleDC(destdc);
                IntPtr bitmap;
                IntPtr bitmapOld = IntPtr.Zero;
                IntPtr logfnotOld;

                int uFormat = DT_SINGLELINE | DT_NOPREFIX;

                BITMAPINFO dib = new BITMAPINFO();
                dib.bmiHeader.biHeight = -(rc.bottom - rc.top);
                dib.bmiHeader.biWidth = rc.right - rc.left;
                dib.bmiHeader.biPlanes = 1;
                dib.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                dib.bmiHeader.biBitCount = 32;
                dib.bmiHeader.biCompression = BI_RGB;
                if (!(SaveDC(Memdc) == 0))
                {
                    bitmap = CreateDIBSection(Memdc, ref dib, DIB_RGB_COLORS, 0, IntPtr.Zero, 0);
                    if (!(bitmap == IntPtr.Zero))
                    {
                        bitmapOld = SelectObject(Memdc, bitmap);
                        IntPtr hFont = font.ToHfont();
                        logfnotOld = SelectObject(Memdc, hFont);
                        try
                        {
                            System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);
                            DTTOPTS dttOpts = new DTTOPTS();
                            dttOpts.dwSize = (uint)Marshal.SizeOf(typeof(DTTOPTS));
                            dttOpts.dwFlags = DTT_COMPOSITED | DTT_GLOWSIZE;
                            dttOpts.iGlowSize = iglowSize;
                            DrawThemeTextEx(renderer.Handle, Memdc, 0, 0, text, -1, uFormat, ref rc2, ref dttOpts);
                            BitBlt(destdc, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, Memdc, 0, 0, SRCCOPY);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        SelectObject(Memdc, bitmapOld);
                        SelectObject(Memdc, logfnotOld);
                        DeleteObject(bitmap);
                        DeleteObject(hFont);

                        ReleaseDC(Memdc, -1);
                        DeleteDC(Memdc);
                    }
                }
            }
        }
    }
}
