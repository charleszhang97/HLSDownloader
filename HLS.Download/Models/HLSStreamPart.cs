namespace HLS.Download.Models
{
    public class HLSStreamPart
    {
        // Properties
        public string Length { get; private set; }
        public string Path { get; private set; }

        // Constructor
        public HLSStreamPart(string line1, string line2)
        {
            Length = line1.Split(':')[1];
            Path = line2;
        }
    }
}
