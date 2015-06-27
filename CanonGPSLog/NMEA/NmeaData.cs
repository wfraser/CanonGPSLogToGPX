using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CanonGPSLog.NMEA
{
    public class NmeaData
    {
        public static IList<NmeaSentence> Read(StreamReader reader)
        {
            var sentences = new List<NmeaSentence>();
            string header = reader.ReadLine();

            int lineNumber = 1;
            string line;
            while (null != (line = reader.ReadLine()))
            {
                lineNumber++;

                int starPos = line.LastIndexOf('*');
                string checksum = line.Substring(starPos + 1);
                line = line.Substring(0, starPos);

                bool valid = ValidateChecksum(line, checksum);
                if (!valid)
                {
                    throw new InvalidDataException(string.Format("Line {0}: invalid checksum", lineNumber));
                }

                string[] fields = line.Split(',');
                NmeaSentence sentence = null;
                switch (fields[0])
                {
                    case "$GPGGA":
                        sentence = new GGA();
                        break;
                    case "$GPRMC":
                        sentence = new RMC();
                        break;
                    default:
                        throw new InvalidDataException(string.Format("Line {0}: unknown sentence \"{1}\"", lineNumber, fields[0]));
                }

                sentence.Read(fields);

                sentences.Add(sentence);
            }

            return sentences;
        }

        private static bool ValidateChecksum(string line, string checksum)
        {
            if (line[0] != '$')
                return false;

            byte expectedSum = byte.Parse(checksum, System.Globalization.NumberStyles.HexNumber);

            byte sum = 0;
            for (int i = 1; i < line.Length; i++)
            {
                sum ^= (byte)line[i];
            }

            return (sum == expectedSum);
        }
    }
}
