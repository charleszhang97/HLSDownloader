#region 类信息
/*----------------------------------------------------------------
 * 模块名：Aria2cWarpper
 * 创建者：isRight
 * 修改者列表：
 * 创建日期：2016.12.29
 * 模块描述：aria2c接口封装
 *----------------------------------------------------------------*/
#endregion

namespace FlyVR.Aria2
{
    using CookComputing.XmlRpc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal sealed class Aria2cWarpper
    {
        private static Aria2cProxy aria2c = XmlRpcProxyGen.Create<Aria2cProxy>();
        private static string _token;

        #region 属性
        /// <summary>
        /// Aria2cHost服务器地址
        /// </summary>
        public static string Aria2cHost
        {
            get
            {
                return aria2c.Url;
            }
            set
            {
                aria2c.Url = value;
            }
        }

        /// <summary>
        /// 下载目录
        /// </summary>
        public static string DownLoadDirectory
        {
            get
            {
                XmlRpcStruct option = aria2c.GetGlobalOption(Aria2cRpcSecret);
                string dir = Aria2cTools.GetRpcStructValue(option, "dir") as string;
                return dir;
            }

            set
            {
                XmlRpcStruct option = new XmlRpcStruct();
                option.Add("dir", value);
                aria2c.ChangeGlobalOption(Aria2cRpcSecret, option);
            }
        }

        /// <summary>
        /// Aria2c密码令牌
        /// </summary>
        public static string Aria2cRpcSecret
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_token) ? _token : "token:" + aria2c.Headers.Get("token");
            }
            set
            {
                _token = "token:" + value;
                if (!aria2c.Headers.AllKeys.Contains("token"))
                    aria2c.Headers.Add("token", value);
                else
                    aria2c.Headers.Set("token", value);
            }
        }
        #endregion

        #region Aria2cRPC接口封装

        /// <summary>
        /// 添加下载任务
        /// </summary>
        /// <param name="uri">下载地址</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="dir">下载文件夹</param>
        /// <returns>成功返回任务标识符，失败返回空</returns>
        public static string AddUri(string uri, string fileName = "", string dir = "", string userAgent = null)
        {
            string[] uris = new string[] { uri };
            string gid;
            XmlRpcStruct option = new XmlRpcStruct();

            if (dir != "")
            {
                option.Add("dir", dir);
            }

            if (fileName != "")
            {
                option.Add("out", fileName);
            }

            if (!string.IsNullOrWhiteSpace(userAgent))
                option.Add("user-agent", userAgent);

            if (option.Count > 0)
            {
                gid = aria2c.AddUri(Aria2cRpcSecret, uris, option);
            }
            else
            {
                gid = aria2c.AddUri(Aria2cRpcSecret, uris);
            }

            return gid;
        }

        /// <summary>
        /// 下载种子链接文件
        /// </summary>
        /// <param name="torrent">种子文件</param>
        /// <param name="fileName">输出文件名</param>
        /// <param name="dir">下载目录</param>
        /// <returns>成功返回任务标识符，失败返回空</returns>
        public static string AddTorrent(string torrent, string fileName = "", string dir = "")
        {
            FileStream fs = File.Open(torrent, FileMode.Open);
            byte[] bytes = new byte[fs.Length];
            fs.Close();

            string gid;
            XmlRpcStruct option = new XmlRpcStruct();

            if (dir != "")
            {
                option.Add("dir", dir);
            }

            if (fileName != "")
            {
                option.Add("out", fileName);
            }

            if (option.Count > 0)
            {
                gid = aria2c.AddTorrent(Aria2cRpcSecret, bytes, new string[] { "" }, option);
            }
            else
            {
                gid = aria2c.AddTorrent(Aria2cRpcSecret, bytes);
            }

            return gid;
        }

        /// <summary>
        /// 下载磁链接文件
        /// </summary>
        /// <param name="metalink">磁链接文件</param>
        /// <param name="dir">下载目录</param>
        /// <returns>成功返回任务标识符，失败返回空</returns>
        public static string AddMetalink(string metalink, string fileName = "", string dir = "")
        {
            FileStream fs = File.Open(metalink, FileMode.Open);
            byte[] bytes = new byte[fs.Length];
            fs.Close();

            string gid;
            XmlRpcStruct option = new XmlRpcStruct();

            if (dir != "")
            {
                option.Add("dir", dir);
            }

            if (fileName != "")
            {
                option.Add("out", fileName);
            }

            if (option.Count > 0)
            {
                gid = aria2c.AddMetalink(Aria2cRpcSecret, bytes, option);
            }
            else
            {
                gid = aria2c.AddMetalink(Aria2cRpcSecret, bytes);
            }

            return gid;
        }

        /// <summary>
        /// 删除正在下载任务
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool Remove(string gid)
        {
            string ok = aria2c.Remove(Aria2cRpcSecret, gid);
            return IsGidEquals(gid, ok);
        }


        /// <summary>
        /// 强制删除任务
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool ForceRemove(string gid)
        {
            string ok = aria2c.ForceRemove(Aria2cRpcSecret, gid);
            return IsGidEquals(gid, ok);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool Pause(string gid)
        {
            string ok = aria2c.Pause(Aria2cRpcSecret, gid);
            return IsGidEquals(gid, ok);
        }

        /// <summary>
        /// 强制暂停任务
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool ForcePause(string gid)
        {
            string ok = aria2c.ForcePause(Aria2cRpcSecret, gid);
            return IsGidEquals(gid, ok);
        }

        /// <summary>
        /// 暂停所有任务
        /// </summary>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool PauseAll()
        {
            string ok = aria2c.PauseAll(Aria2cRpcSecret);
            return IsOK(ok);
        }

        /// <summary>
        /// 强制暂停所有任务
        /// </summary>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool ForcePauseAll()
        {
            string ok = aria2c.ForcePauseAll(Aria2cRpcSecret);
            return IsOK(ok);
        }

        /// <summary>
        /// 取消暂停任务
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool UnPause(string gid)
        {
            string ok = aria2c.UnPause(Aria2cRpcSecret, gid);
            return IsGidEquals(gid, ok);
        }


        /// <summary>
        /// 取消暂停所有任务
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool UnPauseAll()
        {
            string ok = aria2c.UnPauseAll(Aria2cRpcSecret);
            return IsOK(ok);
        }

        /// <summary>
        /// 获取下载状态
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <param name="task">获取的任务信息</param>
        /// <param name="keys">属性过滤字段</param>
        /// <returns>成功返回任务信息，失败返回空</returns>
        public static Aria2cTask TellStatus(string gid, params string[] keys)
        {
            XmlRpcStruct xmlstruct = aria2c.TellStatus(Aria2cRpcSecret, gid, keys);
            Aria2cTask task = new Aria2cTask(xmlstruct);
            return task;
        }

        /// <summary>
        /// 获取下载任务的链接地址
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回下载地址列表，失败返回空</returns>
        public static List<Aria2cUri> GetUris(string gid)
        {
            XmlRpcStruct[] xmlstruct = aria2c.GetUris(Aria2cRpcSecret, gid);
            List<Aria2cUri> uris = Aria2cTools.ConvertToAria2cUris(xmlstruct);
            return uris;
        }

        /// <summary>
        /// 获取下载任务的文件信息
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回文件信息列表，失败返回空</returns>
        public static List<Aria2cFile> GetFiles(string gid)
        {
            XmlRpcStruct[] xmlstruct = aria2c.GetFiles(Aria2cRpcSecret, gid);
            List<Aria2cFile> files = Aria2cTools.ConvertToAria2cFiles(xmlstruct);
            return files;
        }

        /// <summary>
        /// 获取BT下载种子链接信息
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回种子链接信息列表，失败返回空</returns>
        public static List<Aria2cPeers> GetPeers(string gid)
        {
            XmlRpcStruct[] xmlstruct = aria2c.GetFiles(Aria2cRpcSecret, gid);
            List<Aria2cPeers> peers = Aria2cTools.ConvertToAria2cPeers(xmlstruct);
            return peers;
        }

        /// <summary>
        /// 获取普通下载服务器链接信息
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回服务器链接列表，失败返回空</returns>
        public static List<Aria2cLink> GetServers(string gid)
        {
            XmlRpcStruct[] xmlstruct = aria2c.GetFiles(Aria2cRpcSecret, gid);
            List<Aria2cLink> servers = Aria2cTools.ConvertToAria2cServers(xmlstruct);
            return servers;
        }

        /// <summary>
        /// 所有正在下载的任务信息
        /// </summary>
        /// <param name="tasks">返回的任务信息列表</param>
        /// <param name="keys">查找的属性关键字</param>
        /// <returns>成功返回正在下载的任务信息列表，失败返回空</returns>
        public static List<Aria2cTask> TellActive(params string[] keys)
        {
            XmlRpcStruct[] xmlstruct = aria2c.TellActive(Aria2cRpcSecret, keys);
            List<Aria2cTask> tasks = Aria2cTools.ConvertToAria2cTasks(xmlstruct);
            return tasks;
        }

        /// <summary>
        /// 所有正在等待的任务信息
        /// </summary>
        /// <param name="tasks">返回的任务信息列表</param>
        /// <param name="keys">查找的属性关键字</param>
        /// <returns>成功返回正在等待的任务信息列表，失败返回空</returns>
        public static List<Aria2cTask> TellWaiting(int offset, int num, params string[] keys)
        {
            XmlRpcStruct[] xmlstruct = aria2c.TellWaiting(Aria2cRpcSecret, offset, num, keys);
            List<Aria2cTask> tasks = Aria2cTools.ConvertToAria2cTasks(xmlstruct);
            return tasks;
        }

        /// <summary>
        /// 停止的的任务信息
        /// </summary>
        /// <param name="tasks">返回的任务信息列表</param>
        /// <param name="keys">查找的属性关键字</param>
        /// <returns>成功返回停止的的任务信息列表,失败返回空</returns>
        public static List<Aria2cTask> TellStopped(int offset, int num, params string[] keys)
        {
            XmlRpcStruct[] xmlstruct = aria2c.TellStopped(Aria2cRpcSecret, offset, num, keys);
            List<Aria2cTask> tasks = Aria2cTools.ConvertToAria2cTasks(xmlstruct);
            return tasks;
        }

        /// <summary>
        /// 改变任务在列表中的位置
        /// </summary>
        /// <param name="gid">任务标识符param>
        /// <param name="pos">位置</param>
        /// <param name="how">更改方式, "POS_SET"：列表首位, "POS_CUR"：移动到pos指定位置,  "POS_END"：列表末尾</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool ChangePosition(string gid, int pos, PosType how)
        {
            string desc = Aria2cTools.GetEnumDescription(how);
            int resultt = aria2c.ChangePosition(Aria2cRpcSecret, gid, pos, desc);
            return true;
        }

        /// <summary>
        /// 更改下载任务的链接信息
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <param name="fileIndex">一个下载任务可能包含多个下载文件，指定文件索引， 从1开始</param>
        /// <param name="delUris">要删除的现在链接</param>
        /// <param name="addUris">要添加的下载链接</param>
        /// <param name="position">链接插入的位置</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool ChangeUri(string gid, int fileIndex, string[] delUris, string[] addUris)
        {
            int[] array = aria2c.ChangeUri(Aria2cRpcSecret, gid, fileIndex, delUris, addUris);
            return true;
        }

        /// <summary>
        /// 获取下载任务设置
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <param name="option">设置信息</param>
        /// <returns>成功返回任务设置， 失败返回空</returns>
        public static Aria2cOption GetOption(string gid)
        {
            XmlRpcStruct xmlstruct = aria2c.GetOption(Aria2cRpcSecret, gid);
            Aria2cOption option = new Aria2cOption(xmlstruct);
            return option;

        }

        /// <summary>
        /// 下载任务设置
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <param name="options">设置参数</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool ChangeOption(string gid, Aria2cOption option)
        {
            string ok = aria2c.ChangeOption(Aria2cRpcSecret, gid, option.ToXmlRpcStruct());
            return IsOK(ok);
        }

        /// <summary>
        /// 获取全局下载设置
        /// </summary>
        /// <param name="option">设置信息</param>
        /// <returns>成功返回任务设置， 失败返回空</returns>
        public static Aria2cOption GetGlobalOption()
        {
            XmlRpcStruct xmlstruct = aria2c.GetGlobalOption(Aria2cRpcSecret);
            Aria2cOption option = new Aria2cOption(xmlstruct);
            return option;
        }

        /// <summary>
        /// 设置全局下载设置
        /// </summary>
        /// <param name="option">设置信息</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool ChangeGlobalOption(Aria2cOption option)
        {
            string ok = aria2c.ChangeGlobalOption(Aria2cRpcSecret, option.ToXmlRpcStruct());
            return IsOK(ok);
        }


        /// <summary>
        /// 获取全局下载状态
        /// </summary>
        /// <param name="globalStat">设置信息</param>
        /// <returns>成功返回全局下载状态， 失败返回空</returns>
        public static Aria2cGlobalStat GetGlobalStat()
        {
            XmlRpcStruct xmlstruct = aria2c.GetGlobalStat(Aria2cRpcSecret);
            Aria2cGlobalStat globalStat = new Aria2cGlobalStat(xmlstruct);
            return globalStat;
        }

        /// <summary>
        /// 清空内存
        /// </summary>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool PurgeDownloadResult()
        {
            string ok = aria2c.PurgeDownloadResult(Aria2cRpcSecret);
            return IsOK(ok);
        }


        /// <summary>
        /// 移除指定任务信息, 清空内存
        /// </summary>
        /// <param name="gid">任务标识符</param>
        /// <returns>成功返回true， 失败返回fale</returns>
        public static bool RemoveDownloadResult(string gid)
        {
            string ok = aria2c.RemoveDownloadResult(Aria2cRpcSecret, gid);
            return IsOK(ok);
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>成功返回版本号，失败返回空字符串</returns>
        public static string GetVersion()
        {
            XmlRpcStruct rpcStruct = aria2c.GetVersion(Aria2cRpcSecret);
            string version = Aria2cTools.GetRpcStructValue(rpcStruct, "version") as string;
            return version;
        }

        /// <summary>
        /// 获取会话ID
        /// </summary>
        /// <returns>成功返回会话ID，失败返回空字符串</returns>
        public static string GetSessionInfo()
        {
            XmlRpcStruct rpcStruct = aria2c.GetSessionInfo(Aria2cRpcSecret);
            string sessionId = Aria2cTools.GetRpcStructValue(rpcStruct, "sessionId") as string;
            return sessionId;
        }

        /// <summary>
        /// 关机
        /// </summary>
        public static bool Shutdown()
        {
            string ok = aria2c.Shutdown(Aria2cRpcSecret);
            return IsOK(ok);
        }

        /// <summary>
        /// 强制关机
        /// </summary>
        public static bool ForceShutdown()
        {
            string ok = aria2c.ForceShutdown(Aria2cRpcSecret);
            return IsOK(ok);
        }

        /// <summary>
        /// 保存会话
        /// </summary>
        public static bool SaveSession()
        {
            string ok = aria2c.SaveSession(Aria2cRpcSecret);
            return IsOK(ok);
        }


        #endregion

        #region 工具方法
        /// <summary>
        /// 判断字符串是否等于“OK”
        /// </summary>
        /// <param name="ok">字符串</param>
        /// <returns>相等返回true</returns>
        private static bool IsOK(string ok)
        {
            return string.Equals(ok, "OK", StringComparison.CurrentCultureIgnoreCase) ? true : false;
        }

        /// <summary>
        /// 任务标识符相等
        /// </summary>
        /// <param name="oGid">任务标识符</param>
        /// <param name="nGid">任务标识符</param>
        /// <returns>相等返回true</returns>
        private static bool IsGidEquals(string oGid, string nGid)
        {
            return string.Equals(oGid, nGid);
        }

        /// <summary>
        /// 获取下载目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static string GetDownLoadDirectory(string dir)
        {
            if (dir != "")
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return dir;
            }
            else if (DownLoadDirectory != "")
            {
                if (!Directory.Exists(DownLoadDirectory))
                {
                    Directory.CreateDirectory(DownLoadDirectory);
                }

                return DownLoadDirectory;
            }
            else
            {
                //string downDir = AppDomain.CurrentDomain.BaseDirectory + "\\DownLoad";
                string downDir = Path.Combine(Environment.CurrentDirectory, "Download");
                if (!Directory.Exists(downDir))
                {
                    Directory.CreateDirectory(downDir);
                }

                return downDir;
            }
        }
        #endregion
    }
}
