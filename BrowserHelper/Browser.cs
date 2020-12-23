using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BrowserHelper
{
    public class Browser
    {
        /// <summary>
        /// 调用系统浏览器打开网页
        /// </summary>
        /// <param name="url">打开网页的链接</param>
        public bool OpenChrome(string url)
        {
            RegistryKey appPath = null;
            try
            {
                // 64位注册表路径
                var openKey = @"SOFTWARE\Wow6432Node\Google\Chrome";
                if (IntPtr.Size == 4)
                {
                    // 32位注册表路径
                    openKey = @"SOFTWARE\Google\Chrome";
                }
                appPath = Registry.LocalMachine.OpenSubKey(openKey);
                // 谷歌卸载了，注册表还没有清空，程序会返回一个"系统找不到指定的文件。"的bug
                if (appPath != null)
                {
                    //MessageBox.Show(ExePath("chrome.exe"));
                    string exePath = ExePath("chrome.exe");
                    var result = Process.Start(exePath, url);
                    if (result == null)
                    {
                        return false;
                    }
                }
                else
                {
                    var result = Process.Start("chrome.exe", url);
                    if (result == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                appPath?.Dispose();
            }
        }

        /// <summary>
        /// 用IE打开浏览器
        /// </summary>
        /// <param name="url"></param>
        public bool OpenIE(string url)
        {
            try
            {
                Process.Start("iexplore.exe", url);
                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                // IE浏览器路径安装：C:\Program Files\Internet Explorer
                // at System.Diagnostics.process.StartWithshellExecuteEx(ProcessStartInfo startInfo)注意这个错误
                try
                {
                    var programFilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    if (File.Exists(programFilesFolder + @"\Internet Explorer\iexplore.exe"))
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = programFilesFolder + @"\Internet Explorer\iexplore.exe",
                            Arguments = url,
                            UseShellExecute = false,
                            CreateNoWindow = false
                        };
                        Process.Start(processStartInfo);
                    }
                    else
                    {
                        programFilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                        if (File.Exists(programFilesFolder + @"\Internet Explorer\iexplore.exe"))
                        {
                            ProcessStartInfo processStartInfo = new ProcessStartInfo
                            {
                                FileName = programFilesFolder + @"\Internet Explorer\iexplore.exe",
                                Arguments = url,
                                UseShellExecute = false,
                                CreateNoWindow = false
                            };
                            Process.Start(processStartInfo);                            
                        }
                        else
                        {
                            if (MessageBox.Show(@"系统未安装IE浏览器，是否下载安装？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // 打开下载链接，从微软官网下载
                                OpenDefaultBrowser("http://windows.microsoft.com/zh-cn/internet-explorer/download-ie");
                            }
                            return false;
                        }
                    }
                    return true;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 打开系统默认浏览器（用户自己设置了默认浏览器）
        /// </summary>
        /// <param name="url"></param>
        public bool OpenDefaultBrowser(string url)
        {
            RegistryKey key = null;
            try
            {
                // 方法1
                //从注册表中读取默认浏览器可执行文件路径
                key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                if (key != null)
                {
                    string s = key.GetValue("").ToString();
                    //s就是你的默认浏览器，不过后面带了参数，把它截去，不过需要注意的是：不同的浏览器后面的参数不一样！
                    //"D:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"
                    var lastIndex = s.IndexOf(".exe", StringComparison.Ordinal);
                    if (lastIndex == -1)
                    {
                        lastIndex = s.IndexOf(".EXE", StringComparison.Ordinal);
                    }
                    var path = s.Substring(1, lastIndex + 3);
                    var result = Process.Start(path, url);
                    if (result == null)
                    {
                        // 方法2
                        // 调用系统默认的浏览器 
                        var result1 = Process.Start("explorer.exe", url);
                        if (result1 == null)
                        {
                            // 方法3
                            Process.Start(url);
                        }
                    }
                }
                else
                {
                    // 方法2
                    // 调用系统默认的浏览器 
                    var result1 = Process.Start("explorer.exe", url);
                    if (result1 == null)
                    {
                        // 方法3
                        Process.Start(url);
                    }
                }
                return true;
            }
            catch (Exception error)
            {
                MessageBox.Show("无法使用默认浏览器：\n" + error.Message);
                return false;
            }
            finally
            {
                key?.Dispose();
            }
        }

        /// <summary>
        /// 火狐浏览器打开网页
        /// </summary>
        /// <param name="url"></param>
        public bool OpenFireFox(string url)
        {
            RegistryKey appPath = null;
            try
            {
                // 64位注册表路径
                var openKey = @"SOFTWARE\Wow6432Node\Mozilla\Mozilla Firefox";
                if (IntPtr.Size == 4)
                {
                    // 32位注册表路径
                    openKey = @"SOFTWARE\Mozilla\Mozilla Firefox";
                }
                appPath = Registry.LocalMachine.OpenSubKey(openKey);
                if (appPath != null)
                {
                    //MessageBox.Show(ExePath("firefox.exe"));
                    string exePath = ExePath("firefox.exe");
                    var result = Process.Start(exePath, url);
                    if (result == null)
                    {
                        return false;
                    }
                }
                else
                {
                    var result = Process.Start("firefox.exe", url);
                    if (result == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                appPath?.Dispose();
            }
        }

        private string ExePath(string exeFullName)
        {
            try
            {
                return Microsoft.Win32
                        .Registry
                        .LocalMachine
                        .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + exeFullName, false)
                        .GetValue("")
                        .ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}