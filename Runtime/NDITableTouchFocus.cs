using System;
using System.Linq;
using System.Runtime.InteropServices;
using xFrame.NDITable;
using xFrame.NDITable.Unity.EventSystems;
using xFrame.Unity;

public class NDITableTouchFocus : XComponent {
    private bool focus;

#if UNITY_STANDALONE_WIN
    private IntPtr hWnd = IntPtr.Zero;

    [DllImport("user32.dll")]
    private static extern uint GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    void OnApplicationFocus(bool windowFocus) {
        focus = windowFocus;
    }

    void Start() {
        if (hWnd == IntPtr.Zero) {
            hWnd = (IntPtr)GetActiveWindow();
        }
    }

    void Update() {
        if (!focus) {
            if (NDITableInput.FingerCount() > 0
            && NDITableInput.GetFingerDatas().Any(IsValidFinger)) {
                SetFocus(hWnd);
            }
        }
    }
#endif

    private bool IsValidFinger(FingerData data) {
        return data.X >= 0.0f && data.X <= 1.0f && data.Y >= 0.0f && data.Y <= 1.0f;
    }
}
