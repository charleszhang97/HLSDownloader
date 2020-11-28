using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace m3u8dl.service
{
    /* 进程间使用无效，目标进程会直接卡死。推测同一进程内可能有效
    /// <summary>
    /// 自定义传输数据的结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct STRSTRUCT
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string str;
    }*/

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

    public partial class M3u8dlServiceForm : Form
    {
        private string[] parameters;
        private bool windowHidden = false;
        //private const int USER = 0x0400;//用户自定义消息的开始数值
        //private const int WM_POST_ARGS = USER + 1;//自定义POST传送参数消息ID
        //private const int WM_SEND_ARGS = USER + 2;//自定义SEND传送参数消息ID
        private const int WM_COPYDATA = 0x004A;//copydata消息ID

        //[DllImport("user32.dll ", CharSet = CharSet.Ansi)]
        //private static extern bool PostMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        //进程间使用无效，目标进程会直接卡死。推测同一进程内可能有效
        //[DllImport("User32.dll", CharSet = CharSet.Ansi, EntryPoint = "SendMessage")]
        //private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, ref STRSTRUCT lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, ref COPYDATASTRUCT lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //[DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "FindWindowEx")]
        //private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        public M3u8dlServiceForm()
        {
            InitializeComponent();

            //Environment.CurrentDirectory == C:\Windows\system32，说明是系统调用，工作目录是system32
            //这种情况下没必要显示窗口，没必要输出日志，处理参数后快速调用主窗体即可
            //其他调用情况下，显示窗体、输出日志
            //增加判断，Environment.CurrentDirectory != Application.StartupPath，Chrome调用工作目录是其安装目录
            if (Environment.CurrentDirectory == Environment.GetFolderPath(Environment.SpecialFolder.System) ||
                Environment.CurrentDirectory != Application.StartupPath
                )
            {
                this.Hide();
                windowHidden = true;
            }
        }

        public M3u8dlServiceForm(string[] args) : this()
        {
            if (args.Length == 1 && args[0].IndexOf("://") > -1)//Url协议对程序进行参数调用
            {
                if (args[0].IndexOf('%') > -1) //Chrome会将|等分割符号编码，这里判断有编码的进行解码
                    args[0] = Uri.UnescapeDataString(args[0]);

                /* 为了更高的通用性，修改以‘://’来截取
                if (args[0].ToLower().StartsWith("m3u8dl://"))
                    //args[0] = args[0].Replace("m3u8dl://", "");
                    args[0] = args[0].Substring("m3u8dl://".Length);
                */
                args[0] = args[0].Substring(args[0].IndexOf("://") + 3);

                //移除浏览器等调用URL Protocol时自动在末尾添加的/（不是所有浏览器都添加），因为base64编码本身也包含/，为避免误伤正确数据，
                //只能根据长度是否为4的倍数判断是否多出 /
                if (args[0].EndsWith("/"))
                {
                    var lastSepartor = args[0].LastIndexOf('|');
                    var lastPart = "";
                    if (lastSepartor > -1)
                        lastPart = args[0].Substring(lastSepartor + 1);
                    else
                        lastPart = args[0];

                    var lastEquIndex = lastPart.IndexOf('=');
                    if (lastEquIndex > -1) //找不到=的之后参数处理会直接忽略
                    {
                        var value = lastPart.Substring(lastEquIndex + 1);
                        if (value.Length % 4 == 1 || value == "/") //除4后余1，多出个/，或者就等于/（要么空要么4位及其倍数）
                            args[0] = args[0].Substring(0, args[0].Length - 1);
                    }
                }

                //if (args[0].IndexOf('|') > -1)
                string[] paras = args[0].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                this.parameters = new string[paras.Length + 1];
                this.parameters[0] = "decode=true";//标记参数值需要解码
                paras.CopyTo(this.parameters, 1);
            }
            else
            {
                this.parameters = args;
            }
        }

        private void M3u8dlServiceForm_Load(object sender, EventArgs e)
        {
            #region
            /*
            WriteLog("运行路径", Environment.CurrentDirectory);
            WriteLog("运行路径", this.GetType().Assembly.Location);
            WriteLog("运行路径", Process.GetCurrentProcess().MainModule.FileName);
            WriteLog("运行路径", AppDomain.CurrentDomain.BaseDirectory);
            WriteLog("运行路径", AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            WriteLog("运行路径", Application.StartupPath);
            WriteLog("运行路径", Application.ExecutablePath);
            WriteLog("运行路径", System.IO.Directory.GetCurrentDirectory());
            //运行结果依次如下：
            //运行路径：G:\Windows\system32
            //运行路径：I:\93.Ye.M3U8.Downloader\Ye.M3U8.Downloader.exe
            //运行路径：I:\93.Ye.M3U8.Downloader\Ye.M3U8.Downloader.exe
            //运行路径：I:\93.Ye.M3U8.Downloader\
            //运行路径：I:\93.Ye.M3U8.Downloader\
            //运行路径：I:\93.Ye.M3U8.Downloader
            //运行路径：I:\93.Ye.M3U8.Downloader\Ye.M3U8.Downloader.exe
            //运行路径：G:\Windows\system32
            */
            #endregion

            var mainWindowHandle = GetRunningSingletonInstanceWindowHandle();
            var mainExePath = "";
            if (mainWindowHandle != IntPtr.Zero)
            {
                WriteLog("主程序信息", "主程序窗口句柄：" + (int)mainWindowHandle);
            }
            else
            {
                mainExePath = Path.Combine(Directory.GetParent(Application.StartupPath).FullName, "HLSDownloader.exe");
                if (File.Exists(mainExePath))
                    WriteLog("主程序信息", "主程序执行路径：" + mainExePath);
                else
                {
                    mainExePath = "";
                    WriteLog("主程序信息", "无法找到主程序执行路径！");
                }
            }
            var TAG = "处理传入参数";
            if (parameters != null && parameters.Length != 0)
            {
                WriteLog(TAG, "处理中...");
                string args = "";

                var argumentSeparatorChar = ' ';
                var formatString = argumentSeparatorChar + "{0}=\"{1}\"";//带参数启动主程序，分隔符“ ”,value值用引号包起来
                if (mainWindowHandle != IntPtr.Zero)//主程序窗口句柄SendMessage方式，分隔符|，value值不处理
                { 
                    argumentSeparatorChar = '|';
                    formatString = argumentSeparatorChar + "{0}={1}";
                }
                try
                {
                    foreach (var str in parameters)
                    {
                        var equIndex = str.IndexOf('=');
                        if (equIndex > -1)
                        {
                            var _key = str.Substring(0, equIndex);
                            if (equIndex == str.Length - 1)
                            {
                                WriteLog(TAG, string.Format("未指定参数“{0}”的值！", _key));
                                continue;
                            }
                            var _value = str.Substring(equIndex + 1);
                            switch (_key)
                            {
                                case "decode":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    args += string.Format(formatString, _key, _value);
                                    break;
                                case "config-name":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    args += string.Format(formatString, _key, _value);
                                    break;
                                case "aria2c-args-append":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    args += string.Format(formatString, _key, _value);
                                    break;
                                case "max-speed-limit":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    int iDownSpeedLimit;
                                    if (int.TryParse(_value, out iDownSpeedLimit))
                                    {
                                        args += string.Format(formatString, _key, _value);
                                    }
                                    else
                                        WriteLog(TAG, string.Format("参数值“{0}”不正确！", _value));
                                    break;
                                case "name-url-string":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    var NameAndUrls = _value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    Dictionary<string, string> mNameAndUrlMap = new Dictionary<string, string>();
                                    foreach (var _str in NameAndUrls)
                                    {
                                        var name_url = _str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (name_url.Length >= 2)
                                            //mNameAndUrlMap.Add(name_url[0], name_url[1]);
                                            mNameAndUrlMap.Add(name_url[0], _str.Substring(_str.IndexOf(':') + 1));//未加密时，url中可能存在：，所以截取剩余字符串
                                        else
                                            WriteLog(TAG, string.Format("参数值“{0}”格式不正确！已忽略", _str));
                                    }
                                    if (mNameAndUrlMap.Count > 0)
                                    {
                                        var tmpStr = "";
                                        foreach (var _task in mNameAndUrlMap)
                                        {
                                            tmpStr += _task.Key + ":" + _task.Value + ";";
                                        }
                                        args += string.Format(formatString, _key, tmpStr);
                                    }
                                    break;
                                case "action-after-downloaded":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    args += string.Format(formatString, _key, _value);
                                    break;
                                case "":
                                    WriteLog(TAG, "参数名为空！");
                                    break;
                                default:
                                    WriteLog(TAG, "不支持的参数名：" + _key);
                                    break;
                            }
                        }
                        else
                        {
                            WriteLog(TAG, string.Format("不正确的参数“{0}”！", str));
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(TAG, "出现异常");
                    WriteLog(TAG, ex.Message);
                }
                finally
                {
                    if(args!="")//移除行首多余的那个分隔符
                        args = args.Substring(1);
                    WriteLog(TAG, "完整参数：" + args);
                    WriteLog(TAG, "处理结束");
                }

                if (mainWindowHandle != IntPtr.Zero) //向主程序窗口发送Windows消息，传入参数信息
                {
                    int size = Encoding.Unicode.GetByteCount(args);
                    COPYDATASTRUCT cds;
                    cds.dwData = this.Handle;
                    cds.cbData = size + 1;//最后以null结束，占一位
                    cds.lpData = args;
                    int iResult = SendMessage(mainWindowHandle, WM_COPYDATA, 0, ref cds);
                    WriteLog("Send参数", string.Format("主程序窗口句柄：{0}，消息ID：{1}，参数字符串长度：{2}", (int)mainWindowHandle, WM_COPYDATA, size));
                    WriteLog("Send参数", "Send结果：" + iResult);

                    #region 以下代码无效，保留作为参考
                    /* 进程间使用无效，目标进程会直接卡死。推测同一进程内可能有效
                    //创建托管对象
                    STRSTRUCT ss = new STRSTRUCT();
                    ss.str = args;
                    //分配非托管内存，并获取非托管内存地址起始位置指针
                    int size = Marshal.SizeOf(ss);
                    IntPtr buffer = Marshal.AllocHGlobal(size);
                    //将托管对象拷贝到非托管内存
                    Marshal.StructureToPtr(ss, buffer, false);
                    //调用SendMessage发送数据
                    int iResult = SendMessage(mainWindowHandle, WM_SEND_ARGS, 0, ref ss);
                    WriteLog("Send参数", string.Format("主程序窗口句柄：{0}，自定义消息ID：{1}，参数字符串指针：{2}，参数字符串长度：{3}", (int)mainWindowHandle, WM_SEND_ARGS, buffer.ToInt32().ToString(), size));
                    WriteLog("Send参数", "Send结果：" + iResult);
                    Marshal.FreeHGlobal(buffer);
                    */

                    /* 进程间使用无效，目标进程会直接卡死
                    //先将字符串转化成字节方式
                    Byte[] btData = Encoding.Default.GetBytes(args);
                    //申请非托管空间
                    IntPtr m_ptr = Marshal.AllocHGlobal(btData.Length);
                    //给非托管空间清0
                    Byte[] btZero = new Byte[btData.Length + 1]; //一定要加1,否则后面是乱码
                    Marshal.Copy(btZero, 0, m_ptr, btZero.Length);
                    //给指针指向的空间赋值
                    Marshal.Copy(btData, 0, m_ptr, btData.Length);
                    //应该接收方释放
                    //Marshal.FreeHGlobal(m_ptr);
                    */

                    /* 进程间使用无效，目标进程会直接卡死。推测同一进程内可能有效
                    IntPtr sPtr = Marshal.StringToHGlobalUni(args);
                    //int iSize = Marshal.SizeOf(args);//不能作为非托管结构进行封送处理；无法计算有意义的大小或偏移量。
                    int iSize = Encoding.Unicode.GetByteCount(args);
                    //int iSize = args.Length;
                    bool isOK = PostMessage(mainWindowHandle, WM_POST_ARGS, sPtr, new IntPtr(iSize));
                    WriteLog("Post参数", string.Format("主程序窗口句柄：{0}，自定义消息ID：{1}，参数字符串指针：{2}，参数字符串长度：{3}", (int)mainWindowHandle, WM_POST_ARGS, (int)sPtr, iSize));
                    WriteLog("Post参数", "Post结果：" + isOK);
                    */
                    #endregion
                }
                else //主程序窗口句柄无效，带参数启动主程序
                {
                    if (mainExePath != "")
                    {
                        Process newProcess = new Process();
                        newProcess.StartInfo.FileName = mainExePath;
                        newProcess.StartInfo.Arguments = args;
                        newProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(mainExePath);
                        newProcess.StartInfo.UseShellExecute = false;
                        newProcess.Start();
                    }
                }

            }
            else
            {
                WriteLog(TAG, "没有需要处理的参数！");
                //参数为空，两种情况：（1）主程序已启动，什么不做；（2）未启动，打开
                if (mainWindowHandle == IntPtr.Zero && mainExePath != "")
                { //执行路径不为空，主程序窗口句柄无效
                    Process newProcess = new Process();
                    newProcess.StartInfo.UseShellExecute = false;
                    newProcess.StartInfo.FileName = mainExePath;
                    newProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(mainExePath);
                    newProcess.Start();
                }
            }

            if (windowHidden)
            {
                this.Dispose(true);
                Application.Exit();
            }
        }

        private void WriteLog(String tag, String info)
        {
            if (windowHidden)//窗体隐藏，直接返回
                return;
            if (txbLog.InvokeRequired)
            {
                while (!txbLog.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (txbLog.Disposing || txbLog.IsDisposed)
                        return;
                }
                txbLog.Invoke(new MethodInvoker(() => WriteLog(tag, info)));
                return;
            }

            if (tag == null)
                txbLog.AppendText(info);
            else
            {
                txbLog.AppendText(tag);
                txbLog.AppendText("：");
                txbLog.AppendText(info);
                txbLog.AppendText(Environment.NewLine);
            }
        }


        private IntPtr GetRunningSingletonInstanceWindowHandle()
        {
            var windowHandle = IntPtr.Zero;
            foreach (var enumeratedProcess in Process.GetProcessesByName("HLSDownloader"))
            {
                if (enumeratedProcess.MainWindowHandle != IntPtr.Zero)
                    windowHandle = enumeratedProcess.MainWindowHandle;
            }
            if (windowHandle == IntPtr.Zero) //按进程名查找失败
            {
                windowHandle = FindWindow(null, "HLS视频流下载器");
            }
            return windowHandle;
        }

        private void txbLog_TextChanged(object sender, EventArgs e)
        {
            txbLog.SelectionStart = txbLog.Text.Length;
            txbLog.ScrollToCaret();
        }
    }
}
