using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinAPI
{
    /// <summary>
    /// Windows操作系统控制类
    /// </summary>
    public class ForSystem
    {
        /// <summary>
        /// 关机
        /// </summary>
        /// <returns></returns>
        public static void Shutdown()
        {
            //ExitWindowsEx(1, 0);
            Process.Start("shutdown", "/s /t 0");
            //Process.Start("shutdown", "/p");
        }

        /// <summary>
        /// 重启
        /// </summary>
        /// <returns></returns>
        public static void Reboot()
        {
            //ExitWindowsEx(2, 0);
            Process.Start("shutdown", "/r /t 0");
        }

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public static void Logoff()
        {
            //ExitWindowsEx(0, 0);
            Process.Start("shutdown", "/l");
        }

        [DllImport("user32")]
        public static extern void LockWorkStation();
        /// <summary>
        /// 锁定
        /// </summary>
        /// <returns></returns>
        public static void Lock()
        {
            //LockWorkStation();
            Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
        }

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
        /// <summary>
        /// 睡眠
        /// </summary>
        /// <returns></returns>
        public static void Sleep()
        {
            //ExitWindowsEx(-1, 0);
            SetSuspendState(false, true, false);
            //System.Windows.Forms.Application.SetSuspendState(PowerState.Suspend, true, false);
            //Process.Start("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");//讹传，无效，参数0 1 0无效
        }

        /// <summary>
        /// 休眠
        /// </summary>
        /// <returns></returns>
        public static void Hibernate()
        {
            //ExitWindowsEx(-2, 0);
            //SetSuspendState(true, true, false);
            //System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, true, false);
            Process.Start("shutdown", "/h");
            //Process.Start("rundll32.exe", "powrprof.dll,SetSuspendState");//有效
        }


        #region 关闭显示器
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }
        public static void TurnOffMonitor(IntPtr hwnd_broadcast)
        {
            SetMonitorInState(hwnd_broadcast, MonitorState.MonitorStateOff);
        }
        private static void SetMonitorInState(IntPtr hwnd_broadcast, MonitorState state)
        {            
            //SendMessage((IntPtr)0xFFFF, (uint)0x0112, 0xF170, (int)state);
            SendMessage(hwnd_broadcast, WM_SYSCOMMAND, SC_MONITORPOWER, (int)state);
        }
        #endregion

        #region 查找窗体
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //[DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindWindowEx")]
        //public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        #endregion

        #region 还原窗体

        #region 窗体状态
        public const uint SW_HIDE = 0;
        //public const uint SW_SHOWNORMAL = 1;
        //public const uint SW_SHOWMINIMIZED = 2;
        //public const uint SW_SHOWMAXIMIZED = 3;
        //public const uint SW_MAXIMIZE = 3;
        //public const uint SW_SHOWNOACTIVATE = 4;
        public const uint SW_SHOW = 5;
        //public const uint SW_MINIMIZE = 6;
        //public const uint SW_SHOWMINNOACTIVE = 7;
        //public const uint SW_SHOWNA = 8;
        public const uint SW_RESTORE = 9;
        #endregion

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int IsIconic(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        //[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        //public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        //public const uint SWP_NOSIZE = 0x0001;
        //public const uint SWP_NOMOVE = 0x0002;
        //public const uint SWP_NOZORDER = 0x0004;
        //public const uint SWP_NOREDRAW = 0x0008;
        //public const uint SWP_NOACTIVATE = 0x0010;
        //public const uint SWP_FRAMECHANGED = 0x0020;
        //public const uint SWP_SHOWWINDOW = 0x0040;
        //public const uint SWP_HIDEWINDOW = 0x0080;
        //public const uint SWP_NOCOPYBITS = 0x0100;
        //public const uint SWP_NOOWNERZORDER = 0x0200;
        //public const uint SWP_NOSENDCHANGING = 0x0400;
        //public const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        #endregion

        #region 窗体间传递消息
        public const int WM_COPYDATA = 0x004A;//copydata消息ID
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, ref COPYDATASTRUCT lParam);
        #endregion
    }

    /// <summary>
    /// 定义结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData; //可以是任意值
        public int cbData;    //指定lpData内存区域的字节数
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpData; //发送给目录窗口所在进程的数据
    }
}
