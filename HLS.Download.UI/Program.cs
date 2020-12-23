using System;
using System.Windows.Forms;
using System.Threading;
using System.Security.Principal;
using System.Diagnostics;
using WinAPI;
using System.Text;

namespace HLS.Download.UI
{
    static class Program
    {
        private static Mutex mutex;
        private static MainForm mainWindow;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            bool isNewInstanceCreate;
            mutex = new Mutex(true, Application.ProductName, out isNewInstanceCreate);
            if (isNewInstanceCreate)
            {
                if (args.Length == 0)//仅在手动开启非调用时提示“以管理员运行”
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();//获得当前登录的Windows用户标识
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    if (!principal.IsInRole(WindowsBuiltInRole.Administrator))//判断当前登录用户是否为管理员
                    {
                        MessageBox.Show("当前用户不是管理员！（建议设置以管理员身份运行程序）\r\n部分功能可能会出现异常。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (mainWindow == null)
                {
                    GC.KeepAlive(mutex);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    if (args.Length == 0)
                        mainWindow = new MainForm();
                    else
                        mainWindow = new MainForm(args);
                    Application.Run(mainWindow);
                }
            }
            else
            {
                //MessageBox.Show("程序已经运行，请勿重复打开！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (args.Length == 0)//不包含任何参数时，激活到最前
                    SwitchToCurrentInstance();//激活已经打开的程序
                else//包含参数时，传递参数给m3u8dl.service.exe，由它传给主程序（正确处理进程间通信）
                {
                    try
                    {
                        #region 传递给m3u8dl.service，再传递给HLSDownloader，容易丢失引号“”，造成参数截断，故更换方法
                        //var cmdStr = Path.Combine(Application.StartupPath, "Service\\m3u8dl.service.exe") + " " + String.Join(" ", args);
                        //MessageBox.Show(cmdStr);

                        //var serviceAppPath = Path.Combine(Application.StartupPath, "Service\\m3u8dl.service.exe");
                        //if (File.Exists(serviceAppPath))
                        //{
                        //ProcessStartInfo psi = new ProcessStartInfo(serviceAppPath,
                        //String.Join(" ", args));
                        ////设定工作目录，是为了让m3u8dl.service窗体必定隐藏
                        //psi.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
                        //psi.UseShellExecute = false;
                        //psi.CreateNoWindow = true;
                        //Process.Start(psi);
                        //}
                        #endregion

                        var mainWindowHandle = GetRunningSingletonInstanceWindowHandle();
                        if (mainWindowHandle != IntPtr.Zero) //向主程序窗口发送Windows消息，传入参数信息
                        {
                            string strArgs = String.Join("|", args);
                            int size = Encoding.Unicode.GetByteCount(strArgs);
                            COPYDATASTRUCT cds;
                            cds.dwData = mainWindowHandle;
                            cds.cbData = size + 1;//最后以null结束，占一位
                            cds.lpData = strArgs;
                            ForSystem.SendMessage(mainWindowHandle, ForSystem.WM_COPYDATA, 0, ref cds);
                        }
                    }
                    catch { }
                }
                CloseSingletonInstanceMutex();
                Application.Exit();
            }
        }

        public static void CloseSingletonInstanceMutex()
        {
            mutex?.Dispose();
            mutex = null;
            mainWindow = null;
        }

        private static void SwitchToCurrentInstance()
        {
            var singletonInstanceWindowHandle = GetRunningSingletonInstanceWindowHandle();
            if (singletonInstanceWindowHandle == IntPtr.Zero)
                return;
            if (ForSystem.IsWindowVisible(singletonInstanceWindowHandle) == 0)//最小化到托盘，窗体不可见，显示
            {
                ForSystem.ShowWindow(singletonInstanceWindowHandle, (int)ForSystem.SW_SHOW);
                ForSystem.SetForegroundWindow(singletonInstanceWindowHandle);//设置到最前显示
            }
            else if (ForSystem.IsIconic(singletonInstanceWindowHandle) != 0)//最小化到任务栏，还原显示
                ForSystem.ShowWindow(singletonInstanceWindowHandle, (int)ForSystem.SW_RESTORE);
            else//正常状态，设置到最前显示
                ForSystem.SetForegroundWindow(singletonInstanceWindowHandle);
        }

        private static IntPtr GetRunningSingletonInstanceWindowHandle()
        {
            var windowHandle = IntPtr.Zero;
            var currentProcess = Process.GetCurrentProcess();
            foreach (var enumeratedProcess in Process.GetProcessesByName(currentProcess.ProcessName))
            {
                if (enumeratedProcess.Id != currentProcess.Id && enumeratedProcess.MainModule.FileName == currentProcess.MainModule.FileName && enumeratedProcess.MainWindowHandle != IntPtr.Zero)
                    windowHandle = enumeratedProcess.MainWindowHandle;
            }

            if (windowHandle == IntPtr.Zero) //按进程名查找失败，按窗体名称查找一次
            {
                windowHandle = ForSystem.FindWindow(null, "HLS下载器");
            }

            return windowHandle;
        }

    }
}
