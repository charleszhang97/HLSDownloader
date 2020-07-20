using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HLS.Download.Models
{
    public class HLSStreamEntry
    {
        // Properties
        public string ProgramId { get; private set; }
        public string Bandwidth { get; private set; }
        public string Resolution { get; private set; }
        public string FrameRate { get; private set; }
        public string Codecs { get; private set; }
        public string Path { get; private set; }

        // Constructor
        public HLSStreamEntry(string line, string line2)
        {
            string[] parts = line.Split(',');
            ProgramId = parts[0].Split('=')[1];
            Bandwidth = parts[1].Split('=')[1];
            Resolution = parts[2].Split('=')[1];
            FrameRate = parts[3].Split('=')[1];
            Codecs = string.Join(", ", parts[4].Split('=')[1], parts[5]);
            Path = line2;
        }

        /// <summary>
        /// Parses a HLS adaptive stream list from a string
        /// </summary>
        /// <param name="m3u8">The string</param>
        /// <returns>Returns an array of adaptive stream entries</returns>
        private static HLSStreamEntry[] Parse(string m3u8)
        {
            string[] lines = m3u8.Split('\n');
            List<HLSStreamEntry> Entries = new List<HLSStreamEntry>();

            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("EXT-X-STREAM-INF"))
                {
                    string line2 = lines[i + 1];

                    Entries.Add(new HLSStreamEntry(line, line2));
                    i = i + 2;
                }
            }

            return Entries.ToArray();
        }

        /// <summary>
        /// Parses an array of adaptive stream entries from a given file path
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>Returns an array of adaptive stream entries</returns>
        public async static Task<HLSStreamEntry[]> GetEntries(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return Parse(await reader.ReadToEndAsync());
            }
        }
    }
}
