using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DataGridValidationTest
{
    public class ScreenChange:Control
    {
        [DllImport("user32")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        private delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref Rect pRect, int dwData);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        public static List<MonitorInfo> ActualScreens = new List<MonitorInfo>();

        private const int WM_DISPLAYCHANGE = 0x007e;



        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_DISPLAYCHANGE)
            {
                //RefreshActualScreens();
                // do something really interesting here
            }
        }
        //public static void RefreshActualScreens()
        //{
        //    ActualScreens.Clear();
        //    MonitorEnumProc callback = (IntPtr hDesktop, IntPtr hdc, ref Rect prect, int d) =>
        //    {
        //        ActualScreens.Add(new MonitorInfo()
        //        {
        //            Bounds = new Rectangle()
        //            {
        //                X = prect.left,
        //                Y = prect.top,
        //                Width = prect.right - prect.left,
        //                Height = prect.bottom - prect.top,
        //            },
        //            IsPrimary = (prect.left == 0) && (prect.top == 0),
        //        });

        //        return true;
        //    };

        //    EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0);
        //}
    }
    //public class MonitorInfo
    //{
    //    public bool IsPrimary = false;
    //    public Rectangle Bounds = new Rectangle();
    //}
}
