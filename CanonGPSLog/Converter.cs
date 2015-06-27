using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CanonGPSLog
{
    public static class Converter
    {
        public static void LogToGPX(string inputPath, string outputPath)
        {
            var gpx = new GPX();
            gpx.Creator = "Canon GPS Log to GPX Converter - https://github.com/wfraser/CanonGPSLogToGPX";

            using (var reader = new StreamReader(inputPath))
            {
                var sentences = NMEA.NmeaData.Read(reader);

                if (sentences.Count % 2 != 0)
                {
                    throw new InvalidDataException("Error: odd number of NMEA sentences");
                }

                for (int i = 0, n = sentences.Count; i < n; i += 2)
                {
                    var gga = sentences[i] as NMEA.GGA;
                    var rmc = sentences[i + 1] as NMEA.RMC;

                    if (gga == null || rmc == null)
                    {
                        throw new InvalidDataException("Error: input is expected to be only $GPGGA followed by $GPRMC, alternating.");
                    }

                    var point = new GPX.TrackPoint()
                    {
                        Latitude = gga.Latitude,
                        Longitude = gga.Longitude,
                        Altitude = gga.Altitude,
                        DateTime = rmc.DateTime,
                    };

                    gpx.AddTrackPoint(point);
                }
            }

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                gpx.Serialize(stream);
            }
        }
    }
}
