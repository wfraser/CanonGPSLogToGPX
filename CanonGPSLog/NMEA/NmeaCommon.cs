using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonGPSLog.NMEA
{
    public class NmeaCommon
    {
        protected NmeaCommon()
        { }

        protected TimeSpan ParseTime(string field)
        {
            int hour = int.Parse(field.Substring(0, 2));
            int minute = int.Parse(field.Substring(2, 2));
            int second = int.Parse(field.Substring(4, 2));
            int millisecond = 0;
            if (field[6] == '.')
            {
                for (int i = 0; i < Math.Max(3, field.Length - 7); i++)
                {
                    millisecond *= 10;
                    millisecond += field[7 + i] - '0';
                }
            }

            return new TimeSpan(0, hour, minute, second, millisecond);
        }

        private double ParseLatLong(string field)
        {
            int dotPos = field.IndexOf('.');
            double degrees = double.Parse(field.Substring(0, dotPos - 2));
            double minutes = double.Parse(field.Substring(dotPos - 2));
            return degrees + minutes / 60;
        }

        protected double ParseLatitude(string field1, string field2)
        {
            double latitude = ParseLatLong(field1);
            if (field2 == "S")
                latitude *= -1;
            else if (field2 != "N")
                throw new InvalidDataException("Latitude must be followed by 'N' or 'S'");
            return latitude;
        }

        protected double ParseLongitude(string field1, string field2)
        {
            double longitude = ParseLatLong(field1);
            if (field2 == "W")
                longitude *= -1;
            else if (field2 != "E")
                throw new InvalidDataException("Longitude must be followed by 'E' or 'W'");
            return longitude;
        }
    }
}
