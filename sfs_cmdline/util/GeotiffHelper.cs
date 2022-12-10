using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfs_cmdline.util.util
{
    public class GeotiffHelper
    {
        public static string dst_wkt = ToWkt(@"PROJCS['unnamed',GEOGCS['unnamed ellipse',DATUM['unknown',SPHEROID['unnamed',1737400,0]],PRIMEM['Greenwich',0],UNIT['degree',0.0174532925199433]],PROJECTION['Polar_Stereographic'],PARAMETER['latitude_of_origin',-90],PARAMETER['central_meridian',0],PARAMETER['scale_factor',1],PARAMETER['false_easting',0],PARAMETER['false_northing',0],UNIT['metre',1,AUTHORITY['EPSG','9001']]]");

        public static PointD PixelToCoordinate(int line, int sample)
        {
            const double S0 = 15199.5d;             // PDS SAMPLE_PROJECTION_OFFSET
            const double L0 = 15199.5d;             // PDS LINE_PROJECTION_OFFSET
            const double Scale = 20d;

            var x = (sample - S0) * Scale;
            var y = (L0 - line) * Scale;
            return new PointD(x, y);
        }

        public static RectangleD GetGeoExtent(Rectangle r)
        {
            var ul = PixelToCoordinate(r.Top, r.Left);  // line,sample
            var lr = PixelToCoordinate(r.Bottom, r.Right);
            return new RectangleD(ul, lr);
        }

        // [minx, scalex, 0, maxy, 0, -scaley]
        public static double[] GetAffineTransformOfBounds(Rectangle bounds)
        {
            var r = GetGeoExtent(bounds);
            return new double[] { r.Left, 20d, 0d, r.Top, 0d, -20d };
        }

        public static string ToWkt(string s) => s.Replace('\'', '"');

         #region GDAL Driver Support

        static Driver _memory_driver = null;
        static Driver _geotif_driver = null;

        public static bool GetGdalDrivers()
        {
            if (_memory_driver != null)
                return true;
            GdalConfiguration.ConfigureGdal();
            const string gdalFormat = "GTiff";
            _memory_driver = Gdal.GetDriverByName("MEM");
            _geotif_driver = Gdal.GetDriverByName(gdalFormat);
            if (!(_memory_driver != null && _geotif_driver != null))
            {
                Console.WriteLine($"Can't load {(_geotif_driver == null ? "GTiff" : "MEM")} driver.");
                if (_memory_driver != null) _memory_driver.Dispose();
                if (_geotif_driver != null) _geotif_driver.Dispose();
                _memory_driver = _geotif_driver = null;
                return false;
            }
            return true;
        }

        public static Driver GetGeoTIFFDriver()
        {
            if (GetGdalDrivers())
                return _geotif_driver;
            throw new Exception(@"Couldn't open both GDAL Drivers");
        }

        public static Driver GetMemoryDriver()
        {
            if (GetGdalDrivers())
                return _memory_driver;
            throw new Exception(@"Couldn't open both GDAL Drivers");
        }

        #endregion
    }
}
