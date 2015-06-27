using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonGPSLog.NMEA
{
    public class RMC : NmeaCommon, NmeaSentence
    {
        public string Name() { return "RMC"; }
        public string Description() { return "NMEA Fix Data"; }

        public DateTime DateTime;

        public enum StatusType
        {
            Void,
            Active,
        }

        public StatusType Status;

        public double Latitude, Longitude;

        public double? GroundSpeedKnots;

        public double? TrackAngle;

        public void Read(string[] fields)
        {
            if (fields.Length != 13)
            {
                throw new InvalidDataException(string.Format("$GPRMC sentence needs to have 13 fields, but {0} were found.", fields.Length));
            }

            int n = 0;

            if (fields[n] != "$GPRMC")
                throw new InvalidDataException(string.Format("Expected \"$GPRMC\", found \"{0}\"", fields[n]));
            n++;

            TimeSpan time = ParseTime(fields[n]);
            n++;

            if (fields[n] == "A")
                Status = StatusType.Active;
            else if (fields[n] == "V")
                Status = StatusType.Void;
            else
                throw new InvalidDataException(string.Format("Unknown status \"{0}\"", fields[n]));
            n++;

            Latitude = ParseLatitude(fields[n], fields[n + 1]);
            n += 2;

            Longitude = ParseLongitude(fields[n], fields[n + 1]);
            n += 2;

            double speed;
            if (double.TryParse(fields[n], out speed))
                GroundSpeedKnots = speed;
            n++;

            double angle;
            if (double.TryParse(fields[n], out angle))
                TrackAngle = angle;
            n++;

            int day = int.Parse(fields[n].Substring(0, 2));
            int month = int.Parse(fields[n].Substring(2, 2));
            int year = int.Parse(fields[n].Substring(4, 2));

            if (year > 90)
                year += 1900;
            else
                year += 2000;

            DateTime = new DateTime(year, month, day, time.Hours, time.Minutes, time.Seconds, time.Milliseconds, DateTimeKind.Utc);

            //Magnetic variation not implemented
            //Fix kind not implemented
        }

        public override string ToString()
        {
            return string.Format("RMC: {0} {1} {2},{3} {4} knots {5} degrees",
                Status.ToString(),
                DateTime,
                Latitude,
                Longitude,
                GroundSpeedKnots.ValueOrNaN(),
                TrackAngle.ValueOrNaN()
                );
        }
    }
}
