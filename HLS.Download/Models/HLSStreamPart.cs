namespace HLS.Download.Models
{
    public class HLSStreamPart
    {
        // Properties
        public string Length { get; private set; }
        public string Path { get; private set; }
        public string Title { get; private set; }

        // Constructor
        public HLSStreamPart(string line1, string line2)
        {
            string[] parts = line1.Split(new char[] { ':', ',' });
            Length = parts[1];
            if(parts.Length>2)
                Title = parts[2];

            Path = line2;
        }
    }
}
