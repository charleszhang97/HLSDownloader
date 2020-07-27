namespace m3u8dl.service
{
    partial class M3u8dlServiceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M3u8dlServiceForm));
            this.txbLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txbLog
            // 
            this.txbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbLog.Location = new System.Drawing.Point(0, 0);
            this.txbLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txbLog.Multiline = true;
            this.txbLog.Name = "txbLog";
            this.txbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbLog.Size = new System.Drawing.Size(340, 455);
            this.txbLog.TabIndex = 0;
            this.txbLog.TextChanged += new System.EventHandler(this.txbLog_TextChanged);
            // 
            // M3u8dlServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 455);
            this.Controls.Add(this.txbLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "M3u8dlServiceForm";
            this.ShowInTaskbar = false;
            this.Text = "URL Protocol服务程序";
            this.Load += new System.EventHandler(this.M3u8dlServiceForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbLog;
    }
}

