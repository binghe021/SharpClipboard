﻿using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

//https://stackoverflow.com/questions/621577/clipboard-event-c-sharp
//https://stackoverflow.com/questions/17762037/error-while-trying-to-copy-string-to-clipboard
//https://gist.github.com/glombard/7986317

internal static class NativeMethods
{
    //Reference https://docs.microsoft.com/en-us/windows/desktop/dataxchg/wm-clipboardupdate
    public const int WM_CLIPBOARDUPDATE = 0x031D;
    //Reference https://www.pinvoke.net/default.aspx/Constants.HWND
    public static IntPtr HWND_MESSAGE = new IntPtr(-3);

    //Reference https://www.pinvoke.net/default.aspx/user32/AddClipboardFormatListener.html
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AddClipboardFormatListener(IntPtr hwnd);

    //Reference https://www.pinvoke.net/default.aspx/user32.setparent
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    //Reference https://www.pinvoke.net/default.aspx/user32/getwindowtext.html
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    //Reference https://www.pinvoke.net/default.aspx/user32.getwindowtextlength
    [DllImport("user32.dll")]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    //Reference https://www.pinvoke.net/default.aspx/user32.getforegroundwindow
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
}

public static class Clipboard
{
    public static string GetText()
    {
        string ReturnValue = string.Empty;
        Thread STAThread = new Thread(
            delegate ()
            {
                // Use a fully qualified name for Clipboard otherwise it
                // will end up calling itself.
                ReturnValue = System.Windows.Forms.Clipboard.GetText();
            });
        STAThread.SetApartmentState(ApartmentState.STA);
        STAThread.Start();
        STAThread.Join();

        return ReturnValue;
    }
}

public sealed class ClipboardNotification
{
    private class NotificationForm : Form
    {
        public NotificationForm()
        {
            //Turn the child window into a message-only window (refer to Microsoft docs)
            NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
            //Place window in the system-maintained clipboard format listener list
            NativeMethods.AddClipboardFormatListener(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            //Listen for operating system messages
            if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                //Get the date and time for the current moment expressed as coordinated universal time (UTC).
                DateTime saveNow = DateTime.Now;
                Console.WriteLine("Copy event detected at {0}", saveNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                //Write to stdout active window
                IntPtr active_window = NativeMethods.GetForegroundWindow();
                int length = NativeMethods.GetWindowTextLength(active_window);
                StringBuilder sb = new StringBuilder(length + 1);
                NativeMethods.GetWindowText(active_window, sb, sb.Capacity);
                Console.WriteLine("Clipboard Active Window: " + sb.ToString());

                //Write to stdout clipboard contents
                string clipboardContents = Clipboard.GetText();
                Console.WriteLine("Clipboard Content: " + clipboardContents);

                if (!string.IsNullOrEmpty(clipboardContents))
                {
                    if (clipboardContents.StartsWith("http") && clipboardContents.EndsWith(".m3u8"))
                    {
                        string m3u8downloadExe = ConfigurationManager.AppSettings["m3u8downloadExe"].ToString();
                        if (!string.IsNullOrEmpty(m3u8downloadExe))
                        {
                            ProcessStartInfo processStartInfo = new ProcessStartInfo();
                            processStartInfo.FileName = @m3u8downloadExe;// @"F:\M3U8Dwonloader\N_m3u8DL-CLI_v2.9.9.exe";
                            processStartInfo.Arguments = clipboardContents;

                            Process.Start(processStartInfo);
                        }

                    }
                }


            }
            //Called for any unhandled messages
            base.WndProc(ref m);
        }
    }

    private static void Main(string[] args)
    {
        //starts a message loop on current thread and displays specified form
        Application.Run(new NotificationForm());
    }
}
