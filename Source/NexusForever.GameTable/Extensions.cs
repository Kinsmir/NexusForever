using System.Text;
using NLog;

namespace NexusForever.GameTable
{
    public static class Extensions
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        public static string ReadWideString(this BinaryReader reader)
        {
            // Assume that we might not know the reader's encoding
            // Capture the current position of the base stream in case it's needed
            long originalPosition = reader.BaseStream.Position;

            // Try to read the string assuming the reader is using Unicode
            var characters = new List<char>();
            try
            {
                while (true)
                {
                    char character = reader.ReadChar();  // This assumes Encoding.Unicode if configured correctly
                    if (character == 0)
                        break;
                    characters.Add(character);
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs (e.g., due to incorrect encoding), reset and try a fallback method
                reader.BaseStream.Position = originalPosition;

                // Log the exception
                log.Warn(ex, "Failed to read wide string with current encoding, retrying with Encoding.Unicode.");
                
                // Fallback to using a manually configured BinaryReader for Unicode
                using var safeReader = new BinaryReader(reader.BaseStream, Encoding.Unicode, true);
                characters.Clear();
                while (true)
                {
                    char character = safeReader.ReadChar();
                    if (character == 0)
                        break;
                    characters.Add(character);
                }
            }

            return new string(characters.ToArray());
        }
    }
}
