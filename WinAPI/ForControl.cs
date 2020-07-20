using System;
using System.Runtime.InteropServices;

namespace WinAPI
{
    /// <summary>
    /// Windows窗体控件API
    /// </summary>
    public class ForControl
    {
        #region 隐藏光标
        /// <summary>
        /// 隐藏控件的光标
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "HideCaret")]
        public static extern bool HideCaret(IntPtr hWnd);
        #endregion
    }
}
