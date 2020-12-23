namespace HLS.Download.Models
{
    /// <summary>
    /// #region 参考资料
    /// https://blog.csdn.net/langeldep/article/details/8603045
    ///   [draft-pantos-http-live-streaming-06 - HTTP Live Streaming](https://tools.ietf.org/html/draft-pantos-http-live-streaming-06) 

    ///      3.3.8.  EXT-X-STREAM-INF

    /// The EXT-X-STREAM-INF tag indicates that the next URI in the Playlist
    /// file identifies another Playlist file.  Its format is:

    /// #EXT-X-STREAM-INF:attribute-list
    /// URI

    /// The following attributes are defined:

    /// BANDWIDTH

    /// The value is a decimal-integer of bits per second.  It MUST be an
    /// upper bound of the overall bitrate of each media file, calculated to
    /// include container overhead, that appears or will appear in the
    /// Playlist.

    /// Every EXT-X-STREAM-INF tag MUST include the BANDWIDTH attribute.

    /// PROGRAM-ID

    /// The value is a decimal-integer that uniquely identifies a particular
    /// presentation within the scope of the Playlist file.

    /// A Playlist file MAY contain multiple EXT-X-STREAM-INF tags with the
    /// same PROGRAM-ID to identify different encodings of the same
    /// presentation.  These variant playlists MAY contain additional EXT-X-
    /// STREAM-INF tags.

    /// CODECS

    /// The value is a quoted-string containing a comma-separated list of
    /// formats, where each format specifies a media sample type that is
    /// present in a media file in the Playlist file.  Valid format
    /// identifiers are those in the ISO File Format Name Space defined by
    /// RFC 4281 [RFC4281].

    /// Every EXT-X-STREAM-INF tag SHOULD include a CODECS attribute.

    /// RESOLUTION

    /// The value is a decimal-resolution describing the approximate encoded
    /// horizontal and vertical resolution of video within the stream.*/
    /// #endregion
    /// </summary>
    public class HLSPlaylist
    {
        // Properties
        public string BANDWIDTH { get; set; }
        public string PROGRAM_ID { get; set; }
        public string CODECS { get; set; }
        public string RESOLUTION { get; set; }
        public string URI { get; set; }
        public string AUDIO { get; set; }
        public string VIDEO { get; set; }
    }
}
