namespace HLS.Download.Models
{
    public class HLSEncryptionKey
    {
        // Properties
        public string Method { get; private set; }
        public string Path { get; private set; }
        public string IV { get; private set; }

        // Constructor
        public HLSEncryptionKey(string line)
        {
            string[] parts = line.Split(',');
            Method = "";
            Path = "";
            IV = "";

            foreach (var part in parts)
            {
                if (part.Split('=')[0] == "METHOD")
                    Method = part.Split('=')[1];
                else if (part.Split('=')[0] == "URI")
                    Path = part.Substring(part.IndexOf('=')+1).Trim('"');
                else if (part.Split('=')[0] == "IV")
                    IV = part.Split('=')[1];
            }
        }
    }
}
