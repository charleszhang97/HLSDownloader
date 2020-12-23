namespace HLS.Download.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class HLSStream
    {
        // Properties
        public HLSPlaylist[] Playlist { get; private set; }
        public HLSStreamPart[] Parts { get; private set; }
        public HLSEncryptionKey Key { get; private set; }
        public string TargetDuration { get; private set; }
        public string AllowCache { get; private set; }
        public string PlaylistType { get; private set; }
        public string Version { get; private set; }
        public string MediaSequence { get; private set; }
        public string ByteRange { get; private set; }
        public string Program_Date_Time { get; private set; }
        public string Media { get; private set; }
        public string Discontinuity { get; private set; }
        public string Zen_Total_Duration { get; private set; }

        /// <summary>
        /// Parse a HLSStream object from a string
        /// </summary>
        /// <param name="m3u8">The string</param>
        /// <returns>Returns a parsed HLSStream object</returns>
        private static HLSStream ParseFromString(string m3u8)
        {
            string[] lines = m3u8.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            HLSStream STREAM = new HLSStream();
            List<HLSStreamPart> PARTS = new List<HLSStreamPart>();
            List<HLSPlaylist> Playlist = new List<HLSPlaylist>();

            string line;
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i];

                if (line.Contains("EXTINF"))
                {
                    /*
                     指定每个媒体段（ts）的持续时间，这个仅对其后面的 URI 有效，每两个媒体段 URI 间被这个 tag 分隔开
                    其格式为：#EXTINF:<duration>,<title> 
                     */
                    PARTS.Add(new HLSStreamPart(line, lines[i + 1]));
                    i = i + 1;
                    continue;
                }
                else if (line.Contains("EXT-X-ENDLIST"))
                    break;
                else if (line.Contains("EXT-X-STREAM-INF"))
                {
                    /*
                    指定一个包含多媒体信息的 media URI 作为 Playlist，一般做 m3u8 的嵌套使用，它只对紧跟后面的 URI 有效，格式如下：#EXT-X-STREAM-INF:<attribute-list>
                    常用的属性如下：
                    BANDWIDTH：带宽，必须有
                    PROGRAM-ID：该值是一个十进制整数，唯一地标识一个在 Playlist 文件范围内的特定的描述。一个 Playlist 文件中可能包含多个有相同 ID 的此 tag
                    CODECS：指定流的编码类型，不是必须的
                    RESOLUTION：分辨率
                    AUDIO：这个值必须和 AUDIO 类别的 "EXT-X-MEDIA" 标签中 "GROUP-ID" 属性值相匹配
                    VIDEO：同上
                     */
                    #region 多码率适配流
                    var tmpPlaylist = new HLSPlaylist();

                    //#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=800000,RESOLUTION=720x406,CODECS="avc1.4d401f,mp4a.40.2"
                    var tmpItems = line.Split("\"".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var codecsValue = "";
                    var tmpLine = line;
                    if (tmpItems.Length == 2)
                    {
                        tmpLine = tmpItems[0];
                        codecsValue = tmpItems[1];
                    }
                    var items = tmpLine.Split("#:,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (var si = 1/*跳过EXT-X-STREAM-INF*/; si < items.Length; si++)
                    {
                        var keyValue = items[si].Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        switch (keyValue[0])
                        {
                            case "BANDWIDTH":
                                tmpPlaylist.BANDWIDTH = keyValue[1];
                                break;
                            case "CODECS":
                                tmpPlaylist.CODECS = codecsValue;// keyValue[1];
                                break;
                            case "PROGRAM-ID":
                                tmpPlaylist.PROGRAM_ID = keyValue[1];
                                break;
                            case "RESOLUTION":
                                tmpPlaylist.RESOLUTION = keyValue[1];
                                break;
                            case "AUDIO":
                                tmpPlaylist.AUDIO = keyValue[1];
                                break;
                            case "VIDEO":
                                tmpPlaylist.VIDEO = keyValue[1];
                                break;
                            default:
                                break;
                        }
                    }

                    //
                    tmpPlaylist.URI = lines[i + 1];
                    i = i + 1;

                    Playlist.Add(tmpPlaylist);
                    #endregion
                    continue;
                }
                else if (line.Contains("EXT-X-KEY"))
                {
                    /*
                    表示怎么对 media segments 进行解码。其作用范围是下次该 tag 出现前的所有 media URI。
                    格式为：#EXT-X-KEY:<attribute-list>
                    NONE 或者 AES-128。如果是 NONE，则 URI 以及 IV 属性必须不存在，如果是 AES-128(Advanced Encryption Standard)，则 URI 必须存在，IV 可以不存在。
                    对于 AES-128 的情况，keytag 和 URI 属性共同表示了一个 key 文件，通过 URI 可以获得这个 key，如果没有 IV（Initialization Vector），则使用序列号作为 IV 进行编解码，将序列号的高位赋到 16 个字节的 buffer 中，左边补 0；如果有 IV，则将该值当成 16 个字节的 16 进制数。 
                    */
                    STREAM.Key = new HLSEncryptionKey(line.Substring(line.IndexOf(':') + 1));
                    continue;
                }
                else if (line.Contains("EXT-X-TARGETDURATION"))
                {
                    /*
                    指定当前视频流中的单个切片（即 ts）文件的最大时长（秒）。所以 #EXTINF 中指定的时间长度必须小于或是等于这个最大值。这个 tag 在整个 Playlist 文件中只能出现一次（在嵌套的情况下，一般有真正
                    ts url 的 m3u8 才会出现该 tag）。格式为：#EXT-X-TARGETDURATION:<s>
                     */
                    STREAM.TargetDuration = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("EXT-X-ALLOW-CACHE"))
                {
                    STREAM.AllowCache = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("EXT-X-PLAYLIST-TYPE"))
                {
                    /*
                    # EXT-X-PLAYLIST-TYPE:<EVENT|VOD>
                    VOD，即为点播视频，服务器不能改变 Playlist 文件，换句话说就是该视频全部的 ts 文件已经被生成好了
                    EVENT，就是实时生成 m3u8 和 ts 文件。服务器不能改变或是删除 Playlist 文件中的任何部分，但是可以向该文件中增加新的一行内容。它的索引文件一直处于动态变化中，播放的时候需要不断下载二级 index 文件
                    */
                    STREAM.PlaylistType = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("EXT-X-VERSION"))
                {
                    STREAM.Version = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("EXT-X-MEDIA-SEQUENCE"))
                {
                    STREAM.MediaSequence = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("EXTM3U"))
                    continue;
                else if (line.Contains("EXT-X-BYTERANGE"))
                {
                    /*
                    表示媒体段是一个媒体 URI 资源中的一段，只对其后的 media URI 有效，
                    格式为：#EXT-X-BYTERANGE:<n>[@o]
                    n：表示这个区间的大小
                    o：表示在 URI 中的 offset
                     */
                    STREAM.ByteRange = line.Substring(line.IndexOf(':') + 1);
                    continue;
                }
                else if (line.Contains("EXT-X-PROGRAM-DATE-TIME"))//#EXT-X-PROGRAM-DATE-TIME:<YYYY-MM-DDThh:mm:ssZ>
                {
                    STREAM.Program_Date_Time = line.Substring(line.IndexOf(':') + 1);
                    continue;
                }
                else if (line.Contains("EXT-X-MEDIA"))
                {
                    /*
                    #EXT-X-MEDIA:<attribute-list>
                    该属性列表中包含：URI、TYPE、GROUP-ID、LANGUAGE、NAME、DEFAULT、AUTOSELECT。
                    URI：如果没有，则表示这个 tag 描述的可选择版本在主 PlayList 的 EXT-X-STREAM-INF 中存在
                    TYPE：AUDIO and VIDEO
                    GROUP-ID：具有相同 ID 的 MEDIAtag，组成一组样式
                    LANGUAGE：identifies the primary language used in the rendition
                    NAME：The value is a quoted-string containing a human-readable description of the rendition. If the LANGUAGE attribute is present then this description SHOULD be in that language
                    DEFAULT：YES 或是 NO，默认是 No，如果是 YES，则客户端会以这种选项来播放，除非用户自己进行选择
                    AUTOSELECT：YES 或是 NO，默认是 No，如果是 YES，则客户端会根据当前播放环境来进行选择（用户没有根据自己偏好进行选择的前提下）    
                    */
                    STREAM.Media = line.Substring(line.IndexOf(':') + 1);
                    continue;
                }
                else if (line.Contains("EXT-X-DISCONTINUITY"))//当遇到该 tag 的时候说明属性发生了变化
                {
                    STREAM.Discontinuity = line.Substring(line.IndexOf(':') + 1);
                    continue;
                }
                else if (line.Contains("ZEN-TOTAL-DURATION"))//表示这个 m3u8 所含 ts 的总时间长度
                {
                    STREAM.Zen_Total_Duration = line.Substring(line.IndexOf(':') + 1);
                    continue;
                }

            }
            line = null;
            STREAM.Playlist = Playlist.ToArray();
            STREAM.Parts = PARTS.ToArray();

            return STREAM;
        }

        /// <summary>
        /// Opens a HLSStream object from a given file path
        /// </summary>
        /// <param name="path">File path string</param>
        /// <returns>Returns a parsed HLSStream object</returns>
        public async static Task<HLSStream> Open(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return ParseFromString(await reader.ReadToEndAsync());
            }
        }

        /// <summary>
        /// Parses a HLSStream object from a string
        /// </summary>
        /// <param name="text">The string</param>
        /// <returns>Returns a parsed HLSStream object</returns>
        public static HLSStream Parse(string text)
        {
            return ParseFromString(text);
        }
    }
}
