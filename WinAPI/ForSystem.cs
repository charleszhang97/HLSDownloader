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
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
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


    }
}
