using System;
using System.Windows.Forms;
using System.Threading;
using System.Security.Principal;

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
                MessageBox.Show("程序已经运行，请勿重复打开！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
        }

        public static void CloseSingletonInstanceMutex()
        {
            mutex?.Dispose();
            mutex = null;
            mainWindow = null;
        }
    }
}
