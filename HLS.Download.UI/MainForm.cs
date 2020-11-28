using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Media;
using System.Threading.Tasks;
using System.IO.Compression;
using FlyVR.Aria2;
using HLS.Download.Models;
using WinAPI;
using BrowserHelper;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace HLS.Download.UI
{
    //================================================================================
    //灵感标记规则：已完成的打√，不准备完成的打×，尚未未处理的打？
    //================================================================================
    //√数量从1K多激增到4K多，而且异常后，停止计时器了还是在执行！
    //？数量多时，加大刷新间隔？可以缓解？
    //×设计新的思路，假如按顺序下载，则只遍历当前下载数量的16*N倍？前方全下载，后面的全是未下载，中间的部分需要更新状态？
    //？对于TS文件来说，C#端不需要监听状态。只要监听m3u8即可？那如何感知到所有文件下载完成了？是否有全局状态感知？再触发合并？
    //×-y 自动覆盖
    //？-report 自动分析日志是否有遗漏的切片或别的错误，在界面增加正则表达式框，符合的行输出到日志列表。
    //？检测report没错误时，自动删除临时文件的功能？
    //√自解压包能压缩到14MB
    //√检测包含 *.TS 所在的 *.m3u8 文件，然后修改路径？还是在解析时顺便修改好另存为一个新的 .m3u8 文件？
    //？根据Map输出友好的文件名提示，而不是任务ID
    //？增加一个删除Setion的功能？
    //×增加一个自动分组的显示方式？看看是否支持 Aria2
    //×缺少时，手工下载 aria2c ,ffmpeg.exe提醒？可以的结构目录树展示？
    //？ff 解压后的路径 怎么设置工作目录？否则无法识别！！
    //？或者清单文件里直接设置 ts 文件的绝对路径？ 导出文件也设置为绝对路径！
    //？需要去掉 / 符号！！！本地路径才识别。或者换为\符号？
    //？可以使用COPY /B *.ts name.mp4 来解决无法通过 ffmpeg 合并的视频文件。如提示 Timestamps are unset in a packet for stream 0
    public partial class MainForm : Form
    {
        private string[] parameters;
        private const int USER = 0x0400;//用户自定义消息的开始数值
        private const int WM_POST_ARGS = USER + 1;//自定义POST传送参数消息ID
        private const int WM_SEND_ARGS = USER + 2;//自定义SEND传送参数消息ID
        private const int WM_COPYDATA = 0x004A;//copydata消息ID
        private Aria2c mAria2c;
        private decimal? mSelectedBandwidth = null;
        private string mSelectedUserAgent;
        private string mCurrentDownLoadDir;
        private bool mAllDownLoadComplete = true;
        private bool mAllPause = false;
        //private Dictionary<string, string> mUrlAndNameMap = new Dictionary<string, string>();
        private Dictionary<string, string> mUrlAndDownloadDirMap = new Dictionary<string, string>();
        //private Dictionary<string, string> mPidAndUrlMap = new Dictionary<string, string>();
        private Dictionary<string, int> mDownDirAndTaskCountMap = new Dictionary<string, int>();
        //private Thread readM3U8Thread;
        //private System.Timers.Timer timer;
        private System.Windows.Forms.Timer timer;//修改Timer类型
        private TimeSpan ts;
        private long DownloadCompletedData = 0;
        private int DownloadTotalSeconds = 0;
        private static String[] units = new String[] { "B", "KB", "MB", "GB", "TB", "PB" };
        private string mLastTaskDir;
        private long LastTotalDoneTasks;
        private bool mPlayCompletedSound;
        private bool mExitRequireConfirm;
        private static object SequenceLock = new object();

        public MainForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            if (HighDpiCheck())//Windows系统原始缩放以及高DPI情况下界面处理
            {
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    if (g.DpiX == 96f) //缩放百分比为100%时（程序代码在125%环境开发）
                    {
                        this.Font = new Font(Font.Name, this.Font.Size * 1.25f, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
                    }
                    else if (g.DpiX != 120f) //高分屏、高DPI（150% 200%） 缩放125%像素120
                    {
                        this.Font = new Font(Font.Name, this.Font.Size * g.DpiX / 120f, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
                    }
                }
            }

        }

        /// <summary>
        /// 读取配置判断是否检查DPI，便于手动设定跳过
        /// </summary>
        /// <returns></returns>
        private bool HighDpiCheck()
        {
            bool isCheck = true;
            var iniPath = Path.Combine(Environment.CurrentDirectory, "settings.ini");
            if (File.Exists(iniPath))
            {
                bool.TryParse(INIHelper.Read("General", "HighDpiCheck", "", iniPath), out isCheck);
            }
            return isCheck;
        }

        public MainForm(string[] args) : this()
        {
            this.SetArgs(args);
        }

        public void SetArgs(string[] args)
        {
            //this.parameters = args;
            if (args.Length == 1 && args[0].IndexOf("://") > -1)//Url协议对程序进行参数调用
            {
                if (args[0].IndexOf('%') > -1) //Chrome会将|等分割符号编码，这里判断有编码的进行解码
                    args[0] = Uri.UnescapeDataString(args[0]);

                /*为了更高的通用性，修改以‘://’来截取
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

        public void EnabledParameters()
        {
            if (parameters != null && parameters.Length != 0)
            {
                var TAG = "启用传入参数";
                WriteLog(null, Environment.NewLine);
                WriteLog(TAG, "执行中...");
                bool isAria2cNotRunning = mAria2c == null;
                bool isStartAria2c = false;
                bool isApplyMaxSpeed = false;
                bool isStartDownTask = false;
                try
                {
                    bool isNeedDecode = false;
                    foreach (var str in parameters)
                    {
                        if (str.StartsWith("decode"))
                        {
                            //isNeedDecode = true;
                            isNeedDecode = str == "decode=true";//str == "decode=false"时，isNeedDecode = false
                            continue;
                        }
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
                            if (isNeedDecode && _key != "name-url-string")
                            {
                                _value = DecodeBase64(_value);
                            }
                            switch (_key)
                            {
                                case "config-name":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    if (isAria2cNotRunning)
                                    {
                                        if (cbbConfigFileName.Items.Contains(_value))
                                        {
                                            cbbConfigFileName.Text = _value;
                                            isStartAria2c = true;
                                        }
                                        else
                                            WriteLog(TAG, string.Format("Aria2配置文件“{0}”没有找到，将忽略该参数", _value));
                                    }
                                    else
                                        WriteLog(TAG, string.Format("Aria2已经启动过，将忽略该参数“{0}”（重启Aria2影响之前任务，故忽略）", _key));
                                    break;
                                case "aria2c-args-append":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    if (isAria2cNotRunning)
                                    {
                                        txbAria2Args.Text = _value;
                                        isStartAria2c = true;
                                    }
                                    else
                                    {
                                        //WriteLog(TAG, string.Format("Aria2已经启动过，将忽略该参数“{0}”", _key));
                                        WriteLog(TAG, string.Format("Aria2已经启动过，将尝试拆分参数值“{0}”，依次应用到全局设置", _value));
                                        WriteLog(TAG, "【注意】可能造成之前未完成的任务下载失败。");

                                        string pattern = "--(?<OptionKey>[\\w-]+)=(?<OptionValue>[^ ]+)|(?<OptionValue>['\"][^'\"]+['\"])";
                                        foreach (Match match in Regex.Matches(_value, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                                        {
                                            try {
                                                Aria2cOption option = new Aria2cOption();
                                                var optionValue = match.Groups["OptionValue"].Value.Trim("'\"".ToCharArray());
                                                option.SetOption(match.Groups["OptionKey"].Value, optionValue);
                                                bool isSuccess = mAria2c.ChangeGlobalOption(option);
                                                WriteLog(TAG, string.Format("Aria2参数“{0}”应用到全局{1}；", match.Groups["OptionKey"].Value, 
                                                    isSuccess ? "成功":"失败"));
                                            }
                                            catch { WriteLog(TAG, string.Format("Aria2参数“{0}”应用到全局失败；", match.Groups["OptionKey"].Value)); }
                                        }
                                    }
                                    break;
                                case "max-speed-limit":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    int iDownSpeedLimit;
                                    if (int.TryParse(_value, out iDownSpeedLimit))
                                    {
                                        txbMaxDownloadSpeed.Text = _value;
                                        isApplyMaxSpeed = true;
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
                                        {
                                            var _url = _str.Substring(_str.IndexOf(':') + 1);//未加密时，url中可能存在：，所以截取剩余字符串
                                            if (isNeedDecode)
                                                //mNameAndUrlMap.Add(DecodeBase64(name_url[0]), DecodeBase64(name_url[1]));
                                                mNameAndUrlMap.Add(DecodeBase64(name_url[0]), DecodeBase64(_url));
                                            else
                                                //mNameAndUrlMap.Add(name_url[0], name_url[1]);
                                                mNameAndUrlMap.Add(name_url[0], _url);
                                        }
                                        else
                                            WriteLog(TAG, string.Format("参数值“{0}”格式不正确！已忽略", _str));
                                    }
                                    if (mNameAndUrlMap.Count > 0)
                                    {
                                        var tmpStr = ""; int _index = 0;
                                        foreach (var _task in mNameAndUrlMap)
                                        {
                                            _index++;
                                            tmpStr += _task.Key + "|" + _task.Value + Environment.NewLine;
                                            WriteLog(TAG, string.Format(_index + "、文件名：{0}，下载地址：{1}；", _task.Key, _task.Value));
                                        }
                                        txbUrls.Text = tmpStr;
                                        isStartDownTask = true;
                                    }
                                    break;
                                case "action-after-downloaded":
                                    WriteLog(TAG, string.Format("参数名：{0}，参数值：{1}", _key, _value));
                                    if (cbbAction.Items.Contains(_value))
                                    {
                                        cbbAction.Text = _value;
                                    }
                                    else
                                        WriteLog(TAG, string.Format("参数值“{0}”不正确！", _value));
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
                finally { WriteLog(TAG, "执行结束"); }
                //参数的应用需要有先后顺序，不然可能出现无效的情况，故上面只赋值，以下按序执行
                if (isStartAria2c)
                    btnStartAria2_Click(btnStartAria2, null);
                if (isApplyMaxSpeed)
                    btnApplyMaxSpeed_Click(btnApplyMaxSpeed, null);
                if (isStartDownTask)
                {
                    btnDoIt_Click(btnDoIt, null);
                }
            }
        }

        private string EncodeBase64(string str)
        {
            return BrowserHelper.Base64.EncodeBase64("utf-8", str);
        }

        private string DecodeBase64(string str)
        {
            return BrowserHelper.Base64.DecodeBase64("utf-8", str);
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    //COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    //Type t = cds.GetType();
                    //cds = (COPYDATASTRUCT)m.GetLParam(t);
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                    string receiveData = cds.lpData;

                    WriteLog(null, Environment.NewLine);
                    WriteLog("接收到参数", receiveData);
                    //WriteLog("参数长度", Encoding.Unicode.GetByteCount(receiveData).ToString());
                    WriteLog("参数长度", receiveData.Length.ToString());
                    WriteLog("来源窗口句柄", cds.dwData.ToInt32().ToString());

                    //设定参数，启用参数
                    //this.SetArgs(receiveData.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    //使用空格切分参数存在意外情况，可能将参数截断，改成以|分割
                    this.SetArgs(receiveData.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    this.EnabledParameters();
                    break;
                case WM_SEND_ARGS:
                    try
                    {
                        #region 以下代码无效，保留作为参考
                        /* 进程间使用无效，本进程会直接卡死
                        int length = m.LParam.ToInt32();
                        //string args = Marshal.PtrToStringUni(m.WParam, length);
                        STRSTRUCT ss = (STRSTRUCT)Marshal.PtrToStructure(m.WParam, typeof(STRSTRUCT));
                        string args = ss.str;
                        //string args = Marshal.PtrToStringUni(m.WParam);
                        //string args = Marshal.PtrToStructure(m.WParam, typeof(string)) as string; //没有为该对象定义无参数的构造函数。
                        Marshal.FreeHGlobal(m.WParam);
                        WriteLog("接收参数", args);
                        */

                        /* 进程间使用无效，本进程会直接卡死
                        STRSTRUCT ss = (STRSTRUCT)m.GetLParam(typeof(STRSTRUCT));
                        string args = ss.str;
                        WriteLog("接收参数", args);
                        WriteLog("参数长度", Encoding.Default.GetByteCount(args).ToString());
                        */
                        #endregion
                        WriteLog("接收参数hwnd", this.Handle.ToInt32().ToString());
                        WriteLog("接收参数WMsg", m.Msg.ToString());
                        WriteLog("接收参数WParam", m.WParam.ToInt32().ToString());
                        WriteLog("接收参数LParam", m.LParam.ToInt32().ToString());
                    }
                    catch (Exception ex)
                    {
                        WriteLog("接收参数", "出现异常：" + ex.Message);
                    }
                    break;
                case WM_POST_ARGS:
                    try
                    {
                        #region 以下代码无效，保留作为参考
                        /* 进程间使用无效，本进程会直接卡死
                        int length = m.LParam.ToInt32();
                        string args = Marshal.PtrToStringAnsi(m.WParam, length);
                        //string args = Marshal.PtrToStringAnsi(m.WParam);
                        //string args = Marshal.PtrToStructure(m.WParam, typeof(string)) as string; //没有为该对象定义无参数的构造函数。
                        Marshal.FreeHGlobal(m.WParam);
                        WriteLog("接收参数", args);
                        */
                        /*
                        int length = m.LParam.ToInt32();
                        IntPtr pData = m.WParam;
                        byte[] byData = new byte[length];
                        Marshal.Copy(pData, byData, 0, length);
                        string strData = Encoding.Default.GetString(byData);
                        WriteLog("接收参数", strData);
                        Marshal.FreeHGlobal(m.WParam);
                        */
                        #endregion
                        WriteLog("接收参数hwnd", this.Handle.ToInt32().ToString());
                        WriteLog("接收参数WMsg", m.Msg.ToString());
                        WriteLog("接收参数WParam", m.WParam.ToInt32().ToString());
                        WriteLog("接收参数LParam", m.LParam.ToInt32().ToString());
                    }
                    catch (Exception ex)
                    {
                        WriteLog("接收参数", "出现异常：" + ex.Message);
                    }
                    break;
                default:
                    base.DefWndProc(ref m);//调用基类函数，以便系统处理其它消息
                    break;
            }
        }

        private string[] getDownloadUrls()
        {
            return txbUrls.Text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        private void txbUrls_TextChanged(object sender, EventArgs e)
        {
            if (txbUrls.Text.Trim() == "")
                return;
            string[] lines = txbUrls.Text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            Uri tmpUri;
            bool flag = false;
            string strMergePath = DateTime.Now.ToString("yyyyMMddHHmmss");
            var ckb = sender as CheckBox;
            bool isFromMergeTasks = false;
            if (ckb != null && ckb.Name == "ckbMergeTasks")
                isFromMergeTasks = true;
            for (int i = 0; i < lines.Length; i++)
            {
                var idx = lines[i].IndexOf('|');
                if (idx > -1 && !isFromMergeTasks)
                    continue;
                var originalText = lines[i];
                if (idx > -1)
                {
                    if (idx == originalText.Length - 1)
                        originalText = "";
                    else
                        originalText = originalText.Substring(idx + 1);
                }
                flag = Uri.TryCreate(originalText, UriKind.Absolute, out tmpUri);//某一行可以转换为URL，说明是网址，生成临时名称
                if (flag)
                {
                    if (!isFromMergeTasks || !ckbMergeTasks.Checked)
                        lines[i] = string.Format("{0}{1}|{2}", DateTime.Now.ToString("yyyyMMddHHmmss"), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()), originalText);
                    else
                        lines[i] = string.Format("{0}|{1}", strMergePath, originalText);

                }
            }
            if (flag)//文本框需要更新时
            {
                txbUrls.TextChanged -= new EventHandler(txbUrls_TextChanged);
                txbUrls.Text = string.Join(Environment.NewLine, lines) + Environment.NewLine;
                txbUrls.SelectionStart = txbUrls.Text.Length;
                txbUrls.TextChanged += new EventHandler(txbUrls_TextChanged);
            }

            btnDoIt.Text = string.Format("下载\n({0})", lines.Length);
        }

        private void btnDoIt_Click(object sender, EventArgs e)
        {
            if (txbUrls.Text.Trim() == "")
                return;
            //防止没启动Aria2
            if (mAria2c == null)
                btnStartAria2_Click(btnStartAria2, null);
            var TAG = "下载";
            if (mAria2c == null)
            {
                WriteLog(null, Environment.NewLine);
                WriteLog(TAG, "Aria2未启动！");
                return;
            }

            ((Button)sender).Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行中");
            try
            {
                if (!mAria2c.IsLoading)
                {
                    WriteLog(TAG, string.Format("OnAllStart={0}; 下载开始！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    DownloadCompletedData = 0;
                    txbTotalData.Text = "";
                    DownloadTotalSeconds = 0;
                    txbTotalTime.Text = "";
                    RefreshDownloadProgress(0);
                    RefreshNotifyIconTrayText("");

                    //mUrlAndNameMap.Clear();
                    mUrlAndDownloadDirMap.Clear();
                    //mPidAndUrlMap.Clear();
                }

                var downloadDir = Aria2cRuntime.DownLoadDirectory;
                List<string> urls = new List<string>();
                //Regex reg = new Regex(@"[\\\/:*?""<>|]{1,}", RegexOptions.Singleline);
                foreach (var s in getDownloadUrls())
                {
                    var urlAndName = s.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (urlAndName.Length != 2)
                    {
                        WriteLog(TAG, "以下格式有误");
                        WriteLog(TAG, s);
                        WriteLog(TAG, "执行结束");
                        return;
                    }
                    var name = urlAndName[0];
                    var url = urlAndName[1];

                    //处理name中特殊符号，避免创建目录抛错                    
                    //name = reg.Replace(name, " ");
                    foreach (var chr in Path.GetInvalidFileNameChars())
                    {
                        if (name.IndexOf(chr) > -1)
                            name = name.Replace(chr, ' ');
                    }

                    var dir = Path.Combine(downloadDir, name);//使用文件名作为新目录来临时存储多个切片。
                    Directory.CreateDirectory(dir);

                    if (url.IndexOf(':') == 1 && File.Exists(url))
                    {
                        if (Path.GetExtension(url).ToLower() != ".m3u8")
                        {
                            WriteLog(TAG, Path.GetFileName(url) + "：不支持的文件类型！已跳过");
                            continue;
                        }
                        WriteLog(TAG, string.Format("文件名={0}", name));
                        WriteLog(TAG, string.Format("本地路径={0}", url));
                        WriteLog(TAG, string.Format("下载目录={0}", dir.Replace(downloadDir, "~")));
                        mLastTaskDir = dir;

                        var m3u8Path = Path.Combine(dir, Path.GetFileName(url));
                        if (url != m3u8Path)
                            File.Copy(url, m3u8Path, true);
                        List<string> paras = new List<string>();
                        paras.AddRange(new string[] { name, m3u8Path });
                        //ReadLocalM3U8(m3u8Path);//直接调用，子方法包含异步方法，界面卡住

                        //readM3U8Thread = new Thread(new ParameterizedThreadStart(ReadLocalM3U8));
                        //readM3U8Thread.IsBackground = true;
                        //readM3U8Thread.Priority = ThreadPriority.AboveNormal;
                        //readM3U8Thread.Start(paras);

                        //Task taskReadM3U8 = Task.Run(() => { ReadLocalM3U8(paras);});
                        Task taskReadM3U8 = new Task(ReadLocalM3U8, (object)paras);//传参，必须是obj
                        taskReadM3U8.Start();
                        taskReadM3U8.Wait(5000);
                        if (taskReadM3U8.IsCompleted)
                            taskReadM3U8.Dispose();
                    }
                    else
                    {
                        //将从网页JSON里提取出来的带转义的网址也能保证正确识别。
                        //http:\/\/x.x.com\/x\/x\/index.m3u8
                        url = url.Replace("\\", "");

                        Uri tmpUri;
                        if (Uri.TryCreate(url, UriKind.Absolute, out tmpUri))//判断是否为标准URL链接，标准时才下载
                        {
                            urls.Add(url);
                            //mUrlAndNameMap.Add(url, name);
                            mUrlAndDownloadDirMap.Add(url, dir);

                            WriteLog(TAG, string.Format("文件名={0}", name));
                            WriteLog(TAG, string.Format("下载网址={0}", url));
                            WriteLog(TAG, string.Format("下载目录={0}", dir.Replace(downloadDir, "~")));
                            mLastTaskDir = dir;
                        }
                        else
                        {
                            WriteLog(TAG, string.Format("跳过不正确的URL地址：{0}", url));
                        }
                    }
                }

                foreach (var url in urls)
                {
                    //mPidAndUrlMap.Add(mAria2c.AddUri(url, "", mUrlAndDownloadDirMap[url], mSelectedUserAgent), url);
                    string gid = mAria2c.AddUri(url, "", mUrlAndDownloadDirMap[url], mSelectedUserAgent);
                    mAria2c.ChangePosition(gid, 0, PosType.POS_SET); //将m3u8任务放到下载队列最前面，优先级最高，避免切片下完才下该m3u8
                }

                mAllDownLoadComplete = false;
                mAllPause = false;
                StartTimer();
                mAria2c.IsPauseAll = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);
            }
            finally
            {
                ((Button)sender).Enabled = true;
                Cursor.Current = Cursors.Default;
                txbUrls.Clear();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var TAG = "主程序";
            WriteLog(TAG, "启动中");

            //生成配置文件选择列表
            var aria2Path = Path.Combine(Environment.CurrentDirectory, "Aria2\\aria2c.exe");
            if (File.Exists(aria2Path))
            {
                var aria2Folder = Path.GetDirectoryName(aria2Path);
                foreach (var file in Directory.GetFiles(aria2Folder))
                {
                    if (Path.GetExtension(file).ToLower() == ".conf")
                    {
                        var configFileName = Path.GetFileName(file);
                        if (!cbbConfigFileName.Items.Contains(configFileName))
                        {
                            cbbConfigFileName.Items.Add(configFileName);
                        }
                    }
                }
            }

            #region 读取ini配置文件
            try
            {
                var iniPath = Path.Combine(Environment.CurrentDirectory, "settings.ini");
                if (File.Exists(iniPath))
                {
                    mCurrentDownLoadDir = INIHelper.Read("Aria2", "DownLoadDir", "", iniPath);
                    if (mCurrentDownLoadDir == "")
                        mCurrentDownLoadDir = Path.Combine(Environment.CurrentDirectory, "Download");

                    mLastTaskDir = INIHelper.Read("Aria2", "LastTaskDir", "", iniPath);
                    if (mLastTaskDir == "")
                        mLastTaskDir = mCurrentDownLoadDir;

                    string downloadSpeedLimit = INIHelper.Read("Aria2", "DownloadSpeedLimit", "", iniPath);
                    if (downloadSpeedLimit != "")
                        txbMaxDownloadSpeed.Text = downloadSpeedLimit;

                    txbMergeCMD.Tag = txbMergeCMD.Text;//将默认值保存到Tag
                    string configFileName = INIHelper.Read("Aria2", "ConfigFileName", "", iniPath);
                    if (configFileName != "")
                    {
                        if (cbbConfigFileName.Items.Contains(configFileName))
                        {
                            cbbConfigFileName.Text = configFileName;

                            //不再需要，自动触发cbbConfigFileName_SelectedIndexChanged
                            //if (File.Exists(Path.Combine(Environment.CurrentDirectory, "FFmpeg\\cmd.txt")))
                            //{
                            //    txbMergeCMD.Text = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "FFmpeg\\cmd.txt"), Encoding.Default);
                            //}
                        }
                    }


                    string selectedBandWidth = INIHelper.Read("Download", "SelectedBandWidth", "", iniPath);
                    if (selectedBandWidth != "")
                    {
                        if (selectedBandWidth == decimal.MaxValue.ToString())
                            rdbSelectBandWidthMax.Checked = true;
                        else if (selectedBandWidth == decimal.MinValue.ToString())
                            rdbSelectBandWidthMin.Checked = true;
                        else
                        {
                            rdbSelectBandWidthCustom.Checked = true;
                            txbCustomBandwidth.Text = selectedBandWidth;
                        }
                        decimal tmpdecimal;
                        decimal.TryParse(selectedBandWidth, out tmpdecimal);
                        mSelectedBandwidth = tmpdecimal;
                    }
                    //不启用，避免用户没注意到选中出现意外
                    //string afterDownloadCompleted = INIHelper.Read("Download", "AfterDownloadCompleted", "", iniPath);
                    //if (afterDownloadCompleted != "")
                    //{
                    //    if (cbbAction.Items.Contains(afterDownloadCompleted))
                    //    {
                    //        cbbAction.Text = afterDownloadCompleted;
                    //    }
                    //}
                    mSelectedUserAgent = INIHelper.Read("Download", "SelectedUserAgent", "", iniPath);
                    if (mSelectedUserAgent != "")
                    {
                        if (cbbUserAgent.Items.Contains(mSelectedUserAgent))
                        {
                            cbbUserAgent.Text = mSelectedUserAgent;
                        }
                    }
                    else
                        mSelectedUserAgent = cbbUserAgent.Text;

                    string mergeTasksToGroup = INIHelper.Read("Download", "MergeTasksToGroup", "", iniPath);
                    if (mergeTasksToGroup != "")
                    {
                        bool isMergeTasks;
                        if (bool.TryParse(mergeTasksToGroup, out isMergeTasks))
                            ckbMergeTasks.Checked = isMergeTasks;
                    }


                    string playCompletedSound = INIHelper.Read("General", "PlayCompletedSound", "", iniPath);
                    if (playCompletedSound != "")
                    {
                        bool isPlaySound;
                        if (bool.TryParse(playCompletedSound, out isPlaySound))
                            mPlayCompletedSound = isPlaySound;
                    }
                    else
                    {
                        mPlayCompletedSound = true;
                    }

                    string exitRequireConfirm = INIHelper.Read("General", "ExitRequireConfirm", "", iniPath);
                    if (exitRequireConfirm != "")
                    {
                        bool isNeedConfirm;
                        if (bool.TryParse(exitRequireConfirm, out isNeedConfirm))
                            mExitRequireConfirm = isNeedConfirm;
                    }
                    else
                    {
                        mExitRequireConfirm = true;
                    }
                }
                else
                {
                    mPlayCompletedSound = true;
                    mExitRequireConfirm = true;
                    bool flag = INIHelper.CreateIni(iniPath);
                    if (!flag)
                        WriteLog(TAG, "配置文件创建失败！");
                }
            }
            catch { WriteLog(TAG, "加载配置文件失败！"); }
            #endregion

            Bitmap img = global::HLS.Download.UI.Properties.Resources.Arrow as Bitmap;
            this.Cursor = GetCursor(img);
            img.Dispose();

            //txbMaxDownloadSpeed.Cursor = this.Cursor;
            //txbCustomBandwidth.Cursor = this.Cursor;
            //txbUrls.Cursor = this.Cursor;
            //txbMergeCMD.Cursor = this.Cursor;
            //txbLog.Cursor = this.Cursor;
            //txbAria2GlobalInfo.Cursor = this.Cursor;
            //txbTotalData.Cursor = this.Cursor;
            //txbTotalTime.Cursor = this.Cursor;
            {
                txbMaxDownloadSpeed.Cursor = txbCustomBandwidth.Cursor =
                    txbUrls.Cursor = txbMergeCMD.Cursor = txbLog.Cursor =
                    txbAria2GlobalInfo.Cursor = txbTotalData.Cursor =
                    txbTotalTime.Cursor = txbAria2Args.Cursor = this.Cursor;
            }

            txbAria2GlobalInfo.GotFocus += new EventHandler(txbReadOnly_GotFocus);
            txbTotalData.GotFocus += new EventHandler(txbReadOnly_GotFocus);
            txbTotalTime.GotFocus += new EventHandler(txbReadOnly_GotFocus);

            InitializeNotifyicon();//初始化托盘

            //不再在启动时检测状态，发现即使检测到已经启动，但是后续的别的模块还是依赖于点击界面的启动按钮才有效。
            //因此没有任何检测的意义了。因为都必须要点击至少一次“启动”按钮才能继续使用别的功能。
            //NO:btnStartAria2.Enabled = !Aria2cRuntime.IsLoaded;
            //NO:WriteLog(TAG, "检测到 Aria2 状态为" + (!btnStartAria2.Enabled ? "【已启动】" : "[未启动]"));            

            WriteLog(TAG, "启动完毕。");

            //启用传入的参数（调用非手动开启）
            EnabledParameters();
        }

        /// <summary> 自定义光标获取</summary>
        /// <param name="img"> 光标图片 </param>
        /// <param name="HotSpotX"> 光标作用点X </param>
        /// <param name="HotSpotY"> 光标作用点Y </param>
        public Cursor GetCursor(Bitmap img, int HotSpotX = 0, int HotSpotY = 0)
        {
            Bitmap curImg = new Bitmap(img.Width * 2, img.Height * 2);
            Graphics g = Graphics.FromImage(curImg);

            g.Clear(Color.FromArgb(0, 0, 0, 0));
            g.DrawImage(img, img.Width - HotSpotX, img.Width - HotSpotY, img.Width, img.Height);
            Cursor cur = new Cursor(curImg.GetHicon());

            g.Dispose();
            curImg.Dispose();

            return cur;
        }

        /// <summary>
        /// 初始化托盘
        /// </summary>
        private void InitializeNotifyicon()
        {
            //定义一个MenuItem数组 
            MenuItem[] itms = new MenuItem[5];
            itms[0] = new MenuItem();
            itms[0].Text = "隐藏窗体";
            itms[0].Click += new EventHandler(ToggleWindowState);
            itms[0].DefaultItem = true;

            itms[1] = new MenuItem("全部开始下载");
            itms[1].Click += new EventHandler(btnUnPauseAll_Click);
            itms[2] = new MenuItem("全部暂停下载");
            itms[2].Click += new EventHandler(btnPauseAll_Click);
            itms[3] = new MenuItem("自定义限速");
            itms[3].Click += new EventHandler(ToggleMaxDownloadSpeed);

            itms[4] = new MenuItem();
            itms[4].Text = "退出程序";
            itms[4].Click += new EventHandler(Exit);
            //把此MenuItem数组赋值给ContextMenu对象
            //ContextMenu notifyiconMenu = new ContextMenu(itms);
            //为托盘程序加入设定好的ContextMenu对象
            notifyIconTray.ContextMenu = new ContextMenu(itms);

            //追加双击显示窗体事件
            notifyIconTray.DoubleClick += new EventHandler(notifyIconTray_DoubleClick);
        }

        /// <summary>
        /// 启动或禁用自定义限速
        /// </summary>
        private void ToggleMaxDownloadSpeed(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (item.Text.IndexOf('√') == -1)//未启用
            {
                btnApplyMaxSpeed_Click(sender, e);

                //重设托盘菜单文本
                item.Text = "√自定义限速";
            }
            else
            {
                int iDownloadSpeedLimit;
                if (!int.TryParse(txbMaxDownloadSpeed.Text.Trim(), out iDownloadSpeedLimit))
                {
                    iDownloadSpeedLimit = 0;
                }

                txbMaxDownloadSpeed.Text = "0";//设成0解除速度限制
                btnApplyMaxSpeed_Click(sender, e);
                txbMaxDownloadSpeed.Text = iDownloadSpeedLimit.ToString();//还原为用户输入的值

                //重设托盘菜单文本
                item.Text = "自定义限速";
            }
        }

        /// <summary>
        /// 双击托盘图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIconTray_DoubleClick(object sender, EventArgs e)
        {
            ToggleWindowState(sender, e);
        }

        /// <summary>
        /// 设定当前窗口状态
        /// </summary>
        private void ToggleWindowState(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => ToggleWindowState(sender, e)));
            }
            else
            {
                if (!this.Visible)
                {
                    //显示窗体
                    this.Show();
                    //this.Visible = true;
                    //重设托盘菜单
                    this.notifyIconTray.ContextMenu.MenuItems[0].Text = "隐藏窗体";
                    //激活窗体
                    this.Activate();
                }
                else
                {
                    //隐藏窗体
                    this.Hide();
                    //this.Visible = false;
                    //重设托盘菜单
                    this.notifyIconTray.ContextMenu.MenuItems[0].Text = "显示窗体";
                }
            }
        }

        private async void btnStartAria2_Click(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            var TAG = "启动Aria2";
            WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行中");
            try
            {
                int iCustomPort = 6800;
                string strRpcSecret = string.Empty;
                string strCustomPath = string.Empty;
                //进程遍历所有相同名字进程，假如存在则不再启动。
                var plist = Process.GetProcessesByName("aria2c");
                if (plist.Length == 0)
                {
                    string strConfigFilePath = Path.Combine(Environment.CurrentDirectory, "Aria2\\" + cbbConfigFileName.Text);
                    if (File.Exists(strConfigFilePath))
                    {
                        string strConfigFileText = File.ReadAllText(strConfigFilePath, Encoding.UTF8);
                        strRpcSecret = GetValueFromConfig(strConfigFileText, 0, "rpc-secret");//获取密码令牌

                        string strCustomPort = GetValueFromConfig(strConfigFileText, 0, "rpc-listen-port");//获取RPC监听端口
                        if (strCustomPort != string.Empty)
                            int.TryParse(strCustomPort, out iCustomPort);

                        strCustomPath = GetValueFromConfig(strConfigFileText, 0, "dir");//获取配置文件中下载目录
                        if (strCustomPath != string.Empty)
                        {
                            if (strCustomPath.IndexOf(':') == 1 /*&& Path.IsPathRooted(strCustomPath)*/)//绝对路径，包含盘符，不作处理
                            {
                            }
                            else//相对路径，拼接
                            {
                                try
                                {
                                    var aria2Path = Path.Combine(Environment.CurrentDirectory, "Aria2");
                                    var downloadPath = Path.GetFullPath(Path.Combine(aria2Path, strCustomPath));
                                    //MessageBox.Show(downloadPath);
                                    strCustomPath = downloadPath;
                                }
                                catch { }
                            }
                        }
                    }

                    var settings = new Aria2cSettings();
                    settings.Aria2Path = Path.Combine(Environment.CurrentDirectory, "Aria2\\aria2c.exe");
                    settings.Aria2Host = "localhost";
                    settings.Aria2Port = iCustomPort;
                    settings.Aria2Args = " --conf-path=" + cbbConfigFileName.Text;//设置配置文件名称，例如" --conf-path=aria2c.conf"   
                    if (txbAria2Args.Text.Trim() != "")
                        settings.Aria2Args += " " + txbAria2Args.Text.Trim();
                    settings.Aria2RpcSecret = strRpcSecret;
                    Aria2cRuntime.Settings = settings;

                    Aria2cRuntime.Start();
                }

                WriteLog(TAG, "执行完毕。");
                WriteLog(TAG, "执行结果：检测中");

                //Thread.Sleep(1500);
                int iTmpCount = 0;
                while (Process.GetProcessesByName("aria2c").Length == 0)//等待aria2c进程开启
                {
                    //Thread.Sleep(1000);//延时，等待Aria2cRuntime.Start生效
                    await Task.Delay(1000);
                    iTmpCount++;
                    if (iTmpCount >= 7)//最多等待7秒
                        break;
                }
                iTmpCount = 0;
                while (!Aria2cRuntime.IsLoaded)//Aria2c RPC是不可用的状态
                {
                    //Thread.Sleep(1000);//延时，等待Aria2c RPC真正可用（进程启动，服务不可用的短暂情况）
                    await Task.Delay(1000);
                    iTmpCount++;
                    if (iTmpCount >= 3)//最多等待3秒
                        break;
                }

                btnStartAria2.Enabled = !Aria2cRuntime.IsLoaded;
                WriteLog(TAG, "执行结果：" + (!btnStartAria2.Enabled ? "【成功】" : "[失败]"));

                if (Aria2cRuntime.IsLoaded)
                {
                    WriteLog(TAG, string.Format("当前配置文件：{0}", cbbConfigFileName.Text));
                    WriteLog(TAG, string.Format("Aria2c启动参数：{0}", Aria2cRuntime.Settings.Aria2Args.Trim()));
                    WriteLog(TAG, string.Format("Aria2c RPC端口：{0}", Aria2cRuntime.Aria2cPort));
                    WriteLog(TAG, string.Format("Aria2c密码令牌：{0}", strRpcSecret == string.Empty ? "无令牌" : strRpcSecret));

                    txbAria2GlobalInfo.Text = "";
                    RefreshDownloadProgress(0);
                    RefreshNotifyIconTrayText("无下载");
                    LastTotalDoneTasks = 0;
                    mDownDirAndTaskCountMap.Clear();
                    DownloadCompletedData = 0;
                    txbTotalData.Text = "";
                    DownloadTotalSeconds = 0;
                    txbTotalTime.Text = "";

                    mAria2c = new Aria2c();
                    mAria2c.OnGlobalStatusChanged += delegate (object obj, Aria2cGlobalStatEvent gevent)
                    {
                        if (txbAria2GlobalInfo.InvokeRequired)
                        {
                            while (!txbAria2GlobalInfo.IsHandleCreated)
                            {
                                //解决窗体关闭时出现“访问已释放句柄“的异常
                                if (txbAria2GlobalInfo.Disposing || txbAria2GlobalInfo.IsDisposed)
                                    return;
                            }
                            txbAria2GlobalInfo.Invoke(new MethodInvoker(
                                /*async*/ () =>
                                {
                                    var strDownloadSpeed = string.Format("{0}/S", HumanReadableSize(gevent.Stat.DownloadSpeed));
                                    //txbAria2GlobalInfo.Text = string.Format("DownloadSpeed={0:F2}KB/S NumActive={1} NumWaiting={2} NumStoppedTotal={3}", gevent.Stat.DownloadSpeed / 1024d, gevent.Stat.NumActive, gevent.Stat.NumWaiting, gevent.Stat.NumStoppedTotal);
                                    txbAria2GlobalInfo.Text = string.Format("DownloadSpeed={0} NumActive={1} NumWaiting={2} NumStoppedTotal={3}"
                                        , strDownloadSpeed
                                        , gevent.Stat.NumActive
                                        , gevent.Stat.NumWaiting
                                        , gevent.Stat.NumStoppedTotal
                                    );
                                    RefreshDownloadData();//刷新流量统计
                                    if (gevent.Stat.NumActive != 0 && !mAllDownLoadComplete)
                                    {
                                        long lDownloadedTasks = gevent.Stat.NumStoppedTotal - LastTotalDoneTasks;
                                        long lRestTasks = gevent.Stat.NumActive + gevent.Stat.NumWaiting;
                                        int iProgress = (int)Math.Round(lDownloadedTasks * 100d / (lDownloadedTasks + lRestTasks), 0);
                                        RefreshDownloadProgress(iProgress);//刷新下载进度

                                        RefreshNotifyIconTrayText(string.Format("正在下载任务数：{0}\n下载速度：{1}", lRestTasks, strDownloadSpeed));//刷新托盘提示
                                    }

                                    //await Task.Delay(500);
                                })
                            );
                            return;
                        }
                    };
                    mAria2c.OnError += delegate (object obj, Aria2cTaskEvent taskEvent)
                    {
                        OnDownloadError(taskEvent, obj);
                    };
                    mAria2c.OnFinish += delegate (object obj, Aria2cTaskEvent taskEvent)
                    {
                        OnDownloadCompleted(taskEvent);
                    };
                    mAria2c.OnAllFinish += delegate (object obj, Aria2cGlobalStatEvent gevent)
                    {
                        OnAllDownloadCompleted(gevent);
                    };
                    GC.KeepAlive(mAria2c);//避免被回收
                    WriteLog(TAG, string.Format("Aria2c版本号：{0}", mAria2c.GetVersion()));

                    //加载user-agent、rpc-secret等设置项
                    Aria2cOption globalOption = mAria2c.GetGlobalOption();
                    if (globalOption != null)
                    {
                        string user_agent = globalOption.Option["user-agent"];
                        if (!string.IsNullOrEmpty(user_agent) && !user_agent.Contains("aria2"))
                        {
                            if (!cbbUserAgent.Items.Contains(user_agent))
                            {
                                cbbUserAgent.Items.Add(user_agent);
                                cbbUserAgent.SelectedIndex = cbbUserAgent.Items.Count - 1;
                                //MessageBox.Show(cbbUserAgent.Text);
                                //MessageBox.Show(mSelectedUserAgent);
                            }
                        }
                        /* //无效，可能是安全性不允许获取密码
                        if (globalOption.Option.ContainsKey("rpc-secret"))
                        {
                            WriteLog(TAG, string.Format("Aria2c密码令牌：{0}", globalOption.Option["rpc-secret"]));
                        }*/
                    }

                    //var downloadPath = Path.Combine(Environment.CurrentDirectory, "Download");
                    var downloadPath = string.IsNullOrWhiteSpace(mCurrentDownLoadDir) ? Path.Combine(Environment.CurrentDirectory, "Download") : mCurrentDownLoadDir;
                    //if (strCustomPath != string.Empty)
                    //    downloadPath = strCustomPath;
                    if (!String.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
                        downloadPath = folderBrowserDialog1.SelectedPath;
                    if (!Directory.Exists(downloadPath))
                        Directory.CreateDirectory(downloadPath);
                    if (strCustomPath != string.Empty && /*downloadPath != strCustomPath*/ !ComparePathEqual(downloadPath, strCustomPath))
                    {
                        Aria2cRuntime.DownLoadDirectory = downloadPath;//如果downloadPath与strCustomPath相等，则不需要修改下载目录
                    }
                    mCurrentDownLoadDir = downloadPath;
                    WriteLog(TAG, "设置全局下载目录=" + downloadPath);

                    //await Task.Delay(3000);//等待3秒后，开始恢复会话任务  修改：取消等待
                    int iRestoreTasksCount = mAria2c.RestoreSession();
                    if (iRestoreTasksCount != 0)
                    {
                        WriteLog(TAG, string.Format("恢复会话任务完毕：共恢复{0}个下载任务。点击“全部开始下载”继续之前的下载", iRestoreTasksCount));
                        InitTimer();
                        mAllDownLoadComplete = false;
                        mAllPause = true;
                    }
                }
                else//如果启动失败，mAria2c没有实例化，为避免后续出错，杀掉Aria2c进程
                {
                    btnKillAllAria2_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);

                //异常情况下，还是将按钮可用化，方便下次执行。
                ((Button)sender).Enabled = true;
            }
            finally
            {
                //由执行结果决定最终是否可用。
                //((Button)sender).Enabled = true;

                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// 比较两个路径是否指向同一地址
        /// </summary>
        /// <param name="strPath1"></param>
        /// <param name="strPath2"></param>
        /// <returns></returns>
        private bool ComparePathEqual(string strPath1, string strPath2)
        {
            return 0 == String.Compare(
            Path.GetFullPath(strPath1).TrimEnd(Path.DirectorySeparatorChar, Path.PathSeparator),
            Path.GetFullPath(strPath2).TrimEnd(Path.DirectorySeparatorChar, Path.PathSeparator),
            true);
        }

        /// <summary>
        /// 根据配置文件获取配置值
        /// </summary>
        /// <param name="strConfigFileText"></param>
        /// <param name="strConfigKey"></param>
        /// <returns></returns>
        private string GetValueFromConfig(string strConfigFileText, int startIndex, string strConfigKey)
        {
            string strConfigValue = string.Empty;

            int idx = strConfigFileText.IndexOf(strConfigKey + "=", startIndex, StringComparison.OrdinalIgnoreCase);
            if (idx != -1)//找到继续，没找到直接返回
            {
                int idx2 = strConfigFileText.IndexOf("\n", idx, StringComparison.OrdinalIgnoreCase);
                string strLineString = string.Empty;
                if (idx2 != -1)//截取配置项整行文本
                {
                    strLineString = strConfigFileText.Substring(idx, idx2 - idx);
                }
                else//配置项是末行，截取配置项后面的所有文本
                {
                    strLineString = strConfigFileText.Substring(idx);
                }

                string strPrveTextOfLineString = string.Empty;
                int idx3 = strConfigFileText.LastIndexOf("\n", idx, StringComparison.OrdinalIgnoreCase);
                if (idx3 != -1)//截取换行符到配置项前面的所有文本
                {
                    strPrveTextOfLineString = strConfigFileText.Substring(idx3, idx - idx3);
                }
                else//配置项是首行，截取配置项前面的文本
                {
                    strPrveTextOfLineString = strConfigFileText.Substring(0, idx);
                }

                if (!strPrveTextOfLineString.Contains("#"))//不能是注释掉的配置项
                {
                    //strConfigValue = strLineString.Split('=')[1];//不准确，等号后可能是带参数网址或Headers
                    strConfigValue = strLineString.Substring(strLineString.IndexOf('=') + 1).Replace("\r", "");//移除回车符，避免出错
                }
                else
                {
                    startIndex = idx + (strConfigKey + "=").Length;
                    strConfigValue = GetValueFromConfig(strConfigFileText, startIndex, strConfigKey);
                }
            }

            return strConfigValue;
        }

        private async void OnAllDownloadCompleted(Aria2cGlobalStatEvent gevent)
        {
            try
            {
                if (!mAllDownLoadComplete && !mAllPause)
                {
                    mAllDownLoadComplete = true;
                    StopTimer();

                    var TAG = "下载结果";
                    WriteLog(TAG, string.Format("OnAllFinish={0}; 全部下载完毕！", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    WriteLog(TAG, string.Format("下载结束，本次任务Aria2共下载了{0}个文件。", gevent.Stat.NumStoppedTotal - LastTotalDoneTasks));
                    if (mAria2c.CountDownTask() != 0)
                    {
                        WriteLog(TAG, string.Format("【注意】可能已出现错误，下载列表遗留{0}个任务。请打开WebUI对出错任务进行重试！", mAria2c.CountDownTask()));
                    }
                    if (this.WindowState == FormWindowState.Minimized || !this.Visible)
                        notifyIconTray.ShowBalloonTip(3000, "下载结束", string.Format("本次任务Aria2共下载了{0}个文件。", gevent.Stat.NumStoppedTotal - LastTotalDoneTasks), ToolTipIcon.Info);
                    RefreshDownloadProgress(100);
                    RefreshNotifyIconTrayText("下载完成");
                    LastTotalDoneTasks = gevent.Stat.NumStoppedTotal;
                    mDownDirAndTaskCountMap.Clear();

                    #region 播放提示音3遍
                    if (mPlayCompletedSound)
                    {
                        //System.Media.SystemSounds.Asterisk.Play();
                        //System.Media.SystemSounds.Beep.Play();
                        //System.Media.SystemSounds.Exclamation.Play();
                        //System.Media.SystemSounds.Hand.Play();
                        //System.Media.SystemSounds.Question.Play();
                        using (SoundPlayer player = new SoundPlayer())
                        {
                            //using (Stream stream_download_complete = global::HLS.Download.UI.Properties.Resources.download_complete)
                            using (Stream stream_download_complete = Properties.Resources.ResourceManager.GetStream("download_complete"))
                            {
                                player.Stream = stream_download_complete;
                                player.LoadAsync();//异步加载
                                for (int i = 0; i < 3; i++)
                                {
                                    player.Play();//声音叠成一次
                                                  //player.PlaySync();//同步，本线程
                                                  //player.PlayLooping();//持续播放

                                    await Task.Delay(2000);
                                }
                            }
                        }
                    }
                    #endregion

                    #region 为避免出现异常，这里不清除队列及内存(已知：影响OnFinish事件)
                    /*await Task.Delay(10000);//等待其他事件先触发，避免PurgeDownloadResult阻止其他（无效）
                    var result = mAria2c?.PurgeDownloadResult();//清空下载结果，释放内存
                    TAG = "释放内存";
                    WriteLog(TAG, "执行完毕");
                    WriteLog(TAG, "执行结果=" + result);*/
                    #endregion

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    DealAction(gevent);//处理下载完成相关操作
                }
            }
            catch (Exception ex)
            {
                WriteLog("下载结果", "出现异常");
                WriteLog("下载结果", ex.Message);
            }
        }

        private async void DealAction(EventArgs e)
        {
            var TAG = "下载完成操作";
            try
            {
                if (cbbAction.Text == "无操作" || cbbAction.Text.Trim() == "")
                    return;
                //注释掉，依不同情况决定是否Kill Aria2
                //if (mAria2c != null)
                //    btnKillAllAria2_Click(this, e);

                WriteLog(null, Environment.NewLine);
                WriteLog(TAG, string.Format("10秒后执行：【{0}】", cbbAction.Text));
                WriteLog(TAG, "若要阻止该命令执行，请在10秒内关闭本程序！");
                await Task.Delay(10000);
                WriteLog(TAG, "命令执行中...");
                switch (cbbAction.Text)
                {
                    case "注销":
                        CloseAria2ByNonUiThread(e);
                        ForSystem.Logoff();
                        break;
                    case "锁定":
                        ForSystem.Lock();
                        break;
                    case "关机":
                        CloseAria2ByNonUiThread(e);
                        ForSystem.Shutdown();
                        break;
                    case "重新启动":
                        CloseAria2ByNonUiThread(e);
                        ForSystem.Reboot();
                        break;
                    case "睡眠":
                        ForSystem.Sleep();
                        break;
                    case "休眠":
                        ForSystem.Hibernate();
                        break;
                    case "关闭显示器":
                        ForSystem.TurnOffMonitor(this.Handle);
                        break;
                    case "关闭Aria2":
                        CloseAria2ByNonUiThread(e);
                        break;
                    case "退出程序":
                        CloseAria2ByNonUiThread(e);
                        //直接调用Exit方法会在退出时崩溃。这里设定FormClosingEventArgs避免弹出提示框（非UserClosing即可）
                        //Exit(this, new FormClosingEventArgs(CloseReason.None,false));
                        //mAria2c的事件响应，非UI主线程，跨线程（ObjectDisposedException）,调用Invoke方法解决
                        if (this.InvokeRequired)
                        {
                            if (!this.IsHandleCreated && (this.Disposing || this.IsDisposed))//窗体已经在关闭时直接返回
                                return;
                            this.Invoke(new MethodInvoker(() => {
                                this.MainForm_FormClosing(this, new FormClosingEventArgs(CloseReason.ApplicationExitCall, false));
                            }));
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLog(TAG, string.Format("执行失败：【{0}】。请确认", cbbAction.Text));
                WriteLog(TAG, "错误信息：" + ex.Message);
            }
        }

        private void CloseAria2ByNonUiThread(EventArgs e)
        {
            if (mAria2c == null) //mAria2c为null不需要kill
                return;
            if (btnKillAllAria2.InvokeRequired)
            {
                //解决窗体关闭时出现“访问已释放句柄“的异常 
                if (!btnKillAllAria2.IsHandleCreated && (btnKillAllAria2.Disposing || btnKillAllAria2.IsDisposed))
                    return;

                btnKillAllAria2.Invoke(new MethodInvoker(() => btnKillAllAria2_Click(this, e)));
                return;//返回是为了不调用下面的非Invoke方法
            }

            //主线程调用的话不要Invoke
            btnKillAllAria2_Click(this, e);
        }

        private void OnDownloadError(Aria2cTaskEvent taskEvent, object obj)
        {
            var TAG2 = "下载状态更新";
            var uri = taskEvent.Task.Files[0].Uris[0].Uri.ToString();
            if (taskEvent.Task.ErrorCode != 0)
            {
                if (mAllPause && !mAllDownLoadComplete && txbLog.Text.EndsWith("【建议取消剩余下载任务！】" + Environment.NewLine))
                    return;
                WriteLog(TAG2, uri);
                WriteLog(TAG2, string.Format("OnFinish={0}; ErrorCode={1}; ErrorMessage={2}; "
                    , taskEvent.Gid, taskEvent.Task.ErrorCode, taskEvent.Task.ErrorMessage));
                if ((taskEvent.Task.ErrorMessage.Contains("status=403") || taskEvent.Task.ErrorMessage.Contains("status=503") || taskEvent.Task.ErrorMessage.Contains("Download aborted"))
                || taskEvent.Task.ErrorCode == 1/*unknown error*/|| taskEvent.Task.ErrorCode == 2/*timeout*/
                || taskEvent.Task.ErrorCode == 3/*resource was not found*/|| taskEvent.Task.ErrorCode == 4/*max resource not found*/
                || taskEvent.Task.ErrorCode == 6/*network problem occurred*/|| taskEvent.Task.ErrorCode == 9/*not enough disk space*/
                || taskEvent.Task.ErrorCode == 17/*file I/O error occurred*/|| taskEvent.Task.ErrorCode == 22/*HTTP response header was bad or unexpected*/
                || taskEvent.Task.ErrorCode == 23/*too many redirects occurred*/|| taskEvent.Task.ErrorCode == 24/*HTTP authorization failed*/)
                {
                    btnPauseAll_Click(obj, taskEvent);
                    WriteLog("下载终止", "出现了无法继续下载的错误，所有下载已暂停！请确认。" + Environment.NewLine + "【建议取消剩余下载任务！】");
                    if (this.WindowState == FormWindowState.Minimized || !this.Visible)
                        notifyIconTray.ShowBalloonTip(3000, "下载终止", "出现了无法继续的错误", ToolTipIcon.Error);
                    //mAllDownLoadComplete = true;//这里注释掉，不然在手动恢复后任务成功没有完成提示
                    RefreshNotifyIconTrayText("下载终止");
                    //StopTimer();//计时器不释放，暂停已禁用，便于恢复下载后继续
                }
                return;
            }
        }

        private void OnDownloadCompleted(Aria2cTaskEvent taskEvent)
        {
            //var TAG2 = "下载状态更新";
            var file_path = taskEvent.Task.Files[0].Path;
            switch (Path.GetExtension(file_path).ToLower())
            {
                case ".m3u8":
                    {
                        WriteLog("下载状态更新", taskEvent.Task.Files[0].Uris[0].Uri);
                        WriteLog("下载状态更新", string.Format("OnFinish={0}; m3u8下载完毕！", taskEvent.Gid));

                        //当播放序列文件下载完成时进行解析
                        OnDownloadCompletedM3U8(taskEvent);
                    }
                    break;
                case ".key":
                    {
                        WriteLog("下载状态更新", taskEvent.Task.Files[0].Uris[0].Uri);
                        WriteLog("下载状态更新", string.Format("OnFinish={0}; key下载完毕！", taskEvent.Gid));
                        OnDownloadCompletedTS(/*taskEvent*/file_path);
                    }
                    break;
                case ".ts":
                    {
                        //当视频流切片下载完成时。
                        OnDownloadCompletedTS(/*taskEvent*/file_path);
                    }
                    break;
                default:
                    {
                        var extension = Path.GetExtension(file_path).ToLower();
                        var foldName = Path.GetFileName(Path.GetDirectoryName(file_path));
                        WriteLog("下载状态更新", string.Format("{0}/{1}：{2}文件", foldName, Path.GetFileName(file_path), extension));
                        //WriteLog(TAG2, uri);
                        WriteLog("下载状态更新", string.Format("OnFinish={0}; 文件下载完毕！", taskEvent.Gid));
                    }
                    break;
            }
            //RefreshDownloadData(taskEvent.Task.CompletedLength);//刷新流量统计
            DownloadCompletedData += taskEvent.Task.CompletedLength;
        }

        private void OnDownloadCompletedTS(/*Aria2cTaskEvent taskEvent*/string filePath)
        {
            //var TAG = "TS分片下载";
            try
            {
                lock (SequenceLock)
                {
                    //var file = taskEvent.Task.Files[0];
                    var dir = Path.GetDirectoryName(filePath);
                    if (mDownDirAndTaskCountMap.ContainsKey(dir))
                    {
                        //int iTmpCount = mDownDirAndTaskCountMap[dir];
                        mDownDirAndTaskCountMap[dir] -= 1;

                        if (mDownDirAndTaskCountMap[dir] == 0)
                        {
                            Thread autoMergeThread = new Thread(new ParameterizedThreadStart(AutoMergeAfterDownOver));
                            autoMergeThread.IsBackground = true;
                            autoMergeThread.Priority = ThreadPriority.Normal;
                            autoMergeThread.Start(dir);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("TS分片下载", "出现异常");
                WriteLog("TS分片下载", ex.Message);
            }
            finally
            {
                //TAG = null;
            }
        }

        private void OnDownloadCompletedM3U8(Aria2cTaskEvent taskEvent)
        {
            var TAG = "解析下载完成的M3U8";
            WriteLog(TAG, "执行中");
            try
            {
                var file = taskEvent.Task.Files[0];
                var dir = Path.GetDirectoryName(file.Path);
                WriteLog(TAG, String.Format("对应的文件名={0}", Path.GetFileName(dir)));
                Uri baseUri;
                Uri.TryCreate(RemoveUriParas(file.Uris[0].Uri), UriKind.Absolute, out baseUri);

                var t = HLS.Download.Models.HLSStream.Open(file.Path);
                var r = t.Result;

                //一个M3u8里可能定义了不同码率的新m3u8文件。
                if (r.Playlist.Length > 0)
                {
                    HLSPlaylist nextPlaylist = null;

                    #region 多码率选择策略
                    if (r.Playlist.Length == 1)
                        nextPlaylist = r.Playlist[0];
                    else
                    {
                        if (mSelectedBandwidth == null)
                        {
                            WriteLog(TAG, string.Format("停止下载视频流切片；因为存在多码率，且没有选择多码率下载策略或自定义码率无效：{0}", mSelectedBandwidth));

                            WriteLog(TAG, "支持的码率有：");
                            for (var pi = 0; pi < r.Playlist.Length; pi++)
                            {
                                var p = r.Playlist[pi];
                                WriteLog(TAG, String.Format("{0}：码率={1}，分辨率={2}", pi + 1, p.BANDWIDTH, p.RESOLUTION));
                            }
                            return;
                        }
                        //根据多码率下载策略取出需要的码率下载.
                        decimal? lastBandwidth = null;
                        foreach (var p in r.Playlist)
                        {
                            var tmpBand = decimal.Parse(p.BANDWIDTH);
                            if (mSelectedBandwidth == decimal.MaxValue)
                            {
                                if (lastBandwidth == null || tmpBand > lastBandwidth)
                                    nextPlaylist = p;
                                continue;
                            }
                            if (mSelectedBandwidth == decimal.MinValue)
                            {
                                if (lastBandwidth == null || tmpBand < lastBandwidth)
                                    nextPlaylist = p;
                                continue;
                            }
                            if (mSelectedBandwidth == tmpBand)
                            {
                                nextPlaylist = p;
                                //自定义模式，匹配成功直接退出循环。
                                break;
                            }
                        }

                        //自定义模式，匹配失败时，将所有码率输出来，方便定位。
                        if (nextPlaylist == null)
                        {
                            WriteLog(TAG, string.Format("自定义码率匹配失败；自定义码率设置无效：{0}", mSelectedBandwidth));

                            WriteLog(TAG, "支持的码率有：");
                            for (var pi = 0; pi < r.Playlist.Length; pi++)
                            {
                                var p = r.Playlist[pi];
                                WriteLog(TAG, String.Format("{0}：码率={1}，分辨率={2}", pi + 1, p.BANDWIDTH, p.RESOLUTION));
                            }
                            return;
                        }
                    }
                    #endregion

                    WriteLog(TAG, String.Format("下载指定码率={0}，分辨率={1}", nextPlaylist.BANDWIDTH, nextPlaylist.RESOLUTION));
                    WriteLog(TAG, "下载指定码率：路径=" + nextPlaylist.URI);

                    //当下载了 index.m3u8 后指向了具体码率的 4405kb/hls/index.m3u8 此时因为文件名都是一样的，导致有BUG，会忽略下载。
                    if (Path.GetFileName(baseUri.AbsoluteUri) == Path.GetFileName(RemoveUriParas(nextPlaylist.URI)))
                        //于是将老的文件名重命名即可。
                        File.Move(file.Path, file.Path + ".old.m3u8");

                    var url = new Uri(baseUri, nextPlaylist.URI).AbsoluteUri;
                    var _name = "";
                    if (Path.GetExtension(RemoveUriParas(nextPlaylist.URI)).ToLower() != ".m3u8")
                        _name = Path.GetFileName(RemoveUriParas(nextPlaylist.URI)) + ".m3u8";
                    var gid = mAria2c.AddUri(url, _name, dir, mSelectedUserAgent);
                    mAria2c.ChangePosition(gid, 0, PosType.POS_SET); //将m3u8任务放到下载队列最前面，优先级最高，避免切片下完才下该m3u8
                    WriteLog(TAG, string.Format("下载指定码率：任务ID={0}", gid));
                }
                else
                {
                    if (r.Key != null && r.Key.Path.Length > 0)
                    {
                        WriteLog(TAG, String.Format("需下载的视频流解密KEY={0}", r.Key.Path));
                        WriteLog(TAG, String.Format("需下载的视频流加密方式={0}", r.Key.Method));
                        var url = r.Key.Path.Replace("\"", "");
                        if (!url.ToLower().Contains("http") && !url.ToLower().Contains("https") && !url.ToLower().Contains("ftp") && !url.ToLower().Contains("sftp"))
                        {
                            url = new Uri(baseUri, url).AbsoluteUri;
                        }
                        lock (SequenceLock)
                            mDownDirAndTaskCountMap.Add(dir, r.Parts.Length + 1);
                        mAria2c.AddUri(url, Path.GetFileNameWithoutExtension(RemoveUriParas(url)) + ".key", dir, mSelectedUserAgent);
                    }
                    else
                    {
                        lock (SequenceLock)
                            mDownDirAndTaskCountMap.Add(dir, r.Parts.Length);
                    }

                    WriteLog(TAG, String.Format("需下载的视频流切片块数量={0}", r.Parts.Length));
                    float fDuration = 0.0f;
                    foreach (var p in r.Parts)
                    {
                        var url = "";
                        if (p.Path.IndexOf("://") > -1)
                        { //p.Path本身就是完整地址，不需要基础Url
                            url = new Uri(p.Path).AbsoluteUri;
                        }
                        else
                        {
                            url = new Uri(baseUri, p.Path).AbsoluteUri;
                        }
                        var saveFileName = "";
                        var extension = Path.GetExtension(RemoveUriParas(url));
                        if (extension.ToLower() != ".ts")
                        {
                            saveFileName = Path.GetFileName(RemoveUriParas(url).Replace(extension, ".ts"));

                            //创建标记目录，用于合并时替换切片后缀
                            var flagPath = Path.Combine(dir, "!ExtReplace" + extension);
                            if (!Directory.Exists(flagPath))
                            {
                                Directory.CreateDirectory(flagPath);
                            }
                        }
                        mAria2c.AddUri(url, saveFileName, dir, mSelectedUserAgent);

                        float duration;
                        if (float.TryParse(p.Length.Replace(",", ""), out duration))
                            fDuration += duration;
                    }
                    int iDuration = Convert.ToInt32(Math.Floor(fDuration));
                    WriteLog(TAG, String.Format("需下载的视频流时长={0}", Seconds_To_HMS(iDuration)));
                    //判断切片是否存在同名情况，提前创建标记目录
                    if (Path.GetFileName(RemoveUriParas(r.Parts[0].Path)) == Path.GetFileName(RemoveUriParas(r.Parts[1].Path))
                        && Path.GetFileName(RemoveUriParas(r.Parts[1].Path)) == Path.GetFileName(RemoveUriParas(r.Parts[2].Path)))
                    {
                        //创建标记目录，用于合并时替换m3u8内容
                        var flagPath = Path.Combine(dir, "!DuplicateName" + Path.GetFileName(RemoveUriParas(r.Parts[0].Path)));
                        if (!Directory.Exists(flagPath))
                        {
                            Directory.CreateDirectory(flagPath);
                        }
                    }

                    WriteLog(TAG, String.Format("Aria2正在下载中..."));
                }
            }
            catch (Exception ex)
            {
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);
            }
            finally
            {
                WriteLog(TAG, "执行完毕");
            }
        }

        private async void ReadLocalM3U8(object objParas)
        {
            List<string> paras = objParas as List<string>;
            string filePath = paras[1];
            var TAG = "解析本地M3U8";
            WriteLog(TAG, "执行中");
            WriteLog(TAG, String.Format("对应的文件名={0}", paras[0]));
            try
            {
                var dir = Path.GetDirectoryName(filePath);

                //var t = HLS.Download.Models.HLSStream.Open(filePath);
                //var r = t.Result;
                var r = await HLS.Download.Models.HLSStream.Open(filePath);

                string tmpUri = "";

                var txt = File.ReadAllText(filePath);
                var sIndex0 = txt.IndexOf("#URI:", 0);
                if (sIndex0 != -1)//从自定义URI中读取基础Url
                {
                    var sIndex0_2 = txt.IndexOf("\n", sIndex0);
                    if (sIndex0_2 == -1)
                    {
                        sIndex0_2 = txt.IndexOf("#", sIndex0);
                    }
                    string strURI = txt.Substring(sIndex0, sIndex0_2 - sIndex0);
                    strURI = strURI.Substring(strURI.IndexOf(':') + 1).Replace("\"", "");
                    if (strURI.EndsWith("/"))
                        tmpUri = strURI.Substring(0, strURI.Length - 1);
                    else
                    {
                        tmpUri = RemoveUriParas(strURI);
                        //var sIndex = tmpUri.LastIndexOf("/");
                        //tmpUri = tmpUri.Substring(0, sIndex);
                    }
                }
                txt = null;

                if (tmpUri == "")
                {
                    if (r.Key != null && r.Key.Path.Length > 0)//从Key中读取基础Url
                    {
                        var tmpPath = r.Key.Path.Replace("\"", "");
                        if (tmpPath.Contains("http") || tmpPath.Contains("https") || tmpPath.Contains("ftp") || tmpPath.Contains("sftp"))
                        {
                            var sIndex = tmpPath.LastIndexOf("/");
                            tmpUri = tmpPath.Substring(0, sIndex);
                        }
                    }
                }

                if (tmpUri == "")
                {
                    WriteLog(TAG, "无法获取基础Url！下载不会进行【请检查m3u8文件】");
                    return;
                }

                Uri baseUri;
                Uri.TryCreate(tmpUri, UriKind.Absolute, out baseUri);
                WriteLog(TAG, String.Format("下载的基础Url={0}", baseUri.AbsoluteUri));
                baseUri = new Uri(baseUri.AbsoluteUri + "/" + Path.GetFileName(filePath), UriKind.Absolute);
                //baseUri = new Uri(baseUri, Path.GetFileName(filePath));//不正确，进入上级目录了

                //一个M3u8里可能定义了不同码率的新m3u8文件。
                if (r.Playlist.Length > 0)
                {
                    HLSPlaylist nextPlaylist = null;

                    #region 多码率选择策略
                    if (r.Playlist.Length == 1)
                        nextPlaylist = r.Playlist[0];
                    else
                    {
                        if (mSelectedBandwidth == null)
                        {
                            WriteLog(TAG, string.Format("停止下载视频流切片；因为存在多码率，且没有选择多码率下载策略或自定义码率无效：{0}", mSelectedBandwidth));

                            WriteLog(TAG, "支持的码率有：");
                            for (var pi = 0; pi < r.Playlist.Length; pi++)
                            {
                                var p = r.Playlist[pi];
                                WriteLog(TAG, String.Format("{0}：码率={1}，分辨率={2}", pi + 1, p.BANDWIDTH, p.RESOLUTION));
                            }
                            return;
                        }
                        //根据多码率下载策略取出需要的码率下载.
                        decimal? lastBandwidth = null;
                        foreach (var p in r.Playlist)
                        {
                            var tmpBand = decimal.Parse(p.BANDWIDTH);
                            if (mSelectedBandwidth == decimal.MaxValue)
                            {
                                if (lastBandwidth == null || tmpBand > lastBandwidth)
                                    nextPlaylist = p;
                                continue;
                            }
                            if (mSelectedBandwidth == decimal.MinValue)
                            {
                                if (lastBandwidth == null || tmpBand < lastBandwidth)
                                    nextPlaylist = p;
                                continue;
                            }
                            if (mSelectedBandwidth == tmpBand)
                            {
                                nextPlaylist = p;
                                //自定义模式，匹配成功直接退出循环。
                                break;
                            }
                        }

                        //自定义模式，匹配失败时，将所有码率输出来，方便定位。
                        if (nextPlaylist == null)
                        {
                            WriteLog(TAG, string.Format("自定义码率匹配失败；自定义码率设置无效：{0}", mSelectedBandwidth));

                            WriteLog(TAG, "支持的码率有：");
                            for (var pi = 0; pi < r.Playlist.Length; pi++)
                            {
                                var p = r.Playlist[pi];
                                WriteLog(TAG, String.Format("{0}：码率={1}，分辨率={2}", pi + 1, p.BANDWIDTH, p.RESOLUTION));
                            }
                            return;
                        }
                    }
                    #endregion

                    WriteLog(TAG, String.Format("下载指定码率={0}，分辨率={1}", nextPlaylist.BANDWIDTH, nextPlaylist.RESOLUTION));
                    WriteLog(TAG, "下载指定码率：路径=" + nextPlaylist.URI);

                    //当下载了 index.m3u8 后指向了具体码率的 4405kb/hls/index.m3u8 此时因为文件名都是一样的，导致有BUG，会忽略下载。
                    if (Path.GetFileName(baseUri.AbsoluteUri) == Path.GetFileName(RemoveUriParas(nextPlaylist.URI)))
                        //于是将老的文件名重命名即可。
                        File.Move(filePath, filePath + ".old.m3u8");

                    var url = new Uri(baseUri, nextPlaylist.URI).AbsoluteUri;
                    var _name = "";
                    if (Path.GetExtension(RemoveUriParas(nextPlaylist.URI)).ToLower() != ".m3u8")
                        _name = Path.GetFileName(RemoveUriParas(nextPlaylist.URI)) + ".m3u8";
                    var gid = mAria2c.AddUri(url, _name, dir, mSelectedUserAgent);
                    mAria2c.ChangePosition(gid, 0, PosType.POS_SET); //将m3u8任务放到下载队列最前面，优先级最高，避免切片下完才下该m3u8
                    WriteLog(TAG, string.Format("下载指定码率：任务ID={0}", gid));
                }
                else
                {
                    if (r.Key != null && r.Key.Path.Length > 0)
                    {
                        WriteLog(TAG, String.Format("需下载的视频流解密KEY={0}", r.Key.Path));
                        WriteLog(TAG, String.Format("需下载的视频流加密方式={0}", r.Key.Method));
                        var url = r.Key.Path.Replace("\"", "");
                        if (!url.ToLower().Contains("http") && !url.ToLower().Contains("https") && !url.ToLower().Contains("ftp") && !url.ToLower().Contains("sftp"))
                        {
                            url = new Uri(baseUri, url).AbsoluteUri;
                        }
                        lock (SequenceLock)
                            mDownDirAndTaskCountMap.Add(dir, r.Parts.Length + 1);
                        mAria2c.AddUri(url, Path.GetFileNameWithoutExtension(RemoveUriParas(url)) + ".key", dir, mSelectedUserAgent);
                    }
                    else
                    {
                        lock (SequenceLock)
                            mDownDirAndTaskCountMap.Add(dir, r.Parts.Length);
                    }

                    WriteLog(TAG, String.Format("需下载的视频流切片块数量={0}", r.Parts.Length));
                    float fDuration = 0.0f;
                    foreach (var p in r.Parts)
                    {
                        var url = "";
                        if (p.Path.IndexOf("://") > -1)//p.Path本身就是完整地址，不需要基础Url
                        {
                            url = new Uri(p.Path).AbsoluteUri;
                        }
                        else
                        {
                            url = new Uri(baseUri, p.Path).AbsoluteUri;
                        }
                        var saveFileName = "";
                        var extension = Path.GetExtension(RemoveUriParas(url)).ToLower();
                        if (extension != ".ts")
                        {
                            saveFileName = Path.GetFileName(RemoveUriParas(url).Replace(extension, ".ts"));

                            //创建标记目录，用于合并时替换切片后缀
                            var flagPath = Path.Combine(dir, "!ExtReplace" + extension);
                            if (!Directory.Exists(flagPath))
                            {
                                Directory.CreateDirectory(flagPath);
                            }
                        }
                        mAria2c.AddUri(url, saveFileName, dir, mSelectedUserAgent);
                        float duration;
                        if (float.TryParse(p.Length.Replace(",", ""), out duration))
                            fDuration += duration;
                    }
                    int iDuration = Convert.ToInt32(Math.Floor(fDuration));
                    WriteLog(TAG, String.Format("需下载的视频流时长={0}", Seconds_To_HMS(iDuration)));
                    //判断切片是否存在同名情况，提前创建标记目录
                    if (Path.GetFileName(RemoveUriParas(r.Parts[0].Path)) == Path.GetFileName(RemoveUriParas(r.Parts[1].Path))
                        && Path.GetFileName(RemoveUriParas(r.Parts[1].Path)) == Path.GetFileName(RemoveUriParas(r.Parts[2].Path)))
                    {
                        //创建标记目录，用于合并时替换m3u8内容
                        var flagPath = Path.Combine(dir, "!DuplicateName" + Path.GetFileName(RemoveUriParas(r.Parts[0].Path)));
                        if (!Directory.Exists(flagPath))
                        {
                            Directory.CreateDirectory(flagPath);
                        }
                    }

                    WriteLog(TAG, String.Format("Aria2正在下载中..."));
                }
            }
            catch (Exception ex)
            {
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);
            }
            finally
            {
                WriteLog(TAG, "执行完毕");
                //Thread.CurrentThread.Abort();
                Thread.CurrentThread.Join(5000);
            }
        }

        private async void btnKillAllAria2_Click(object sender, EventArgs e)
        {
            if (mAria2c != null && !mAllPause)//杀进程前暂停所有任务，避免出错
            {
                WriteLog(null, Environment.NewLine);
                btnPauseAll_Click(sender, e);
            }

            var btn = sender as Button;
            bool isThisBtn = false;
            if (btn != null && btn.Name == "btnKillAllAria2")
                isThisBtn = true;

            btnKillAllAria2.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            var TAG = "杀掉所有Aria2进程";
            WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行中");
            try
            {
                int iProgress = Process.GetProcessesByName("aria2c").Length;
                mAria2c?.SaveSession();
                mAria2c?.PurgeDownloadResult();//清空下载结果，释放内存
                //mAria2c?.Shutdown();
                mAria2c?.ForceShutdown();
                mAria2c?.Dispose();
                mAria2c = null;
                if (iProgress != 0)//等于0时，aria2c根本没启动，不提示关闭是否成功
                {
                    int iTmpCount = 0;
                    while (Process.GetProcessesByName("aria2c").Length > 0)
                    {
                        if (isThisBtn)
                            await Task.Delay(1000);//延时，等待RPC先生效
                        else
                            Thread.Sleep(1000);//关闭窗体，使用线程能够阻塞。Task异步导致不能真正关闭

                        iTmpCount++;
                        if (iTmpCount >= 8)
                            break;
                    }
                    iProgress = Process.GetProcessesByName("aria2c").Length;
                    WriteLog(TAG, string.Format("RPC方式关闭Aria2c执行结果：{0}", iProgress == 0 ? "【成功】" : "[失败]"));
                }
                Aria2cRuntime.ShutDown();
                if (iProgress > 0)//此时Aria2cRuntime杀进程生效
                {
                    if (isThisBtn)
                        await Task.Delay(1500);//延时，等待ShutDown先生效
                    else
                        Thread.Sleep(1500);//关闭窗体，使用线程能够阻塞。Task异步导致不能真正关闭

                    WriteLog(TAG, string.Format("Aria2cRuntime方式关闭Aria2c执行结果：{0}", !Aria2cRuntime.IsLoaded ? "【成功】" : "[失败]"));
                }
                //进程遍历所有相同名字进程。
                var plist = Process.GetProcessesByName("aria2c");
                foreach (var p in plist)
                    if (!p.HasExited)
                        p.Kill();

                WriteLog(TAG, string.Format("执行完毕：尝试杀掉{0}个进程。", plist.Length));
            }
            catch (Exception ex)
            {
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);
            }
            finally
            {
                WriteLog(TAG, "检测是否有进程残留中");
                btnStartAria2.Enabled = Process.GetProcessesByName("aria2c").Length == 0;
                WriteLog(TAG, "检测结果：" + (btnStartAria2.Enabled ? "【无残留】" : "[有残留]"));

                Cursor.Current = Cursors.Default;
                btnKillAllAria2.Enabled = true;

                mAllDownLoadComplete = true;
                txbAria2GlobalInfo.Text = "";
                mDownDirAndTaskCountMap.Clear();
                RefreshDownloadProgress(0);
                RefreshNotifyIconTrayText("");
                DownloadCompletedData = 0;
                txbTotalData.Text = "";
                StopTimer();
                DownloadTotalSeconds = 0;
                txbTotalTime.Text = "";
            }
        }

        private void btnOpenAria2WebUI_Click(object sender, EventArgs e)
        {
            //1.以下两个WebUI是一样的。只是语言不同，且都不够强大。
            //  中文：http://aria2c.com/
            //  原版：http://binux.github.io/yaaw/demo/

            //2.更强大的UI，支持查看和设置各种选项。 #!/settings/rpc/set?protocol=http&host=127.0.0.1&port=6800&interface=jsonrpc
            //  http://ariang.mayswind.net/latest/"

            //3.更强大的UI,支持过滤各种状态的任务,方便重新下载失败的任务.
            //  原版地址：https://ziahamza.github.io/webui-aria2/
            //  将其替换为国内的地址，并且修改为默认localhost模式，方便第一次使用。
            //Process.Start("https://asiontang.gitee.io/webui-aria2/");

            var appDataPath = Path.Combine(Environment.CurrentDirectory, "Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            var indexDocument = Path.Combine(Environment.CurrentDirectory, @"Data\webui-aria2\docs\index.html");
            if (!File.Exists(indexDocument))
            {
                var zipFile = Path.Combine(Environment.CurrentDirectory, @"Data\webui-aria2.zip");
                using (MemoryStream msWebUI = new MemoryStream(global::HLS.Download.UI.Properties.Resources.webui_aria2))
                {
                    //释放到一个新的文件中，需要一个写入文件的文件流
                    using (FileStream fsWrite = new FileStream(zipFile, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;
                        while ((bytesRead = msWebUI.Read(buffer, 0, buffer.Length)) > 0)//循环读取，直到内存流结束
                        {
                            //通过fsWrite流将存放数据的buffer写入到指定路径
                            fsWrite.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                ZipFile.ExtractToDirectory(zipFile, appDataPath, Encoding.Default);//解压zip包                
            }
            Browser browser = new Browser();
            if (!browser.OpenChrome(indexDocument))//谷歌启动失败，调用火狐，再失败调用默认，再失败调用IE，最后直接交给系统
            {
                if (!browser.OpenFireFox(indexDocument))
                {
                    if (!browser.OpenDefaultBrowser(indexDocument))
                    {
                        if (!browser.OpenIE(indexDocument))
                        {
                            Process.Start(indexDocument);
                        }
                    }
                }
            }
        }

        private void WriteLog(String tag, String info)
        {
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
#if DEBUG
            if (tag == null)
                Debug.WriteLine(info);
            else
            {
                Debug.Write(tag);
                Debug.Write("：");
                Debug.WriteLine(info);
            }
#endif
            if (tag == null)
                txbLog.AppendText(info);
            else
            {
                txbLog.AppendText(tag);
                txbLog.AppendText("：");
                txbLog.AppendText(info);
                txbLog.AppendText(Environment.NewLine);
            }
            txbLog.ScrollToCaret();
        }

        private void btnOpenDownloadDir_Click(object sender, EventArgs e)
        {
            //if (mAria2c == null)
            //    btnStartAria2_Click(btnStartAria2, null);
            //Process.Start(Aria2cRuntime.DownLoadDirectory);
            if (mAria2c == null)
                Process.Start(mCurrentDownLoadDir);
            else
                Process.Start(Aria2cRuntime.DownLoadDirectory);
        }

        private void btnSetDownloadLocation_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                mCurrentDownLoadDir = folderBrowserDialog1.SelectedPath;
                if (mAria2c != null)
                    Aria2cRuntime.DownLoadDirectory = folderBrowserDialog1.SelectedPath;
                WriteLog(null, Environment.NewLine);
                WriteLog("设置全局下载目录", folderBrowserDialog1.SelectedPath);
            }
        }

        private void rdbSelectBandWidthCustom_CheckedChanged(object sender, EventArgs e)
        {
            txbCustomBandwidth.Enabled = rdbSelectBandWidthCustom.Checked;
        }

        private void rdbSelectBandWidthMax_CheckedChanged(object sender, EventArgs e)
        {
            mSelectedBandwidth = decimal.MaxValue;
        }

        private void rdbSelectBandWidthMin_CheckedChanged(object sender, EventArgs e)
        {
            mSelectedBandwidth = decimal.MinValue;
        }

        private void txbCustomBandwidth_TextChanged(object sender, EventArgs e)
        {
            decimal tmpdecimal;
            decimal.TryParse(txbCustomBandwidth.Text, out tmpdecimal);
            mSelectedBandwidth = tmpdecimal;
        }

        private async void btnMerge_Click(object sender, EventArgs e)
        {
            //防止没启动Aria2 获取不到正确的下载目录
            //if (mAria2c == null)
            //    btnStartAria2_Click(btnStartAria2, null);

            ((Button)sender).Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            var TAG = "合并";
            WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行中");
            try
            {
                //检测必要的程序都存在
                var ffmpegPath = Path.Combine(Environment.CurrentDirectory, "FFmpeg\\ffmpeg.exe");
                WriteLog(TAG, "FFmpeg所在路径=" + ffmpegPath);
                if (!File.Exists(ffmpegPath))
                {
                    WriteLog(TAG, "检测到本程序路径下的FFmpeg文件不存在！停止执行。");
                    return;
                }

                //获取下载目录
                WriteLog(TAG, "获取下载目录中");
                var DownloadDirectory = mCurrentDownLoadDir;
                if (mAria2c != null)
                    DownloadDirectory = Aria2cRuntime.DownLoadDirectory;
                WriteLog(TAG, "获取到的下载目录=" + DownloadDirectory);

                //遍历下载目录的所有目录里是否有必要的文件
                WriteLog(TAG, "获取下载目录中的所有子目录中");
                var dirs = Directory.GetDirectories(DownloadDirectory);
                WriteLog(TAG, "检测到子目录数量=" + dirs.Length);
                List<Process> dealProcessList = new List<Process>();
                foreach (var dir in dirs)
                {
                    if (dealProcessList.Count >= 3)
                    {
                        WriteLog(TAG, string.Format("执行合并的批处理进程已达{0}个，等待中...", dealProcessList.Count));
                        int iTmpCount = 0;
                        while (dealProcessList.Count >= 3)
                        {
                            await Task.Delay(5000);//延时，等待其他进程结束

                            //cmdProcess Exited事件已处理，以下代码已不再需要
                            //for (int i = 0; i < dealProcessList.Count; i++)
                            //{
                            //    if (dealProcessList[i].HasExited)
                            //    {
                            //        dealProcessList[i].Dispose();
                            //        dealProcessList.RemoveAt(i);
                            //        i--;
                            //    }
                            //}

                            iTmpCount++;
                            if (iTmpCount >= 12 * 5)//5min强制退出，避免死循环
                                break;
                        }
                    }

                    var dirName = Path.GetFileName(dir);

                    //检测是否已经合并过。
                    var flag = Path.Combine(dir, "!YeHadMerge");
                    if (Directory.Exists(flag))
                    {
                        WriteLog(TAG, string.Format("已经手工合并过，跳过！子目录：{0}", dirName));
                        continue;
                    }
                    Directory.CreateDirectory(flag);

                    WriteLog(TAG, string.Format("当前子目录：{0}", dirName));
                    WriteLog(TAG, "检测是否包含.M3U8文件中");

                    //获取目录中所有文件
                    var files = Directory.GetFiles(dir);

                    //提取其中的 两种不同的文件 列表。
                    var m3u8List = new List<string>();
                    var tsList = new List<string>();
                    foreach (var file in files)
                    {
                        switch (Path.GetExtension(file).ToLower())
                        {
                            case ".m3u8":
                                m3u8List.Add(file);
                                break;
                            case ".ts":
                                tsList.Add(file);
                                break;
                            default:
                                break;
                        }
                    }
                    if (m3u8List.Count == 0)
                    {
                        WriteLog(TAG, "没有检测到.M3U8文件，跳过！");
                        continue;
                    }
                    if (tsList.Count == 0)
                    {
                        WriteLog(TAG, "没有检测到.ts文件，跳过！");
                        continue;
                    }

                    //先从 m3u8 文件里提取出 ts 文件列表。
                    foreach (var m3u8File in m3u8List)
                    {
                        //忽略自己生成的新清单文件
                        if (m3u8File.ToLower().EndsWith("_new.m3u8") || m3u8File.ToLower().EndsWith(".old.m3u8"))
                        {
                            WriteLog(TAG, string.Format("分析{0}文件结果：此文件是本程序临时生成的TS清单文件，跳过！"
                                , Path.GetFileName(m3u8File)));
                            continue;
                        }

                        WriteLog(TAG, string.Format("分析{0}文件中", Path.GetFileName(m3u8File)));
                        var txt = File.ReadAllText(m3u8File);
                        bool isExtReplace = false;

                        //检测是否为TS清单文件
                        if (!txt.Contains(".ts"))
                        {
                            if (!txt.Contains("EXTINF"))
                            {
                                WriteLog(TAG, string.Format("分析{0}文件结果：此文件不是TS清单文件，跳过！"
                                , Path.GetFileName(m3u8File)));
                                continue;
                            }
                            else
                            {
                                //根据标记目录，替换切片后缀
                                foreach (var _dir in Directory.GetDirectories(dir))
                                {
                                    var _dirName = Path.GetFileName(_dir);
                                    if (_dirName.StartsWith("!ExtReplace"))
                                    {
                                        var tmpExtension = _dirName.Replace("!ExtReplace", "");
                                        txt = txt.Replace(tmpExtension, ".ts");
                                        isExtReplace = true;
                                    }
                                }
                            }
                        }

                        //计算总的分片数量。
                        {
                            var count1 = getCountOfString(txt, ".ts");
                            var count2 = getCountOfString(txt, "EXTINF");
                            if (count1 != count2)
                            {
                                WriteLog(TAG, string.Format("分析{0}文件结果：.ts出现次数={1} 与 EXTINF出现次数={2} 不一致，跳过！"
                                    , Path.GetFileName(m3u8File), count1, count2));
                                continue;
                            }
                            else if (tsList.Count != count1)
                            {
                                //WriteLog(TAG, string.Format("分析{0}文件结果：.ts切片理论数量={1} 与 实际数量={2} 不一致，跳过！"
                                //    , Path.GetFileName(m3u8File), count1, tsList.Count));
                                //continue;
                                WriteLog(TAG, string.Format("分析{0}文件结果：.ts切片理论数量={1} 与 实际数量={2} 不一致！【继续执行合并，请自行确认最终视频文件】"
                                    , Path.GetFileName(m3u8File), count1, tsList.Count));
                            }
                            else
                                WriteLog(TAG, string.Format("分析{0}文件结果：检测到正确数量的.ts切片={1}"
                                , Path.GetFileName(m3u8File), tsList.Count));
                        }

                        //检测是否存在重名标记
                        bool isDuplicateName = false;
                        string duplicateName = "";
                        foreach (var _dir in Directory.GetDirectories(dir))
                        {
                            var _dirName = Path.GetFileName(_dir);
                            if (_dirName.StartsWith("!DuplicateName"))
                            {
                                duplicateName = _dirName.Replace("!DuplicateName", "");
                                isDuplicateName = true;
                                break;
                            }
                        }
                        //改造此清单文件形成本地可用的新清单文件
                        var newM3U8File = getNewM3U8File(txt, isDuplicateName, duplicateName, isExtReplace, m3u8File);

                        //拼接最终执行的命令行脚本
                        {
                            var cmdPath = string.Format("{0}\\{1}.bat", Path.GetDirectoryName(newM3U8File), Path.GetFileNameWithoutExtension(newM3U8File));

                            WriteLog(TAG, string.Format("分析{0}文件结果：生成批处理脚本中={1}"
                                , Path.GetFileName(m3u8File), Path.GetFileName(cmdPath)));

                            //{ffmpeg} -threads 1 -i {本地TS列表.m3u8} -c copy {文件名}.mkv
                            var cmd = txbMergeCMD.Text;
                            cmd = cmd.Replace("{ffmpeg}", ffmpegPath);
                            cmd = cmd.Replace("{本地TS列表.m3u8}", newM3U8File);
                            cmd = cmd.Replace("{文件名}", dirName);

                            File.WriteAllText(cmdPath, cmd, Encoding.Default);

                            WriteLog(TAG, string.Format("分析{0}文件结果：执行批处理脚本中={1}"
                                , Path.GetFileName(m3u8File), Path.GetFileName(cmdPath)));

                            //Process.Start(cmdPath);
                            //using (Process cmdProcess = new Process())//使用Using自动释放，导致调用HasExited出现“没有与此对象关联的进程”、Exited事件不出发
                            {
                                Process cmdProcess = new Process();
                                cmdProcess.StartInfo.UseShellExecute = false;
                                cmdProcess.StartInfo.FileName = cmdPath;
                                cmdProcess.StartInfo.CreateNoWindow = false;
                                cmdProcess.EnableRaisingEvents = true;
                                cmdProcess.Exited += (object _sender, EventArgs _e) =>
                                {
                                    WriteLog(TAG, string.Format("-->子目录“{0}”已合并完毕，请确认", dirName));
                                    Process p = (Process)_sender;
                                    if (dealProcessList.Contains(p))
                                        dealProcessList.Remove(p);
                                    p.Dispose();
                                };
                                cmdProcess.Start();
                                WriteLog(TAG, string.Format("请等待批处理“{0}”执行...", Path.GetFileName(cmdPath)));

                                dealProcessList.Add(cmdProcess);
                            }

                        }
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
                GC.Collect();
                GC.WaitForPendingFinalizers();
                WriteLog(TAG, "执行完毕");
                Cursor.Current = Cursors.Default;
                ((Button)sender).Enabled = true;
            }
        }

        private string getNewM3U8File(string txt, bool isDuplicateName, string duplicateName, bool isExtReplace, string oldFile)
        {
            //对包含Key的m3u8设置uri指向本地
            var sIndex0 = txt.IndexOf("EXT-X-KEY", 0);
            var keyFlag = false;
            if (sIndex0 != -1)
            {
                var sIndex0_2 = txt.IndexOf("\n", sIndex0);
                if (sIndex0_2 == -1)
                {
                    sIndex0_2 = txt.IndexOf("#", sIndex0);
                }
                string strEXT_X_KEY = txt.Substring(sIndex0, sIndex0_2 - sIndex0);
                strEXT_X_KEY = strEXT_X_KEY.Substring(strEXT_X_KEY.IndexOf(':') + 1);
                if (strEXT_X_KEY.Contains("URI"))
                {
                    //string strUri = (strEXT_X_KEY.Split(',')[1]).Split('=')[1];//此种截取不准确
                    string strUri = "";
                    foreach (var part in strEXT_X_KEY.Split(','))
                    {
                        if (part.Split('=')[0] == "URI")
                        {
                            strUri = part.Substring(part.IndexOf('=') + 1);
                            break;
                        }
                    }
                    var sIndex0_3 = strUri.Replace("\"", "").LastIndexOf("/");
                    string strKeyName = strUri.Replace("\"", "").Substring(sIndex0_3 + 1);
                    strKeyName = Path.GetFileNameWithoutExtension(RemoveUriParas(strKeyName)) + ".key";
                    //strKeyName = string.Format("\"{0}\"", strKeyName);
                    txt = txt.Replace(strUri, strKeyName);
                    keyFlag = true;
                }
            }

            var needReplaceString = "";
            //找到第一个 / 符号
            var sIndex1 = txt.IndexOf("/");
            var question_mark_idx = txt.IndexOf('?');

            //假如找不到URI的分隔符 / 则不需要转换了。情况0.
            if (sIndex1 == -1 && question_mark_idx == -1)
            {
                /*--------------------------------------------------
                 * 情况0：
                 * #EXTINF:4.56,
                 * vKOnx78R3460000.ts
                 *--------------------------------------------------*/
                if (!keyFlag && !isExtReplace && !isDuplicateName)
                    return oldFile;

            }
            else
            {
                #region 旧的替换逻辑，后面是新版本
                ////找到 这个符号 之后的 .ts 
                //var tsIndex = txt.IndexOf(".ts", sIndex1);

                ////再从 tsIndex 往前找 / 符号
                //var sIndex2 = txt.LastIndexOf("/", tsIndex);

                ////当 sIndex2 = sIndex1 时，说明是情况1. 那么就不需要转换新列表
                //if (sIndex1 == sIndex2)
                //    /*--------------------------------------------------
                //     * 情况1：
                //     * #EXTINF:4.56,
                //     * /vKOnx78R3460000.ts
                //     *---------------------------------------------------*/
                //    needReplaceString = "/";
                //else
                //{
                //    //查找第一个 分隔符 左边的换行符
                //    var newLineIndex = txt.LastIndexOf("\n", sIndex1);

                //    //假如 左边的换行符 和 分隔符位置一致时
                //    if (newLineIndex == sIndex1)
                //    {
                //        /*--------------------------------------------------
                //         * 情况2：
                //         * #EXTINF:4.56,
                //         * /20180303/E1okxWlJ/800kb/hls/vKOnx78R3460000.ts
                //         *---------------------------------------------------*/

                //        //截取两个 / 符号之间的字符串
                //        needReplaceString = txt.Substring(sIndex1, sIndex2 - sIndex1 + 1/*+1目的是把/符号去掉！*/);
                //    }
                //    else
                //    {
                //        /*--------------------------------------------------
                //         * 情况3：
                //         * #EXTINF:4.56,
                //         * http://a.b.c/d/vKOnx78R3460000.ts
                //         *---------------------------------------------------*/

                //        //截取两个 / 符号之间的字符串
                //        needReplaceString = txt.Substring(newLineIndex + 1/*+1目的是跳过换行符*/, sIndex2 - (newLineIndex + 1) + 1/*+1目的是把/符号去掉！*/);
                //    }
                //}
                #endregion

                /*
                 * 替换掉切片前面的相对网址
                 * （1）/vKOnx78R3460000.ts
                 * （2）/20180303/E1okxWlJ/vKOnx78R3460000.ts
                 * （3）http://a.b.c/d/vKOnx78R3460000.ts
                 */
                string[] lines = txt.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                List<string> new_text_lines = new List<string>();
                string line;
                for (int i = 0; i < lines.Length; i++)
                {
                    line = lines[i];
                    if (line.StartsWith("#EXTINF:"))
                    {
                        new_text_lines.Add(line);
                        line = RemoveUriParas(lines[i + 1]);

                        var lastIndex = line.LastIndexOf("/");
                        if (lastIndex != -1)
                            line = line.Substring(lastIndex + 1);
                        else
                            line = line.Trim();
                        new_text_lines.Add(line);
                        i++;
                    }
                    else if (line.StartsWith("#URI:"))//自定义的下载基础地址，已经不再需要
                        continue;
                    else if (line.StartsWith("#EXT-X-ENDLIST"))
                    {
                        new_text_lines.Add(line);
                        break;
                    }
                    else
                        new_text_lines.Add(line);
                }
                line = null;
                txt = string.Join(Environment.NewLine, new_text_lines.ToArray());
            }

            var newText = txt;
            //将需要替换的文本都替换为空格。相当于删除掉。只保留后面的文件名 vKOnx78R3460000.ts
            if (needReplaceString != "")
                newText = txt.Replace(needReplaceString, "");

            //开始处理重名问题
            if (isDuplicateName && duplicateName != "")
            {
                string[] lines = newText.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                List<string> new_text_lines = new List<string>();
                int tsIndex = 0;
                foreach (var line in lines)
                {
                    if (line.Contains(duplicateName))
                    {
                        if (tsIndex == 0)
                            new_text_lines.Add(duplicateName);
                        else
                        {
                            var file_name = string.Format("{0}.{1}{2}",
                                Path.GetFileNameWithoutExtension(duplicateName),
                                tsIndex,
                                Path.GetExtension(duplicateName)
                                );
                            new_text_lines.Add(file_name);
                        }
                        tsIndex++;
                    }
                    else
                        new_text_lines.Add(line);
                }
                newText = string.Join(Environment.NewLine, new_text_lines.ToArray());
            }

            //将替换好的新清单文本写入新文件
            var newFile = string.Format("{0}\\{1}_new.m3u8", Path.GetDirectoryName(oldFile), Path.GetFileNameWithoutExtension(oldFile));
            File.WriteAllText(newFile, newText);
            return newFile;
        }

        private int getCountOfString(string sourceText, string searchText)
        {
            var count = 0;
            var lastIndex = 0;
            while ((lastIndex = sourceText.IndexOf(searchText, lastIndex)) != -1)
            {
                //lastIndex++;
                lastIndex += searchText.Length;
                count++;
            }
            return count;
        }

        private void btnPauseAll_Click(object sender, EventArgs e)
        {
            if (mAria2c == null)
                btnStartAria2_Click(btnStartAria2, null);
            var TAG = "全部暂停下载";
            if (mAria2c == null)
            {
                WriteLog(TAG, "Aria2未启动！");
                return;
            }

            var result = mAria2c.ForcePauseAll();
            //WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行完毕");
            WriteLog(TAG, "执行结果=" + result);

            if (result)
            {
                RefreshNotifyIconTrayText("已暂停");
                mAllPause = true;
                if (timer != null)
                    timer.Enabled = false;
            }
        }

        private void btnUnPauseAll_Click(object sender, EventArgs e)
        {
            if (mAria2c == null)
                btnStartAria2_Click(btnStartAria2, null);
            var TAG = "全部启动下载";
            if (mAria2c == null)
            {
                WriteLog(TAG, "Aria2未启动！");
                return;
            }

            var result = mAria2c.UnPauseAll();
            //WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行完毕");
            WriteLog(TAG, "执行结果=" + result);
            if (result)
            {
                mAllPause = false;
                RefreshNotifyIconTrayText("");
                if (mAllDownLoadComplete)//不是下载中途暂停后开始
                    DownloadTotalSeconds = 0;
                if (timer != null)
                    timer.Enabled = true;
            }
        }

        private void cbbUserAgent_SelectedIndexChanged(object sender, EventArgs e)
        {
            mSelectedUserAgent = cbbUserAgent.Text;
        }

        private void btnDelSession_Click(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            var TAG = "清空Aria2下载记录";
            WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行中");
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Aria2\\aria2c.session");
                var bakPath = path + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak";

                WriteLog(TAG, string.Format("备份文件={0}", bakPath));
                File.Copy(path, bakPath);

                WriteLog(TAG, string.Format("删除文件={0}", path));
                File.Delete(path);

                WriteLog(TAG, string.Format("新建空文件={0}", path));
                File.WriteAllText(path, "");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);
            }
            finally
            {
                ((Button)sender).Enabled = true;

                Cursor.Current = Cursors.Default;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //判断关闭事件Reason来源于用户关闭，点击关闭按钮或者按下alt+F4
            if (e.CloseReason == CloseReason.UserClosing && mExitRequireConfirm)
            {
                DialogResult select = MessageBox.Show(this, "是否退出程序？\n【是】退出程序    【否】最小化到托盘"
                    , "退出提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (select == DialogResult.Yes)
                {
                    Exit(sender, e);//退出程序
                }
                else if (select == DialogResult.No)
                {
                    e.Cancel = true;//取消"关闭窗口"事件
                    //最小化到托盘程序（此时窗体一定是显示的，UserClosing窗体是可见的）
                    //一种少见情况：提示选择时，托盘隐藏窗体，所以必须判断
                    if (this.Visible)
                        ToggleWindowState(sender, e);
                }
                else
                {
                    e.Cancel = true;//取消"关闭窗口"事件
                }
            }
            else//托盘退出、任务管理器关闭程序、操作系统关闭窗体
            {
                Exit(sender, e);
            }
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit(object sender, EventArgs e)
        {
            RefreshDownloadProgress(0);
            RefreshNotifyIconTrayText("正在退出");
            if (mAria2c != null)
                btnKillAllAria2_Click(sender, e);
            var TAG = "主程序";
            WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "正在关闭");

            //readM3U8Thread = null;
            StopTimer();
            mAria2c = null;
            SaveConfig(TAG);

            var logPath = Path.Combine(Environment.CurrentDirectory, "Log\\");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            var newLogFile = logPath + string.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            WriteLog(TAG, "关闭成功。");
            File.WriteAllText(newLogFile, txbLog.Text, Encoding.UTF8);

            //释放托盘程序
            notifyIconTray.Visible = false;
            notifyIconTray.Dispose();
            notifyIconTray = null;
            //关闭程序
            Program.CloseSingletonInstanceMutex();
            this.Dispose(true);
            Application.Exit();
        }

        /// <summary>
        /// 保存配置到ini
        /// </summary>
        private void SaveConfig(string TAG)
        {
            var iniPath = Path.Combine(Environment.CurrentDirectory, "settings.ini");
            if (!File.Exists(iniPath))
            {
                bool flag = INIHelper.CreateIni(iniPath);
                if (!flag)
                    WriteLog(TAG, "配置文件创建失败！无法保存配置信息");
                return;
            }
            try
            {
                SaveSectionKey("Aria2", "DownLoadDir", mCurrentDownLoadDir, iniPath);
                SaveSectionKey("Aria2", "LastTaskDir", mLastTaskDir, iniPath);
                SaveSectionKey("Aria2", "DownloadSpeedLimit", txbMaxDownloadSpeed.Text, iniPath);
                SaveSectionKey("Aria2", "ConfigFileName", cbbConfigFileName.Text, iniPath);

                SaveSectionKey("Download", "SelectedBandWidth", mSelectedBandwidth?.ToString(), iniPath);
                SaveSectionKey("Download", "AfterDownloadCompleted", cbbAction.Text, iniPath);
                SaveSectionKey("Download", "SelectedUserAgent", mSelectedUserAgent, iniPath);
                SaveSectionKey("Download", "MergeTasksToGroup", ckbMergeTasks.Checked.ToString(), iniPath);

                SaveSectionKey("General", "PlayCompletedSound", mPlayCompletedSound.ToString(), iniPath);
                SaveSectionKey("General", "ExitRequireConfirm", mExitRequireConfirm.ToString(), iniPath);

            }
            catch (Exception ex) { WriteLog(TAG, "保存配置信息失败！键名称：" + ex.Message); }
        }

        private void SaveSectionKey(string section, string key, string value, string ini_file)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            int iReturnValue = INIHelper.Write(section, key, value, ini_file);
            if (iReturnValue == 0)
                throw new Exception(key);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var tmpStr = "";
                foreach (var filepath in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    if (Path.GetExtension(filepath.ToString()).ToLower() == ".m3u8")
                    {
                        //string foldName = new DirectoryInfo(Path.GetDirectoryName(filepath.ToString())).Name;
                        string foldName = Path.GetFileName(Path.GetDirectoryName(filepath.ToString()));
                        tmpStr += foldName + "|" + filepath.ToString() + Environment.NewLine;
                    }
                    else
                    {
                        WriteLog(null, Environment.NewLine);
                        WriteLog("拖曳文件到窗口", Path.GetFileName(filepath.ToString()) + "：不支持的文件类型！");
                    }
                }
                if (txbUrls.Text.Trim() != "" && !txbUrls.Text.EndsWith(Environment.NewLine))
                    tmpStr = Environment.NewLine + tmpStr;

                txbUrls.Text += tmpStr;
                txbUrls.SelectionStart = txbUrls.Text.Length;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// 将秒数转化为时分秒
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private string Seconds_To_HMS(int duration)
        {
            //ts = new TimeSpan(0, 0, duration);
            ts = TimeSpan.FromSeconds(duration);
            var str = "00:00:00";
            if (ts.Hours > 0)
            {
                str = String.Format("{0:00}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = "00:" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:00:" + String.Format("{0:00}", ts.Seconds);
            }
            return str;
        }

        private void btnRmAllTasks_Click(object sender, EventArgs e)
        {
            if (mAria2c == null)
                btnStartAria2_Click(btnStartAria2, null);
            var TAG = "移除剩余下载任务";
            if (mAria2c == null)
            {
                WriteLog(null, Environment.NewLine);
                WriteLog(TAG, "Aria2未启动！");
                return;
            }
            if (!mAllPause)
            {
                WriteLog(null, Environment.NewLine);
                btnPauseAll_Click(sender, e);
            }
            ((Button)sender).Enabled = false;
            Cursor.Current = Cursors.WaitCursor;
            WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行中");
            try
            {
                int iRemovedCount = mAria2c.ForceRemoveAll();
                WriteLog(TAG, string.Format("执行完毕：共移除{0}个下载任务。", iRemovedCount));

                var result = mAria2c.PurgeDownloadResult();//清空下载结果，释放内存
                TAG = "释放内存";
                WriteLog(TAG, "执行完毕");
                WriteLog(TAG, "执行结果=" + result);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                ((Button)sender).Enabled = true;
                mAllDownLoadComplete = true;
                StopTimer();
                mDownDirAndTaskCountMap.Clear();
                RefreshDownloadProgress(0);
                RefreshNotifyIconTrayText("已取消");
            }
        }

        private void cbbConfigFileName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var config_name = Path.GetFileNameWithoutExtension(cbbConfigFileName.Text);
            string cmdFileName = "FFmpeg\\";
            if (config_name == "aria2c") {
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, "FFmpeg\\aria2c.txt")))
                    cmdFileName += "aria2c.txt";
                else
                    cmdFileName += "cmd.txt";
            }
            else
                cmdFileName += config_name + ".txt";
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, cmdFileName)))
            {
                txbMergeCMD.Text = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, cmdFileName), Encoding.Default);
            }
            else {
                //txbMergeCMD.Text = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "FFmpeg\\cmd.txt"), Encoding.Default);
                //txbMergeCMD.ResetText();//清空，此处不适用
                txbMergeCMD.Text = txbMergeCMD.Tag.ToString();
            }
        }

        /// <summary>
        /// 移除Uri中？及其后的参数，不移除获取文件名或创建目录会抛错
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private string RemoveUriParas(string uri)
        {
            if (uri.IndexOf('?') != -1)
            {
                uri = uri.Substring(0, uri.IndexOf('?'));
            }
            return uri;
        }

        private void btnApplyMaxSpeed_Click(object sender, EventArgs e)
        {
            var TAG = "应用自定义限速";
            try
            {
                int iDownloadSpeedLimit;
                if (int.TryParse(txbMaxDownloadSpeed.Text.Trim(), out iDownloadSpeedLimit))
                {
                    if (mAria2c == null)
                        btnStartAria2_Click(btnStartAria2, null);
                    WriteLog(TAG, "应用中");
                    if (mAria2c == null)
                    {
                        WriteLog(TAG, "Aria2未启动！");
                        return;
                    }
                    Aria2cOption option = new Aria2cOption();
                    option.SetOption("max-overall-download-limit", string.Format("{0}k", iDownloadSpeedLimit));
                    bool isSuccess = mAria2c.ChangeGlobalOption(option);
                    WriteLog(TAG, string.Format("应用结果：{0}", isSuccess ? "【成功】" : "[失败]"));
                    if (isSuccess && iDownloadSpeedLimit != 0)
                        WriteLog(TAG, string.Format("最大下载速度将会限制为{0}KB/s", iDownloadSpeedLimit));
                    else if (isSuccess && iDownloadSpeedLimit == 0)
                        WriteLog(TAG, "当前最大下载速度无限制");
                }
                else
                {
                    WriteLog(TAG, "错误！输入的不是整数");
                }
            }
            catch (Exception ex)
            {
                WriteLog(TAG, "出现异常");
                WriteLog(TAG, ex.Message);
            }
        }

        //private void RefreshDownloadData(long dataLength)
        private void RefreshDownloadData()
        {
            if (txbTotalData.InvokeRequired)
            {
                while (!txbTotalData.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (txbTotalData.Disposing || txbTotalData.IsDisposed)
                        return;
                }
                //txbTotalData.Invoke(new MethodInvoker(() => RefreshDownloadData(dataLength)));
                txbTotalData.Invoke(new MethodInvoker(() => RefreshDownloadData()));
                return;
            }

            //DownloadCompletedData += dataLength;
            txbTotalData.Text = HumanReadableSize(DownloadCompletedData);
        }

        /// <summary>
        /// 转换字节为数据单位
        /// </summary>
        /// <param name="size">字节值</param>
        /// <returns></returns>
        private String HumanReadableSize(long lsize)
        {
            double size = Convert.ToDouble(lsize);
            double mod = 1024.0;
            int idx = 0;
            while (size >= mod)
            {
                size /= mod;
                idx++;
            }
            return Math.Round(size, 2) + units[idx];
        }

        /// <summary>
        /// 启动定时器
        /// </summary>
        private void StartTimer()
        {
            if (timer == null && !mAllDownLoadComplete && !mAllPause)
            {
                InitTimer();

                timer.Enabled = true;
                timer.Start();
            }
        }

        /// <summary>
        /// 关闭定时器
        /// </summary>
        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        /// <summary>
        /// 初始化定时器
        /// </summary>
        private void InitTimer()
        {
            //timer = new System.Timers.Timer(1000);
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(TimePlus);
            ////设置为重复执行
            //timer.AutoReset = true;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TimePlus);
        }

        /// <summary>
        /// 定时器超时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TimePlus(object sender, EventArgs e)
        {
            try
            {
                DownloadTotalSeconds++;
                RefreshTotalSeconds(Seconds_To_HMS(DownloadTotalSeconds));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //已知问题：计时无法停止
                //if (timer == null)
                //    StartTimer();
            }
        }

        private void RefreshTotalSeconds(string timeStr)
        {
            if (txbTotalTime.InvokeRequired)
            {
                while (!txbTotalTime.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (txbTotalTime.Disposing || txbTotalTime.IsDisposed)
                        return;
                }
                txbTotalTime.Invoke(new MethodInvoker(() => RefreshTotalSeconds(timeStr)));
                return;
            }
;
            txbTotalTime.Text = timeStr;
        }

        private void RefreshNotifyIconTrayText(string descStr)
        {
            if (mAllPause && !mAllDownLoadComplete)
                return;

            notifyIconTray.Text = string.Format("Ye.M3U8.Downloader\n{0}", descStr);
        }

        private void RefreshDownloadProgress(int iProgress)
        {
            if (pbDownProgress.InvokeRequired)
            {
                while (!pbDownProgress.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (pbDownProgress.Disposing || pbDownProgress.IsDisposed)
                        return;
                }
                pbDownProgress.Invoke(new MethodInvoker(() => RefreshDownloadProgress(iProgress)));
                return;
            }
;
            pbDownProgress.Value = iProgress;
        }

        private void txbMergeCMD_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void picbCirculate_Click(object sender, EventArgs e)
        {
            cbbConfigFileName.SelectedIndex = (cbbConfigFileName.SelectedIndex == cbbConfigFileName.Items.Count - 1) ? 0 : cbbConfigFileName.SelectedIndex + 1;
        }

        private void txbReadOnly_GotFocus(object sender, EventArgs e)
        {
            ForControl.HideCaret((sender as TextBox).Handle);
        }

        private void txbUrls_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //(sender as TextBox).SelectAll();
            var txb = sender as TextBox;
            if (txb.SelectedText.EndsWith("|"))
            {
                txb.SelectionLength -= 1;
            }
            else
            {
                var idx = txb.SelectionStart;//常规index，从0开始
                if (idx > 1 && txb.Text.Substring(idx - 1, 1) == "|")
                {
                    var idx2 = txb.Text.IndexOf(Environment.NewLine, idx);
                    if (idx2 > -1)
                    {
                        txb.SelectionLength = idx2 - idx;
                    }
                    else
                    {
                        txb.SelectionLength = txb.Text.Substring(idx).Length;
                    }
                }
            }
        }

        private void ckbMergeTasks_CheckedChanged(object sender, EventArgs e)
        {
            txbUrls_TextChanged(sender, e);
        }

        private void btnLastTaskDir_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(mLastTaskDir))
            {
                var lastDirName = Path.GetFileName(mLastTaskDir);
                var parentDir = Directory.GetParent(mLastTaskDir).FullName;
                if (Directory.Exists(parentDir))
                {
                    foreach (var dir in Directory.GetDirectories(parentDir))
                    {
                        if (Path.GetFileName(dir).Contains(lastDirName))
                        {
                            Process.Start(dir);
                            return;
                        }
                    }
                    Process.Start(parentDir);
                }
                else
                {
                    Process.Start(mCurrentDownLoadDir);
                }
            }
            else
            {
                Process.Start(mLastTaskDir);
            }
        }

        private async void llblDeleteMergedFolders_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult select = MessageBox.Show(this, "删除后不可恢复，是否确认删除？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (select == DialogResult.Yes)
            {
                ((LinkLabel)sender).Enabled = false;
                Cursor.Current = Cursors.WaitCursor;
                var TAG = "删除已合并目录";
                WriteLog(null, Environment.NewLine);
                WriteLog(TAG, "执行中");
                try
                {
                    //获取下载目录
                    WriteLog(TAG, "获取下载目录中");
                    var DownloadDirectory = mCurrentDownLoadDir;
                    if (mAria2c != null)
                        DownloadDirectory = Aria2cRuntime.DownLoadDirectory;
                    WriteLog(TAG, "获取到的下载目录=" + DownloadDirectory);

                    WriteLog(TAG, "获取下载目录中的所有子目录中");
                    var dirs = Directory.GetDirectories(DownloadDirectory);
                    WriteLog(TAG, "检测到子目录数量=" + dirs.Length);
                    int iDeleteCount = 0;
                    foreach (var dir in dirs)
                    {
                        var dirName = Path.GetFileName(dir);
                        //检测是否已经合并过。
                        var flag = Path.Combine(dir, "!YeHadMerge");
                        if (Directory.Exists(flag))
                        {
                            //WriteLog(TAG, string.Format("已经合并过，删除！目录：{0}", dirName));
                            /*
                            try { Directory.Delete(dir, true); } catch {
                                WriteLog(TAG, string.Format("删除失败，目录：{0}", dirName));
                                continue;
                            }
                            iDeleteCount++;
                            WriteLog(TAG, string.Format("已成功删除目录：{0}", dirName));
                            */
                            //另开线程，避免删除大Size目录时界面卡住
                            var task = new Task<int>(() =>
                            {
                                try { Directory.Delete(dir, true); }
                                catch
                                {
                                    WriteLog(TAG, string.Format("删除失败，目录：{0}", dirName));
                                    return 0;
                                }
                                WriteLog(TAG, string.Format("已成功删除目录：{0}", dirName));
                                return 1;
                            });
                            task.Start();
                            var iReturnCode = await task;
                            iDeleteCount += iReturnCode;
                        }
                        else
                        {
                            WriteLog(TAG, string.Format("没有合并过，跳过！目录：{0}", dirName));
                            continue;
                        }
                    }

                    WriteLog(TAG, string.Format("本次共删除{0}个已合并过的目录", iDeleteCount));
                }
                catch (Exception ex)
                {
                    WriteLog(TAG, "出现异常");
                    WriteLog(TAG, ex.Message);
                }
                finally
                {
                    WriteLog(TAG, "执行完毕");
                    Cursor.Current = Cursors.Default;
                    ((LinkLabel)sender).Enabled = true;
                }
            }
            else if (select == DialogResult.No)
            {
                return;
            }
        }

        private void AutoMergeAfterDownOver(object objDstDir)
        {
            var dir = objDstDir as string;
            var TAG = "自动合并";
            //WriteLog(null, Environment.NewLine);
            WriteLog(TAG, "执行中");
            try
            {
                //检测必要的程序都存在
                var ffmpegPath = Path.Combine(Environment.CurrentDirectory, "FFmpeg\\ffmpeg.exe");
                //WriteLog(TAG, "FFmpeg所在路径=" + ffmpegPath);
                if (!File.Exists(ffmpegPath))
                {
                    WriteLog(TAG, "检测到本程序路径下的FFmpeg文件不存在！停止执行。");
                    return;
                }

                var dirName = Path.GetFileName(dir);
                //检测是否已经合并过。
                var flag = Path.Combine(dir, "!YeHadMerge");
                if (Directory.Exists(flag))
                {
                    WriteLog(TAG, string.Format("已经手工合并过，跳过！目录名：{0}", dirName));
                    return;
                }
                Directory.CreateDirectory(flag);

                WriteLog(TAG, string.Format("当前目录名：{0}", dirName));
                //WriteLog(TAG, "检测是否包含.M3U8文件中");

                //获取目录中所有文件
                var files = Directory.GetFiles(dir);
                //提取其中的 两种不同的文件 列表。
                var m3u8List = new List<string>();
                //var tsList = new List<string>();
                int tsCount = 0;
                foreach (var file in files)
                {
                    switch (Path.GetExtension(file).ToLower())
                    {
                        case ".m3u8":
                            m3u8List.Add(file);
                            break;
                        case ".ts":
                            //tsList.Add(file);
                            tsCount++;
                            break;
                        default:
                            break;
                    }
                }
                if (m3u8List.Count == 0)
                {
                    WriteLog(TAG, "没有检测到.M3U8文件，跳过！");
                    return;
                }
                if (/*tsList.Count == 0*/tsCount == 0)
                {
                    WriteLog(TAG, "没有检测到.ts文件，跳过！");
                    return;
                }

                //先从 m3u8 文件里提取出 ts 文件列表。
                foreach (var m3u8File in m3u8List)
                {
                    //忽略自己生成的新清单文件
                    if (m3u8File.ToLower().EndsWith("_new.m3u8") || m3u8File.ToLower().EndsWith(".old.m3u8"))
                    {
                        WriteLog(TAG, string.Format("分析{0}文件结果：此文件是本程序临时生成的TS清单文件，跳过！"
                            , Path.GetFileName(m3u8File)));
                        continue;
                    }

                    WriteLog(TAG, string.Format("分析{0}文件中", Path.GetFileName(m3u8File)));
                    var txt = File.ReadAllText(m3u8File);
                    bool isExtReplace = false;

                    //检测是否为TS清单文件
                    if (!txt.Contains(".ts"))
                    {
                        if (!txt.Contains("EXTINF"))
                        {
                            WriteLog(TAG, string.Format("分析{0}文件结果：此文件不是TS清单文件，跳过！"
                            , Path.GetFileName(m3u8File)));
                            continue;
                        }
                        else
                        {
                            //根据标记目录，替换切片后缀
                            foreach (var _dir in Directory.GetDirectories(dir))
                            {
                                var _dirName = Path.GetFileName(_dir);
                                if (_dirName.StartsWith("!ExtReplace"))
                                {
                                    var tmpExtension = _dirName.Replace("!ExtReplace", "");
                                    txt = txt.Replace(tmpExtension, ".ts");
                                    isExtReplace = true;
                                }
                            }
                        }
                    }

                    //计算总的分片数量。
                    {
                        var count1 = getCountOfString(txt, ".ts");
                        var count2 = getCountOfString(txt, "EXTINF");
                        if (count1 != count2)
                        {
                            WriteLog(TAG, string.Format("分析{0}文件结果：.ts出现次数={1} 与 EXTINF出现次数={2} 不一致，跳过！"
                                , Path.GetFileName(m3u8File), count1, count2));
                            continue;
                        }
                        else if (/*tsList.Count != count1*/tsCount != count1)
                        {
                            WriteLog(TAG, string.Format("分析{0}文件结果：.ts切片理论数量={1} 与 实际数量={2} 不一致，跳过！"
                                , Path.GetFileName(m3u8File), count1, tsCount));
                            continue;
                        }
                        else
                            WriteLog(TAG, string.Format("分析{0}文件结果：检测到正确数量的.ts切片={1}"
                            , Path.GetFileName(m3u8File), tsCount));
                    }

                    //检测是否存在重名标记
                    bool isDuplicateName = false;
                    string duplicateName = "";
                    foreach (var _dir in Directory.GetDirectories(dir))
                    {
                        var _dirName = Path.GetFileName(_dir);
                        if (_dirName.StartsWith("!DuplicateName"))
                        {
                            duplicateName = _dirName.Replace("!DuplicateName", "");
                            isDuplicateName = true;
                            break;
                        }
                    }
                    //改造此清单文件形成本地可用的新清单文件
                    var newM3U8File = getNewM3U8File(txt, isDuplicateName, duplicateName, isExtReplace, m3u8File);

                    //拼接最终执行的命令行脚本
                    {
                        var cmdPath = string.Format("{0}\\{1}.bat", Path.GetDirectoryName(newM3U8File), Path.GetFileNameWithoutExtension(newM3U8File));

                        WriteLog(TAG, string.Format("分析{0}文件结果：生成批处理脚本中={1}"
                            , Path.GetFileName(m3u8File), Path.GetFileName(cmdPath)));

                        //{ffmpeg} -threads 1 -i {本地TS列表.m3u8} -c copy {文件名}.mkv
                        var cmd = txbMergeCMD.Text;
                        cmd = cmd.Replace("{ffmpeg}", ffmpegPath);
                        cmd = cmd.Replace("{本地TS列表.m3u8}", newM3U8File);
                        cmd = cmd.Replace("{文件名}", dirName);

                        File.WriteAllText(cmdPath, cmd, Encoding.Default);

                        WriteLog(TAG, string.Format("分析{0}文件结果：执行批处理脚本中={1}"
                            , Path.GetFileName(m3u8File), Path.GetFileName(cmdPath)));

                        //Process.Start(cmdPath);
                        //using (Process cmdProcess = new Process())//使用Using自动释放，导致Exited事件不出发
                        {
                            Process cmdProcess = new Process();
                            cmdProcess.StartInfo.UseShellExecute = false;
                            cmdProcess.StartInfo.FileName = cmdPath;
                            cmdProcess.StartInfo.CreateNoWindow = false;
                            cmdProcess.EnableRaisingEvents = true;
                            cmdProcess.Exited += (_sender, _e) =>
                            {
                                WriteLog(TAG, string.Format("-->目录“{0}”自动合并完毕，请确认", dirName));
                                (_sender as Process).Dispose();

                                //ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
                                //var videoPath = Path.Combine(Directory.GetParent(dir).FullName, dirName + ".mp4");
                                //psi.Arguments = "/e,/select," + videoPath;
                                //Process.Start(psi);
                            };
                            cmdProcess.Start();
                            WriteLog(TAG, string.Format("请等待批处理“{0}”执行...", Path.GetFileName(cmdPath)));
                        }
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
                WriteLog(TAG, "执行完毕");
                lock (SequenceLock)
                {
                    if (mDownDirAndTaskCountMap.ContainsKey(dir))
                        mDownDirAndTaskCountMap.Remove(dir);
                }
                //Thread.CurrentThread.Abort();
                Thread.CurrentThread.Join();
            }
        }

        private void vsbMergeCMD_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }

    /* 进程间使用无效，本进程会直接卡死。推测同一进程内可能有效
    /// <summary>
    /// 自定义传输数据的结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct STRSTRUCT
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string str;
    }
    */

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
