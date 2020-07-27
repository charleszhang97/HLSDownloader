using System;
using System.Windows.Forms;

namespace m3u8dl.service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            M3u8dlServiceForm serviceWindow;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
                serviceWindow = new M3u8dlServiceForm();
            else
                serviceWindow = new M3u8dlServiceForm(args);
            Application.Run(serviceWindow);
        }
    }
}
