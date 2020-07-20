namespace HLS.Download.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class HLSStream
    {
        // Properties
        public string TargetDuration { get; private set; }
        public string AllowCache { get; private set; }
        public string PlaylistType { get; private set; }
        public HLSEncryptionKey Key { get; private set; }
        public string Version { get; private set; }
        public string MediaSequence { get; private set; }
        public HLSStreamPart[] Parts { get; private set; }
        public HLSPlaylist[] Playlist { get; private set; }

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
                    PARTS.Add(new HLSStreamPart(line, lines[i + 1]));
                    i = i + 1;
                    continue;
                }
                else if (line.Contains("EXT-X-ENDLIST"))
                    break; 
                else if (line.Contains("EXTM3U"))
                    continue;
                else if (line.Contains("TARGETDURATION"))
                {
                    STREAM.TargetDuration = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("ALLOW-CACHE"))
                {
                    STREAM.AllowCache = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("PLAYLIST-TYPE"))
                {
                    STREAM.PlaylistType = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("EXT-X-KEY"))
                {
                    STREAM.Key = new HLSEncryptionKey(line.Substring(line.IndexOf(':') + 1));
                    continue;
                }
                else if (line.Contains("VERSION"))
                {
                    STREAM.Version = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("MEDIA-SEQUENCE"))
                {
                    STREAM.MediaSequence = line.Split(':')[1];
                    continue;
                }
                else if (line.Contains("EXT-X-STREAM-INF"))
                {
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
                        }
                    }

                    //
                    tmpPlaylist.URI = lines[i + 1];
                    i = i + 1;

                    Playlist.Add(tmpPlaylist);
                    #endregion
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
