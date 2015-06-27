using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonGPSLog.NMEA
{
    public class GGA : NmeaCommon, NmeaSentence
    {
        public TimeSpan Time;

        public double Latitude, Longitude;

        public enum FixQuality
        {
            Invalid = 0,
            GPS = 1,
            DGPS = 2,
            PPS = 3,
            RealTimeKinematic = 4,
            FloatRTK = 5,
            DeadReckoning = 6,
            ManualInput = 7,
            Simulation = 8,
        }

        public FixQuality Quality;

        public int NumberOfSatellites;

        public double? HDOP;

        public double? Altitude;

        public double? HeightOfGeoid;

        public void Read(string[] fields)
        {
            if (fields.Length != 15)
            {
                throw new InvalidDataException(string.Format("$GPGGA sentence needs to have 15 fields, but {0} were found.", fields.Length));
            }

            int n = 0;

            if (fields[n] != "$GPGGA")
                throw new InvalidDataException(string.Format("Expected \"$GPGGA\", found \"{0}\"", fields[n]));
            n++;

            Time = ParseTime(fields[n]);
            n++;

            Latitude = ParseLatitude(fields[n], fields[n + 1]);
            n += 2;

            Longitude = ParseLongitude(fields[n], fields[n + 1]);
            n += 2;

            int quality = int.Parse(fields[n]);
            if (quality >= 0 && quality <= 8)
                Quality = (FixQuality)quality;
            else
                throw new InvalidDataException(string.Format("Fix quality is an unknown value: {0}", fields[n]));
            n++;

            NumberOfSatellites = int.Parse(fields[n]);
            n++;

            HDOP = double.Parse(fields[n]);
            n++;

            double altitude;
            if (double.TryParse(fields[n], out altitude))
                Altitude = altitude;
            n++;

            if (Altitude.HasValue && fields[n] != "M")
                throw new InvalidDataException(string.Format("Altitude must be in meters (\"M\"), got \"{0}\" instead.", fields[n]));
            n++;

            double heightOfGeoid;
            if (double.TryParse(fields[n], out heightOfGeoid))
                HeightOfGeoid = heightOfGeoid;
            n++;

            if (HeightOfGeoid.HasValue && fields[n] != "M")
                throw new InvalidDataException(string.Format("Height of Geoid must be in meters (\"M\"), got \"{0}\" instead.", fields[n]));

            // DGPS not implemented
        }

        public override string ToString()
        {
            return string.Format("GGA: {0} {1},{2} {3}m via {4}",
                Time,
                Latitude,
                Longitude,
                Altitude.ValueOrNaN(),
                Quality.ToString()
                );
        }
    }
}
