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

                byte readSum = byte.Parse(checksum, System.Globalization.NumberStyles.HexNumber);
                byte calculatedSum = CalculateChecksum(line);
                if (readSum != calculatedSum)
                {
                    throw new InvalidDataException(string.Format("Line {0}: checksum mismatch - calculated \"{1:X}\", read \"{2:X}\" in file", lineNumber, calculatedSum, readSum));
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

                try
                {
                    sentence.Read(fields);
                }
                catch (InvalidDataException ex)
                {
                    throw new InvalidDataException(
                        string.Format("Data error on line {0}: {1}", lineNumber, ex.Message),
                        ex
                        );
                }

                sentences.Add(sentence);
            }

            return sentences;
        }

        private static byte CalculateChecksum(string line)
        {
            byte sum = 0;
            for (int i = 1; i < line.Length; i++)
            {
                sum ^= (byte)line[i];
            }

            return sum;
        }
    }
}
