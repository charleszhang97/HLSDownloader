namespace HLS.Download.UI
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckbMergeTasks = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbbUserAgent = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDoIt = new System.Windows.Forms.Button();
            this.txbUrls = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbbAction = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txbCustomBandwidth = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rdbSelectBandWidthCustom = new System.Windows.Forms.RadioButton();
            this.rdbSelectBandWidthMin = new System.Windows.Forms.RadioButton();
            this.rdbSelectBandWidthMax = new System.Windows.Forms.RadioButton();
            this.txbLog = new System.Windows.Forms.TextBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem18 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txbMaxDownloadSpeed = new System.Windows.Forms.TextBox();
            this.txbMaxConcurrentDownloads = new System.Windows.Forms.TextBox();
            this.cbbConfigFileName = new System.Windows.Forms.ComboBox();
            this.txbAria2Args = new System.Windows.Forms.TextBox();
            this.txbAria2GlobalInfo = new System.Windows.Forms.TextBox();
            this.txbTotalTime = new System.Windows.Forms.TextBox();
            this.txbTotalData = new System.Windows.Forms.TextBox();
            this.picbClear = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.pbDownProgress = new System.Windows.Forms.ProgressBar();
            this.picbCirculate = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnApplyMaxSpeed = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStartAria2 = new System.Windows.Forms.Button();
            this.btnOpenAria2WebUI = new System.Windows.Forms.Button();
            this.btnOpenDownloadDir = new System.Windows.Forms.Button();
            this.btnSetDownloadLocation = new System.Windows.Forms.Button();
            this.btnPauseAll = new System.Windows.Forms.Button();
            this.btnUnPauseAll = new System.Windows.Forms.Button();
            this.btnDelSession = new System.Windows.Forms.Button();
            this.btnRmAllTasks = new System.Windows.Forms.Button();
            this.btnKillAllAria2 = new System.Windows.Forms.Button();
            this.btnLastTaskDir = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.llblDeleteMergedFolders = new System.Windows.Forms.LinkLabel();
            this.btnMerge = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txbMergeCMD = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.llblDeleteLocalLogs = new System.Windows.Forms.LinkLabel();
            this.notifyIconTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbClear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbCirculate)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ckbMergeTasks);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnDoIt);
            this.groupBox1.Controls.Add(this.txbUrls);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(16, 278);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(756, 258);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "下载控制台:";
            // 
            // ckbMergeTasks
            // 
            this.ckbMergeTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbMergeTasks.AutoSize = true;
            this.ckbMergeTasks.Location = new System.Drawing.Point(627, 108);
            this.ckbMergeTasks.Name = "ckbMergeTasks";
            this.ckbMergeTasks.Size = new System.Drawing.Size(119, 19);
            this.ckbMergeTasks.TabIndex = 42;
            this.ckbMergeTasks.Text = "合并为任务组";
            this.toolTip1.SetToolTip(this.ckbMergeTasks, "合并所有任务为一组（设置同一下载目录），建议批量HTTP(S)任务使用。\r\nM3U8/HLS任务不要使用，会造成文件覆盖、丢失。");
            this.ckbMergeTasks.UseVisualStyleBackColor = true;
            this.ckbMergeTasks.CheckedChanged += new System.EventHandler(this.ckbMergeTasks_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.cbbUserAgent);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(5, 61);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(745, 38);
            this.panel2.TabIndex = 28;
            // 
            // cbbUserAgent
            // 
            this.cbbUserAgent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbUserAgent.FormattingEnabled = true;
            this.cbbUserAgent.Items.AddRange(new object[] {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/87.0.4280.66 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:84.0) Gecko/20100101 Firefox/84.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/70.0.3538.102 Safari/537.36 Edge/18.19582",
            "Mozilla/5.0 (compatible, MSIE 11, Windows NT 6.3; Trident/7.0; rv:11.0) like Geck" +
                "o",
            "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:38.0) Gecko/20100101 Thunderbird/38.2.0 L" +
                "ightning/4.0.2",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80." +
                "0.3987.87 Safari/537.36",
            "Mozilla/5.0 (Android 9; Mobile; rv:68.0) Gecko/68.0 Firefox/68.0",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_1 like Mac OS X) AppleWebKit/605.1.15 (KHTM" +
                "L, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/605.1.15 (KHTML, like" +
                " Gecko) Version/11.1.2 Safari/605.1.15",
            "Mozilla/5.0 (X11; CrOS x86_64 13421.99.0) AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/86.0.4240.198 Safari/537.36"});
            this.cbbUserAgent.Location = new System.Drawing.Point(89, 8);
            this.cbbUserAgent.Margin = new System.Windows.Forms.Padding(4);
            this.cbbUserAgent.Name = "cbbUserAgent";
            this.cbbUserAgent.Size = new System.Drawing.Size(648, 23);
            this.cbbUserAgent.TabIndex = 30;
            this.cbbUserAgent.Text = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
    "Chrome/87.0.4280.66 Safari/537.36";
            this.toolTip1.SetToolTip(this.cbbUserAgent, "选择后对所有下载任务即时生效。自定义可以直接输入按Enter生效");
            this.cbbUserAgent.SelectedIndexChanged += new System.EventHandler(this.cbbUserAgent_SelectedIndexChanged);
            this.cbbUserAgent.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbbUserAgent_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 11);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 15);
            this.label4.TabIndex = 29;
            this.label4.Text = "UserAgent:";
            this.toolTip1.SetToolTip(this.label4, "Set user agent for HTTP(S) downloads. Default: aria2/$VERSION, $VERSION is replac" +
        "ed by package version.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 109);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(274, 15);
            this.label2.TabIndex = 32;
            this.label2.Text = "示例：ABC视频|http://a.cn/c/d.m3u8";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 109);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 15);
            this.label1.TabIndex = 31;
            this.label1.Text = "正确格式：文件名 竖线 网址/路径";
            // 
            // btnDoIt
            // 
            this.btnDoIt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoIt.Location = new System.Drawing.Point(694, 130);
            this.btnDoIt.Margin = new System.Windows.Forms.Padding(4);
            this.btnDoIt.Name = "btnDoIt";
            this.btnDoIt.Size = new System.Drawing.Size(57, 113);
            this.btnDoIt.TabIndex = 34;
            this.btnDoIt.Text = "下载";
            this.btnDoIt.UseVisualStyleBackColor = true;
            this.btnDoIt.Click += new System.EventHandler(this.btnDoIt_Click);
            // 
            // txbUrls
            // 
            this.txbUrls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbUrls.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbUrls.Location = new System.Drawing.Point(11, 130);
            this.txbUrls.Margin = new System.Windows.Forms.Padding(4);
            this.txbUrls.MaxLength = 0;
            this.txbUrls.Multiline = true;
            this.txbUrls.Name = "txbUrls";
            this.txbUrls.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbUrls.Size = new System.Drawing.Size(679, 113);
            this.txbUrls.TabIndex = 33;
            this.toolTip1.SetToolTip(this.txbUrls, "示例1：VideoName|https://website.com/E1okxWlJ/index.m3u8\r\n示例2：VideoName|D:\\MyFolder\\" +
        "index.m3u8");
            this.txbUrls.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txbUrls_MouseClick);
            this.txbUrls.TextChanged += new System.EventHandler(this.txbUrls_TextChanged);
            this.txbUrls.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_KeyDown);
            this.txbUrls.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txbUrls_MouseDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.cbbAction);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.txbCustomBandwidth);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.rdbSelectBandWidthCustom);
            this.panel1.Controls.Add(this.rdbSelectBandWidthMin);
            this.panel1.Controls.Add(this.rdbSelectBandWidthMax);
            this.panel1.Location = new System.Drawing.Point(5, 24);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(745, 38);
            this.panel1.TabIndex = 22;
            // 
            // cbbAction
            // 
            this.cbbAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbAction.FormattingEnabled = true;
            this.cbbAction.Items.AddRange(new object[] {
            "无操作",
            "注销",
            "锁定",
            "关机",
            "重新启动",
            "睡眠",
            "休眠",
            "关闭显示器",
            "关闭Aria2",
            "退出程序"});
            this.cbbAction.Location = new System.Drawing.Point(631, 8);
            this.cbbAction.Name = "cbbAction";
            this.cbbAction.Size = new System.Drawing.Size(105, 23);
            this.cbbAction.TabIndex = 44;
            this.cbbAction.Text = "无操作";
            this.toolTip1.SetToolTip(this.cbbAction, "下载完成时执行的操作");
            this.cbbAction.SelectedValueChanged += new System.EventHandler(this.cbbAction_SelectedValueChanged);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(551, 11);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 15);
            this.label11.TabIndex = 43;
            this.label11.Text = "下载完成后";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label11, "下载完成时执行的操作");
            // 
            // txbCustomBandwidth
            // 
            this.txbCustomBandwidth.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbCustomBandwidth.Enabled = false;
            this.txbCustomBandwidth.Location = new System.Drawing.Point(362, 8);
            this.txbCustomBandwidth.Margin = new System.Windows.Forms.Padding(4);
            this.txbCustomBandwidth.Name = "txbCustomBandwidth";
            this.txbCustomBandwidth.Size = new System.Drawing.Size(80, 25);
            this.txbCustomBandwidth.TabIndex = 27;
            this.txbCustomBandwidth.Text = "1000000";
            this.txbCustomBandwidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txbCustomBandwidth, "当只有一种码率时，此设置会被忽略");
            this.txbCustomBandwidth.WordWrap = false;
            this.txbCustomBandwidth.TextChanged += new System.EventHandler(this.txbCustomBandwidth_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 15);
            this.label3.TabIndex = 23;
            this.label3.Text = "多码率选择策略:";
            this.toolTip1.SetToolTip(this.label3, "当只有一种码率时，此设置会被忽略");
            // 
            // rdbSelectBandWidthCustom
            // 
            this.rdbSelectBandWidthCustom.AutoSize = true;
            this.rdbSelectBandWidthCustom.Location = new System.Drawing.Point(280, 9);
            this.rdbSelectBandWidthCustom.Margin = new System.Windows.Forms.Padding(4);
            this.rdbSelectBandWidthCustom.Name = "rdbSelectBandWidthCustom";
            this.rdbSelectBandWidthCustom.Size = new System.Drawing.Size(81, 19);
            this.rdbSelectBandWidthCustom.TabIndex = 26;
            this.rdbSelectBandWidthCustom.Text = "自定义:";
            this.toolTip1.SetToolTip(this.rdbSelectBandWidthCustom, "当只有一种码率时，此设置会被忽略");
            this.rdbSelectBandWidthCustom.UseVisualStyleBackColor = true;
            this.rdbSelectBandWidthCustom.CheckedChanged += new System.EventHandler(this.rdbSelectBandWidthCustom_CheckedChanged);
            // 
            // rdbSelectBandWidthMin
            // 
            this.rdbSelectBandWidthMin.AutoSize = true;
            this.rdbSelectBandWidthMin.Location = new System.Drawing.Point(209, 9);
            this.rdbSelectBandWidthMin.Margin = new System.Windows.Forms.Padding(4);
            this.rdbSelectBandWidthMin.Name = "rdbSelectBandWidthMin";
            this.rdbSelectBandWidthMin.Size = new System.Drawing.Size(58, 19);
            this.rdbSelectBandWidthMin.TabIndex = 25;
            this.rdbSelectBandWidthMin.Text = "最小";
            this.toolTip1.SetToolTip(this.rdbSelectBandWidthMin, "当只有一种码率时，此设置会被忽略");
            this.rdbSelectBandWidthMin.UseVisualStyleBackColor = true;
            this.rdbSelectBandWidthMin.CheckedChanged += new System.EventHandler(this.rdbSelectBandWidthMin_CheckedChanged);
            // 
            // rdbSelectBandWidthMax
            // 
            this.rdbSelectBandWidthMax.AutoSize = true;
            this.rdbSelectBandWidthMax.Checked = true;
            this.rdbSelectBandWidthMax.Location = new System.Drawing.Point(139, 9);
            this.rdbSelectBandWidthMax.Margin = new System.Windows.Forms.Padding(4);
            this.rdbSelectBandWidthMax.Name = "rdbSelectBandWidthMax";
            this.rdbSelectBandWidthMax.Size = new System.Drawing.Size(58, 19);
            this.rdbSelectBandWidthMax.TabIndex = 24;
            this.rdbSelectBandWidthMax.TabStop = true;
            this.rdbSelectBandWidthMax.Text = "最大";
            this.toolTip1.SetToolTip(this.rdbSelectBandWidthMax, "当只有一种码率时，此设置会被忽略");
            this.rdbSelectBandWidthMax.UseVisualStyleBackColor = true;
            this.rdbSelectBandWidthMax.CheckedChanged += new System.EventHandler(this.rdbSelectBandWidthMax_CheckedChanged);
            // 
            // txbLog
            // 
            this.txbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbLog.ContextMenuStrip = this.contextMenu;
            this.txbLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbLog.Location = new System.Drawing.Point(6, 17);
            this.txbLog.Margin = new System.Windows.Forms.Padding(2);
            this.txbLog.MaxLength = 0;
            this.txbLog.Multiline = true;
            this.txbLog.Name = "txbLog";
            this.txbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbLog.Size = new System.Drawing.Size(503, 785);
            this.txbLog.TabIndex = 40;
            this.txbLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_KeyDown);
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.ToolStripMenuItem4,
            this.toolStripMenuItem20,
            this.ToolStripMenuItem5,
            this.ToolStripMenuItem6,
            this.ToolStripMenuItem7,
            this.ToolStripMenuItem8,
            this.toolStripSeparator1,
            this.toolStripMenuItem19});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.ShowItemToolTips = false;
            this.contextMenu.Size = new System.Drawing.Size(255, 298);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.toolStripMenuItem1.Size = new System.Drawing.Size(254, 26);
            this.toolStripMenuItem1.Text = "启动Aria2";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.btnStartAria2_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.U)));
            this.toolStripMenuItem2.Size = new System.Drawing.Size(254, 26);
            this.toolStripMenuItem2.Text = "全部开始下载";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.btnUnPauseAll_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.toolStripMenuItem3.Size = new System.Drawing.Size(254, 26);
            this.toolStripMenuItem3.Text = "全部暂停下载";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.btnPauseAll_Click);
            // 
            // ToolStripMenuItem4
            // 
            this.ToolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.ToolStripMenuItem4.Name = "ToolStripMenuItem4";
            this.ToolStripMenuItem4.Size = new System.Drawing.Size(254, 26);
            this.ToolStripMenuItem4.Text = "应用自定义限速";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.AutoSize = false;
            this.toolStripComboBox1.AutoToolTip = true;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "0",
            "128",
            "256",
            "512",
            "1024",
            "2048",
            "4096",
            "8192",
            "10240"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(70, 28);
            this.toolStripComboBox1.Text = "0";
            this.toolStripComboBox1.ToolTipText = "单位：KB/S";
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox2});
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            this.toolStripMenuItem20.Size = new System.Drawing.Size(254, 26);
            this.toolStripMenuItem20.Text = "并行任务数限制";
            // 
            // toolStripComboBox2
            // 
            this.toolStripComboBox2.AutoSize = false;
            this.toolStripComboBox2.Items.AddRange(new object[] {
            "1",
            "5",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256",
            "512"});
            this.toolStripComboBox2.Name = "toolStripComboBox2";
            this.toolStripComboBox2.Size = new System.Drawing.Size(70, 28);
            this.toolStripComboBox2.Text = "5";
            this.toolStripComboBox2.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox2_SelectedIndexChanged);
            // 
            // ToolStripMenuItem5
            // 
            this.ToolStripMenuItem5.Name = "ToolStripMenuItem5";
            this.ToolStripMenuItem5.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Z)));
            this.ToolStripMenuItem5.Size = new System.Drawing.Size(254, 26);
            this.ToolStripMenuItem5.Text = "最近一次任务目录";
            this.ToolStripMenuItem5.Click += new System.EventHandler(this.btnLastTaskDir_Click);
            // 
            // ToolStripMenuItem6
            // 
            this.ToolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11,
            this.toolStripMenuItem12,
            this.toolStripMenuItem13,
            this.toolStripMenuItem14,
            this.toolStripMenuItem15,
            this.toolStripMenuItem16,
            this.toolStripMenuItem17,
            this.toolStripMenuItem18});
            this.ToolStripMenuItem6.Name = "ToolStripMenuItem6";
            this.ToolStripMenuItem6.Size = new System.Drawing.Size(254, 26);
            this.ToolStripMenuItem6.Text = "下载完成后";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem9.Text = "无操作";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem10.Text = "注销";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem11.Text = "锁定";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem12.Text = "关机";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem13.Text = "重新启动";
            this.toolStripMenuItem13.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem14.Text = "睡眠";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem15.Text = "休眠";
            this.toolStripMenuItem15.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem16.Text = "关闭显示器";
            this.toolStripMenuItem16.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            this.toolStripMenuItem17.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem17.Text = "关闭Aria2";
            this.toolStripMenuItem17.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            this.toolStripMenuItem18.Size = new System.Drawing.Size(159, 26);
            this.toolStripMenuItem18.Text = "退出程序";
            this.toolStripMenuItem18.Click += new System.EventHandler(this.ActionAfterDownloadCompleted_MenuItemClick);
            // 
            // ToolStripMenuItem7
            // 
            this.ToolStripMenuItem7.Name = "ToolStripMenuItem7";
            this.ToolStripMenuItem7.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.ToolStripMenuItem7.Size = new System.Drawing.Size(254, 26);
            this.ToolStripMenuItem7.Text = "取消剩余下载";
            this.ToolStripMenuItem7.Click += new System.EventHandler(this.btnRmAllTasks_Click);
            // 
            // ToolStripMenuItem8
            // 
            this.ToolStripMenuItem8.Name = "ToolStripMenuItem8";
            this.ToolStripMenuItem8.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.ToolStripMenuItem8.Size = new System.Drawing.Size(254, 26);
            this.ToolStripMenuItem8.Text = "关闭Aria2";
            this.ToolStripMenuItem8.Click += new System.EventHandler(this.btnKillAllAria2_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(251, 6);
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
            this.toolStripMenuItem19.Size = new System.Drawing.Size(254, 26);
            this.toolStripMenuItem19.Text = "隐藏窗体";
            this.toolStripMenuItem19.Click += new System.EventHandler(this.ToggleWindowState);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txbMaxDownloadSpeed);
            this.groupBox2.Controls.Add(this.txbMaxConcurrentDownloads);
            this.groupBox2.Controls.Add(this.cbbConfigFileName);
            this.groupBox2.Controls.Add(this.txbAria2Args);
            this.groupBox2.Controls.Add(this.txbAria2GlobalInfo);
            this.groupBox2.Controls.Add(this.txbTotalTime);
            this.groupBox2.Controls.Add(this.txbTotalData);
            this.groupBox2.Controls.Add(this.picbClear);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.pbDownProgress);
            this.groupBox2.Controls.Add(this.picbCirculate);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.btnApplyMaxSpeed);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.flowLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(16, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(756, 254);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Aria2控制台:";
            // 
            // txbMaxDownloadSpeed
            // 
            this.txbMaxDownloadSpeed.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbMaxDownloadSpeed.Location = new System.Drawing.Point(97, 218);
            this.txbMaxDownloadSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.txbMaxDownloadSpeed.Name = "txbMaxDownloadSpeed";
            this.txbMaxDownloadSpeed.Size = new System.Drawing.Size(61, 25);
            this.txbMaxDownloadSpeed.TabIndex = 16;
            this.txbMaxDownloadSpeed.Text = "1024";
            this.txbMaxDownloadSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txbMaxDownloadSpeed, "设置为0时，不限制下载速度（默认）。点击“应用”才会生效");
            this.txbMaxDownloadSpeed.WordWrap = false;
            // 
            // txbMaxConcurrentDownloads
            // 
            this.txbMaxConcurrentDownloads.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbMaxConcurrentDownloads.Location = new System.Drawing.Point(277, 218);
            this.txbMaxConcurrentDownloads.Margin = new System.Windows.Forms.Padding(4);
            this.txbMaxConcurrentDownloads.Name = "txbMaxConcurrentDownloads";
            this.txbMaxConcurrentDownloads.Size = new System.Drawing.Size(32, 25);
            this.txbMaxConcurrentDownloads.TabIndex = 41;
            this.txbMaxConcurrentDownloads.Text = "5";
            this.txbMaxConcurrentDownloads.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txbMaxConcurrentDownloads, "设置最大下载并行任务数，默认值由conf文件决定。输入整数按Enter生效");
            this.txbMaxConcurrentDownloads.WordWrap = false;
            this.txbMaxConcurrentDownloads.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbMaxConcurrentDownloads_KeyPress);
            // 
            // cbbConfigFileName
            // 
            this.cbbConfigFileName.FormattingEnabled = true;
            this.cbbConfigFileName.Items.AddRange(new object[] {
            "aria2c.conf"});
            this.cbbConfigFileName.Location = new System.Drawing.Point(121, 27);
            this.cbbConfigFileName.Margin = new System.Windows.Forms.Padding(4);
            this.cbbConfigFileName.Name = "cbbConfigFileName";
            this.cbbConfigFileName.Size = new System.Drawing.Size(234, 23);
            this.cbbConfigFileName.TabIndex = 4;
            this.toolTip1.SetToolTip(this.cbbConfigFileName, "请在启动Aria2前选择（启动后选择无效），控制台文本如果可用，则自动变化");
            this.cbbConfigFileName.SelectedIndexChanged += new System.EventHandler(this.cbbConfigFileName_SelectedIndexChanged);
            // 
            // txbAria2Args
            // 
            this.txbAria2Args.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbAria2Args.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbAria2Args.Location = new System.Drawing.Point(480, 25);
            this.txbAria2Args.Margin = new System.Windows.Forms.Padding(4);
            this.txbAria2Args.Name = "txbAria2Args";
            this.txbAria2Args.Size = new System.Drawing.Size(240, 25);
            this.txbAria2Args.TabIndex = 18;
            this.toolTip1.SetToolTip(this.txbAria2Args, "对Aria2命令行追加必要参数（启动Aria2前输入）");
            this.txbAria2Args.WordWrap = false;
            this.txbAria2Args.DoubleClick += new System.EventHandler(this.txbAria2Args_DoubleClick);
            this.txbAria2Args.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_KeyDown);
            this.txbAria2Args.Leave += new System.EventHandler(this.txbAria2Args_Leave);
            // 
            // txbAria2GlobalInfo
            // 
            this.txbAria2GlobalInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbAria2GlobalInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txbAria2GlobalInfo.ContextMenuStrip = this.contextMenu;
            this.txbAria2GlobalInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbAria2GlobalInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txbAria2GlobalInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txbAria2GlobalInfo.Location = new System.Drawing.Point(10, 182);
            this.txbAria2GlobalInfo.Margin = new System.Windows.Forms.Padding(4);
            this.txbAria2GlobalInfo.Name = "txbAria2GlobalInfo";
            this.txbAria2GlobalInfo.ReadOnly = true;
            this.txbAria2GlobalInfo.Size = new System.Drawing.Size(736, 24);
            this.txbAria2GlobalInfo.TabIndex = 0;
            this.txbAria2GlobalInfo.TabStop = false;
            this.txbAria2GlobalInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txbTotalTime
            // 
            this.txbTotalTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txbTotalTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txbTotalTime.ContextMenuStrip = this.contextMenu;
            this.txbTotalTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbTotalTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txbTotalTime.Location = new System.Drawing.Point(651, 217);
            this.txbTotalTime.Margin = new System.Windows.Forms.Padding(4);
            this.txbTotalTime.Name = "txbTotalTime";
            this.txbTotalTime.ReadOnly = true;
            this.txbTotalTime.Size = new System.Drawing.Size(95, 24);
            this.txbTotalTime.TabIndex = 0;
            this.txbTotalTime.TabStop = false;
            this.txbTotalTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txbTotalTime, "当前任务下载持续时间");
            // 
            // txbTotalData
            // 
            this.txbTotalData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txbTotalData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txbTotalData.ContextMenuStrip = this.contextMenu;
            this.txbTotalData.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbTotalData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txbTotalData.Location = new System.Drawing.Point(446, 217);
            this.txbTotalData.Margin = new System.Windows.Forms.Padding(4);
            this.txbTotalData.Name = "txbTotalData";
            this.txbTotalData.ReadOnly = true;
            this.txbTotalData.Size = new System.Drawing.Size(95, 24);
            this.txbTotalData.TabIndex = 0;
            this.txbTotalData.TabStop = false;
            this.txbTotalData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txbTotalData, "当前任务已下载的流量");
            // 
            // picbClear
            // 
            this.picbClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picbClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picbClear.Image = global::HLS.Download.UI.Properties.Resources.clear;
            this.picbClear.Location = new System.Drawing.Point(727, 27);
            this.picbClear.Name = "picbClear";
            this.picbClear.Size = new System.Drawing.Size(20, 20);
            this.picbClear.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbClear.TabIndex = 43;
            this.picbClear.TabStop = false;
            this.toolTip1.SetToolTip(this.picbClear, "清除参数追加文本内容");
            this.picbClear.Click += new System.EventHandler(this.picbClear_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(262, 221);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(22, 15);
            this.label13.TabIndex = 42;
            this.label13.Text = "↓";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbDownProgress
            // 
            this.pbDownProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbDownProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pbDownProgress.Location = new System.Drawing.Point(10, 206);
            this.pbDownProgress.Name = "pbDownProgress";
            this.pbDownProgress.Size = new System.Drawing.Size(736, 2);
            this.pbDownProgress.Step = 1;
            this.pbDownProgress.TabIndex = 23;
            // 
            // picbCirculate
            // 
            this.picbCirculate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picbCirculate.Image = ((System.Drawing.Image)(resources.GetObject("picbCirculate.Image")));
            this.picbCirculate.Location = new System.Drawing.Point(365, 28);
            this.picbCirculate.Name = "picbCirculate";
            this.picbCirculate.Size = new System.Drawing.Size(32, 20);
            this.picbCirculate.TabIndex = 22;
            this.picbCirculate.TabStop = false;
            this.toolTip1.SetToolTip(this.picbCirculate, "循环选择配置文件");
            this.picbCirculate.Click += new System.EventHandler(this.picbCirculate_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(406, 32);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 15);
            this.label12.TabIndex = 17;
            this.label12.Text = "参数追加：";
            this.toolTip1.SetToolTip(this.label12, "对Aria2命令行追加必要参数（启动Aria2前输入）");
            // 
            // btnApplyMaxSpeed
            // 
            this.btnApplyMaxSpeed.Location = new System.Drawing.Point(195, 216);
            this.btnApplyMaxSpeed.Margin = new System.Windows.Forms.Padding(4);
            this.btnApplyMaxSpeed.Name = "btnApplyMaxSpeed";
            this.btnApplyMaxSpeed.Size = new System.Drawing.Size(52, 26);
            this.btnApplyMaxSpeed.TabIndex = 18;
            this.btnApplyMaxSpeed.Text = "应用";
            this.toolTip1.SetToolTip(this.btnApplyMaxSpeed, "设置为0时，不限制下载速度（默认）。点击“应用”才会生效");
            this.btnApplyMaxSpeed.UseVisualStyleBackColor = true;
            this.btnApplyMaxSpeed.Click += new System.EventHandler(this.btnApplyMaxSpeed_Click);
            this.btnApplyMaxSpeed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnApplyMaxSpeed_MouseDown);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(340, 221);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(112, 15);
            this.label10.TabIndex = 19;
            this.label10.Text = "下载流量统计：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 32);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(122, 15);
            this.label9.TabIndex = 3;
            this.label9.Text = "Aria2配置文件：";
            this.toolTip1.SetToolTip(this.label9, "请在启动Aria2前选择（启动后选择无效），控制台文本如果可用，则自动变化");
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(545, 222);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 15);
            this.label8.TabIndex = 20;
            this.label8.Text = "下载时间统计：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(159, 221);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 15);
            this.label7.TabIndex = 17;
            this.label7.Text = "KB/s";
            this.toolTip1.SetToolTip(this.label7, "设置为0时，不限制下载速度（默认）。点击“应用”才会生效");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 221);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "自定义限速：";
            this.toolTip1.SetToolTip(this.label6, "设置为0时，不限制下载速度（默认）。点击“应用”才会生效");
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.btnStartAria2);
            this.flowLayoutPanel1.Controls.Add(this.btnOpenAria2WebUI);
            this.flowLayoutPanel1.Controls.Add(this.btnOpenDownloadDir);
            this.flowLayoutPanel1.Controls.Add(this.btnSetDownloadLocation);
            this.flowLayoutPanel1.Controls.Add(this.btnPauseAll);
            this.flowLayoutPanel1.Controls.Add(this.btnUnPauseAll);
            this.flowLayoutPanel1.Controls.Add(this.btnDelSession);
            this.flowLayoutPanel1.Controls.Add(this.btnRmAllTasks);
            this.flowLayoutPanel1.Controls.Add(this.btnKillAllAria2);
            this.flowLayoutPanel1.Controls.Add(this.btnLastTaskDir);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 61);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(15, 2, 2, 2);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(752, 110);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // btnStartAria2
            // 
            this.btnStartAria2.Location = new System.Drawing.Point(19, 6);
            this.btnStartAria2.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartAria2.Name = "btnStartAria2";
            this.btnStartAria2.Size = new System.Drawing.Size(136, 45);
            this.btnStartAria2.TabIndex = 6;
            this.btnStartAria2.Text = "启动Aria2";
            this.toolTip1.SetToolTip(this.btnStartAria2, "使用当前配置文件和参数启动Aria2");
            this.btnStartAria2.UseVisualStyleBackColor = true;
            this.btnStartAria2.Click += new System.EventHandler(this.btnStartAria2_Click);
            // 
            // btnOpenAria2WebUI
            // 
            this.btnOpenAria2WebUI.Location = new System.Drawing.Point(163, 6);
            this.btnOpenAria2WebUI.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenAria2WebUI.Name = "btnOpenAria2WebUI";
            this.btnOpenAria2WebUI.Size = new System.Drawing.Size(136, 45);
            this.btnOpenAria2WebUI.TabIndex = 7;
            this.btnOpenAria2WebUI.Text = "打开WebUI";
            this.toolTip1.SetToolTip(this.btnOpenAria2WebUI, "打开WebUI，显示所有下载任务的详细信息");
            this.btnOpenAria2WebUI.UseVisualStyleBackColor = true;
            this.btnOpenAria2WebUI.Click += new System.EventHandler(this.btnOpenAria2WebUI_Click);
            // 
            // btnOpenDownloadDir
            // 
            this.btnOpenDownloadDir.Location = new System.Drawing.Point(307, 6);
            this.btnOpenDownloadDir.Margin = new System.Windows.Forms.Padding(4);
            this.btnOpenDownloadDir.Name = "btnOpenDownloadDir";
            this.btnOpenDownloadDir.Size = new System.Drawing.Size(136, 45);
            this.btnOpenDownloadDir.TabIndex = 8;
            this.btnOpenDownloadDir.Text = "打开下载目录";
            this.btnOpenDownloadDir.UseVisualStyleBackColor = true;
            this.btnOpenDownloadDir.Click += new System.EventHandler(this.btnOpenDownloadDir_Click);
            // 
            // btnSetDownloadLocation
            // 
            this.btnSetDownloadLocation.Location = new System.Drawing.Point(451, 6);
            this.btnSetDownloadLocation.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetDownloadLocation.Name = "btnSetDownloadLocation";
            this.btnSetDownloadLocation.Size = new System.Drawing.Size(136, 45);
            this.btnSetDownloadLocation.TabIndex = 9;
            this.btnSetDownloadLocation.Text = "设置下载目录";
            this.btnSetDownloadLocation.UseVisualStyleBackColor = true;
            this.btnSetDownloadLocation.Click += new System.EventHandler(this.btnSetDownloadLocation_Click);
            // 
            // btnPauseAll
            // 
            this.btnPauseAll.Location = new System.Drawing.Point(595, 6);
            this.btnPauseAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnPauseAll.Name = "btnPauseAll";
            this.btnPauseAll.Size = new System.Drawing.Size(136, 45);
            this.btnPauseAll.TabIndex = 10;
            this.btnPauseAll.Text = "全部暂停下载";
            this.toolTip1.SetToolTip(this.btnPauseAll, "暂停当前所有的下载任务");
            this.btnPauseAll.UseVisualStyleBackColor = true;
            this.btnPauseAll.Click += new System.EventHandler(this.btnPauseAll_Click);
            // 
            // btnUnPauseAll
            // 
            this.btnUnPauseAll.Location = new System.Drawing.Point(19, 59);
            this.btnUnPauseAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnUnPauseAll.Name = "btnUnPauseAll";
            this.btnUnPauseAll.Size = new System.Drawing.Size(136, 45);
            this.btnUnPauseAll.TabIndex = 11;
            this.btnUnPauseAll.Text = "全部开始下载";
            this.toolTip1.SetToolTip(this.btnUnPauseAll, "继续下载之前未完成的任务");
            this.btnUnPauseAll.UseVisualStyleBackColor = true;
            this.btnUnPauseAll.Click += new System.EventHandler(this.btnUnPauseAll_Click);
            // 
            // btnDelSession
            // 
            this.btnDelSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelSession.Location = new System.Drawing.Point(163, 59);
            this.btnDelSession.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelSession.Name = "btnDelSession";
            this.btnDelSession.Size = new System.Drawing.Size(136, 45);
            this.btnDelSession.TabIndex = 12;
            this.btnDelSession.Text = "清空下载记录";
            this.toolTip1.SetToolTip(this.btnDelSession, "清除aria2c.session文件内容");
            this.btnDelSession.UseVisualStyleBackColor = true;
            this.btnDelSession.Click += new System.EventHandler(this.btnDelSession_Click);
            // 
            // btnRmAllTasks
            // 
            this.btnRmAllTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRmAllTasks.Location = new System.Drawing.Point(307, 59);
            this.btnRmAllTasks.Margin = new System.Windows.Forms.Padding(4);
            this.btnRmAllTasks.Name = "btnRmAllTasks";
            this.btnRmAllTasks.Size = new System.Drawing.Size(136, 45);
            this.btnRmAllTasks.TabIndex = 13;
            this.btnRmAllTasks.Text = "取消剩余下载";
            this.toolTip1.SetToolTip(this.btnRmAllTasks, "移除剩余所有的下载任务，结束下载");
            this.btnRmAllTasks.UseVisualStyleBackColor = true;
            this.btnRmAllTasks.Click += new System.EventHandler(this.btnRmAllTasks_Click);
            // 
            // btnKillAllAria2
            // 
            this.btnKillAllAria2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKillAllAria2.Location = new System.Drawing.Point(451, 59);
            this.btnKillAllAria2.Margin = new System.Windows.Forms.Padding(4);
            this.btnKillAllAria2.Name = "btnKillAllAria2";
            this.btnKillAllAria2.Size = new System.Drawing.Size(136, 45);
            this.btnKillAllAria2.TabIndex = 14;
            this.btnKillAllAria2.Text = "关闭Aria2";
            this.toolTip1.SetToolTip(this.btnKillAllAria2, "关闭Aria2及其进程，尝试杀掉所有残留进程");
            this.btnKillAllAria2.UseVisualStyleBackColor = true;
            this.btnKillAllAria2.Click += new System.EventHandler(this.btnKillAllAria2_Click);
            // 
            // btnLastTaskDir
            // 
            this.btnLastTaskDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLastTaskDir.Location = new System.Drawing.Point(595, 59);
            this.btnLastTaskDir.Margin = new System.Windows.Forms.Padding(4);
            this.btnLastTaskDir.Name = "btnLastTaskDir";
            this.btnLastTaskDir.Size = new System.Drawing.Size(136, 45);
            this.btnLastTaskDir.TabIndex = 41;
            this.btnLastTaskDir.Text = "打开最近一次\r\n任务目录";
            this.toolTip1.SetToolTip(this.btnLastTaskDir, "打开最后一次任务的下载目录");
            this.btnLastTaskDir.UseVisualStyleBackColor = true;
            this.btnLastTaskDir.Click += new System.EventHandler(this.btnLastTaskDir_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.llblDeleteMergedFolders);
            this.groupBox3.Controls.Add(this.btnMerge);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txbMergeCMD);
            this.groupBox3.Location = new System.Drawing.Point(16, 545);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(756, 280);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "合并控制台:";
            // 
            // llblDeleteMergedFolders
            // 
            this.llblDeleteMergedFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblDeleteMergedFolders.AutoSize = true;
            this.llblDeleteMergedFolders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.llblDeleteMergedFolders.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.llblDeleteMergedFolders.LinkColor = System.Drawing.SystemColors.ControlDarkDark;
            this.llblDeleteMergedFolders.Location = new System.Drawing.Point(635, 22);
            this.llblDeleteMergedFolders.Name = "llblDeleteMergedFolders";
            this.llblDeleteMergedFolders.Size = new System.Drawing.Size(112, 15);
            this.llblDeleteMergedFolders.TabIndex = 45;
            this.llblDeleteMergedFolders.TabStop = true;
            this.llblDeleteMergedFolders.Text = "删除已合并目录";
            this.llblDeleteMergedFolders.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.llblDeleteMergedFolders, "删除下载目录中已经合并过的临时目录及文件。谨慎操作");
            this.llblDeleteMergedFolders.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblDeleteMergedFolders_LinkClicked);
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMerge.Location = new System.Drawing.Point(694, 124);
            this.btnMerge.Margin = new System.Windows.Forms.Padding(4);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(57, 144);
            this.btnMerge.TabIndex = 38;
            this.btnMerge.Text = "手工合并";
            this.toolTip1.SetToolTip(this.btnMerge, "对下载目录内的子目录分别进行合并操作。\r\n子目录内需要有M3U8清单、切片、Key等文件。");
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(8, 21);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(523, 68);
            this.label5.TabIndex = 36;
            this.label5.Text = "{ffmpeg}  会自动替换为ffmpeg.exe所在的路径\r\n{本地TS列表.m3u8}  会自动替换为切片列表文件的本地路径\r\n{文件名}  会自动替换为下" +
    "载前设置的“文件名”\r\n{AppPath}  会自动替换为程序所在目录，如“D:\\HLSDownloader”";
            // 
            // txbMergeCMD
            // 
            this.txbMergeCMD.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbMergeCMD.Cursor = System.Windows.Forms.Cursors.Default;
            this.txbMergeCMD.Location = new System.Drawing.Point(11, 124);
            this.txbMergeCMD.Margin = new System.Windows.Forms.Padding(4);
            this.txbMergeCMD.MaxLength = 0;
            this.txbMergeCMD.Multiline = true;
            this.txbMergeCMD.Name = "txbMergeCMD";
            this.txbMergeCMD.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbMergeCMD.Size = new System.Drawing.Size(679, 144);
            this.txbMergeCMD.TabIndex = 37;
            this.txbMergeCMD.Text = resources.GetString("txbMergeCMD.Text");
            this.txbMergeCMD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txb_KeyDown);
            this.txbMergeCMD.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txbMergeCMD_MouseDoubleClick);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txbLog);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(20, 16);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(513, 808);
            this.groupBox4.TabIndex = 39;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "日志信息:";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "选择文件夹作为Aria2的下载目录";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 800;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.ContextMenuStrip = this.contextMenu;
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1MinSize = 770;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.ContextMenuStrip = this.contextMenu;
            this.splitContainer1.Panel2.Controls.Add(this.llblDeleteLocalLogs);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(20, 16, 14, 14);
            this.splitContainer1.Panel2MinSize = 300;
            this.splitContainer1.Size = new System.Drawing.Size(1348, 840);
            this.splitContainer1.SplitterDistance = 796;
            this.splitContainer1.SplitterIncrement = 30;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.splitContainer1, "调整左右两侧面板宽度");
            // 
            // llblDeleteLocalLogs
            // 
            this.llblDeleteLocalLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llblDeleteLocalLogs.AutoSize = true;
            this.llblDeleteLocalLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.llblDeleteLocalLogs.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.llblDeleteLocalLogs.LinkColor = System.Drawing.SystemColors.ControlDarkDark;
            this.llblDeleteLocalLogs.Location = new System.Drawing.Point(441, 5);
            this.llblDeleteLocalLogs.Name = "llblDeleteLocalLogs";
            this.llblDeleteLocalLogs.Size = new System.Drawing.Size(97, 15);
            this.llblDeleteLocalLogs.TabIndex = 46;
            this.llblDeleteLocalLogs.TabStop = true;
            this.llblDeleteLocalLogs.Text = "删除本地日志";
            this.llblDeleteLocalLogs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.llblDeleteLocalLogs, "删除日志目录中已经超过一周的日志文件。谨慎操作");
            this.llblDeleteLocalLogs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblDeleteLocalLogs_LinkClicked);
            // 
            // notifyIconTray
            // 
            this.notifyIconTray.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIconTray.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconTray.Icon")));
            this.notifyIconTray.Text = "Ye.M3U8.Downloader";
            this.notifyIconTray.Visible = true;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1348, 840);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1130, 768);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HLS下载器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.VisibleChanged += new System.EventHandler(this.MainForm_VisibleChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenu.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbClear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbCirculate)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDoIt;
        private System.Windows.Forms.TextBox txbUrls;
        private System.Windows.Forms.TextBox txbLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOpenAria2WebUI;
        private System.Windows.Forms.Button btnStartAria2;
        private System.Windows.Forms.Button btnKillAllAria2;
        private System.Windows.Forms.Button btnOpenDownloadDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.TextBox txbMergeCMD;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSetDownloadLocation;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txbCustomBandwidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdbSelectBandWidthCustom;
        private System.Windows.Forms.RadioButton rdbSelectBandWidthMin;
        private System.Windows.Forms.RadioButton rdbSelectBandWidthMax;
        private System.Windows.Forms.Button btnUnPauseAll;
        private System.Windows.Forms.Button btnPauseAll;
        private System.Windows.Forms.TextBox txbAria2GlobalInfo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cbbUserAgent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDelSession;
        private System.Windows.Forms.Button btnRmAllTasks;
        private System.Windows.Forms.TextBox txbTotalTime;
        private System.Windows.Forms.Button btnApplyMaxSpeed;
        private System.Windows.Forms.TextBox txbMaxDownloadSpeed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbbConfigFileName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txbTotalData;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NotifyIcon notifyIconTray;
        private System.Windows.Forms.PictureBox picbCirculate;
        private System.Windows.Forms.Button btnLastTaskDir;
        private System.Windows.Forms.CheckBox ckbMergeTasks;
        private System.Windows.Forms.ComboBox cbbAction;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.LinkLabel llblDeleteMergedFolders;
        private System.Windows.Forms.TextBox txbAria2Args;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ProgressBar pbDownProgress;
        private System.Windows.Forms.TextBox txbMaxConcurrentDownloads;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.LinkLabel llblDeleteLocalLogs;
        private System.Windows.Forms.PictureBox picbClear;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem16;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem17;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem18;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem20;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem19;
    }
}

