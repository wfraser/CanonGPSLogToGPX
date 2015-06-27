# CanonGPSLogToGPX

![Screenshot](https://raw.githubusercontent.com/wfraser/CanonGPSLogToGPX/master/Screenshot/Capture.PNG)

A small Windows utility to convert Canon DSLR GPS tracks (a particular variant of NMEA format) to more standard .GPX files.

### Note: this has only been tested with a *Canon EOS 6D*.

Canon DSLRs that have integrated GPS units can be configured to record a track of everywhere you go, and can output that track to a file on the SD card.

Its format is a particular variant of the NMEA standard (a format mostly used for marine GPS applications), and it writes just two sentences ($GPGGA and $GPRMC) for each track point.

This little program takes that format and converts it to the more standard GPX format.

It uses both NMEA sentences because each contains a little information the other lacks:
* $GPGGA is the only one with an altitude
* $GPRMC is the only one with a date

I'm using GPX serializer code borrowed from another project of mine, [Breadcrumbs](https://github.com/wfraser/Breadcrumbs).
