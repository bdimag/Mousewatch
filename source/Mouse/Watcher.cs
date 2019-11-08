using System;
using System.Runtime.InteropServices;

namespace Mousewatch
{
    internal sealed class Watcher : IDisposable
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private readonly int hHook;
        private readonly Win32Api.HookProc hProc;

        public Watcher()
        {
            hProc = new Win32Api.HookProc(MouseHookProc);
            hHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
        }

        public event EventHandler<MouseStateChangedEventArgs> MouseStateChanged;

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                switch ((int)wParam)
                {
                    case WM_LBUTTONDOWN:
                        MouseStateChanged?.Invoke(this, new MouseStateChangedEventArgs(1, true));
                        break;

                    case WM_RBUTTONDOWN:
                        MouseStateChanged?.Invoke(this, new MouseStateChangedEventArgs(2, true));
                        break;

                    case WM_MBUTTONDOWN:
                        MouseStateChanged?.Invoke(this, new MouseStateChangedEventArgs(3, true));
                        break;

                    case WM_LBUTTONUP:
                        MouseStateChanged?.Invoke(this, new MouseStateChangedEventArgs(1, false));
                        break;

                    case WM_RBUTTONUP:
                        MouseStateChanged?.Invoke(this, new MouseStateChangedEventArgs(2, false));
                        break;

                    case WM_MBUTTONUP:
                        MouseStateChanged?.Invoke(this, new MouseStateChangedEventArgs(3, false));
                        break;
                }
                return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public void Dispose()
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }

        private class Win32Api
        {
            public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetWindowsHookEx(int idHook, HookProc ipfn, IntPtr hinstance, int threadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern bool UnhookWindowsHookEx(int idHook);
        }

        public class MouseStateChangedEventArgs : EventArgs
        {
            public int Button { get; }

            public bool State { get; }

            public MouseStateChangedEventArgs(int button, bool state)
            {
                Button = button;
                State = state;
            }
        }
    }
}