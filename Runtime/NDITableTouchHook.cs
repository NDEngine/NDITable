using System;
using System.Runtime.InteropServices;
using xFrame.NDITable;
using xFrame.Unity;

public class NDITableTouchHook : XComponent {
#if UNITY_STANDALONE_WIN
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        public int X;
        public int Y;

        public POINT(int x, int y) {
            X = x;
            Y = y;
        }
    }

    [DllImport("user32.dll")]
    private static extern uint GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(IntPtr proc, IntPtr handle, int message, IntPtr wparam, IntPtr lparam);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLongPtr(IntPtr handle, int index, long ptr);

    [DllImport("user32.dll")]
    static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    private delegate IntPtr WndProc(IntPtr handle, int message, IntPtr wparam, IntPtr lparam);

    private const int GWL_WNDPROC = -4;

    private const int WM_POINTERUPDATE = 0x0245;
    private const int WM_POINTERDOWN = 0x0246;
    private const int WM_POINTERUP = 0x0247;

    private IntPtr wndHandle = IntPtr.Zero;

    private IntPtr unityWndProc = IntPtr.Zero;

    private WndProc hook;

    void FixedUpdate() {
        if (wndHandle == IntPtr.Zero) {
            wndHandle = (IntPtr)GetActiveWindow();

            hook = TouchProc;

            unityWndProc = SetWindowLongPtr(wndHandle, GWL_WNDPROC, (long)(Marshal.GetFunctionPointerForDelegate(hook)));
        }
    }

    private int GetIntUnchecked(IntPtr value) {
        return IntPtr.Size == 8 ? unchecked((int)value.ToInt64()) : value.ToInt32();
    }
    private int LowWord(IntPtr value) {
        return unchecked((short)GetIntUnchecked(value));
    }
    private int HighWord(IntPtr value) {
        return unchecked((short)(((uint)GetIntUnchecked(value)) >> 16));
    }

    private IntPtr TouchProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam) {
        switch (msg) {
            case WM_POINTERDOWN: {
                    var pt = new POINT(LowWord(lparam), HighWord(lparam));

                    ScreenToClient(hwnd, ref pt);

                    var touch = new NDITouch {
                        Id = LowWord(wparam),
                        x = pt.X,
                        y = pt.Y
                    };

                    NDITouchInput.Instance.Add(touch);
                }
                break;

            case WM_POINTERUPDATE: {
                    var pt = new POINT(LowWord(lparam), HighWord(lparam));

                    ScreenToClient(hwnd, ref pt);

                    var touch = new NDITouch {
                        Id = LowWord(wparam),
                        x = pt.X,
                        y = pt.Y
                    };

                    NDITouchInput.Instance.Update(touch);
                }
                break;

            case WM_POINTERUP: {
                    var pt = new POINT(LowWord(lparam), HighWord(lparam));

                    ScreenToClient(hwnd, ref pt);

                    var touch = new NDITouch {
                        Id = LowWord(wparam),
                        x = pt.X,
                        y = pt.Y
                    };

                    NDITouchInput.Instance.Remove(touch);
                }
                break;
        }

        return CallWindowProc(unityWndProc, hwnd, msg, wparam, lparam);
    }

    protected override void OnDisable() {
        SetWindowLongPtr(wndHandle, GWL_WNDPROC, (long)unityWndProc);

        wndHandle = IntPtr.Zero;
        unityWndProc = IntPtr.Zero;

        hook = null;
    }
#endif
}
