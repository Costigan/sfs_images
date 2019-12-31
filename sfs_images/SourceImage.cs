using OSGeo.GDAL;
using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using sfs_images.util;

namespace sfs_images
{
    public class SourceImage : IDisposable
    {
        protected Bitmap _cached_raw_image = null;
        protected Bitmap _cached_projected_image = null;
        protected LitPatches _lit_patches = null;
        protected Size? _size;
        protected float[,] _data = null;

        Driver _geotiff_driver = null, _png_driver = null, _mem_driver = null;

        public static string PolarWkt = ToWkt(@"PROJCS['unnamed',GEOGCS['unnamed ellipse',DATUM['unknown',SPHEROID['unnamed',1737400,0]],PRIMEM['Greenwich',0],UNIT['degree',0.0174532925199433]],PROJECTION['Polar_Stereographic'],PARAMETER['latitude_of_origin',-90],PARAMETER['central_meridian',0],PARAMETER['scale_factor',1],PARAMETER['false_easting',0],PARAMETER['false_northing',0],UNIT['metre',1,AUTHORITY['EPSG','9001']]]");
        public static string PolarWkt2 = ToWkt(@"PROJCS['Moon_South_Pole_Stereographic', GEOGCS['Moon 2000', DATUM['D_Moon_2000', SPHEROID['Moon_2000_IAU_IAG', 1737400.0, 0.0]], PRIMEM['Greenwich', 0], UNIT['Decimal_Degree', 0.0174532925199433]], PROJECTION['Stereographic'], PARAMETER['False_Easting', 0], PARAMETER['False_Northing', 0], PARAMETER['Central_Meridian', 0], PARAMETER['Scale_Factor', 1], PARAMETER['Latitude_Of_Origin', -90], UNIT['Meter', 1]]");

        public static string ToWkt(string s) => s.Replace('\'', '"');

        public SourceImage() { }
        public SourceImage(FileInfo fi)
        {
            Name = fi.Name;
            FullName = fi.FullName;
        }
        public SourceImage(string path)
        {
            Name = Path.GetFileName(path);
            FullName = path;
        }

        public string Name { get; set; }
        public string FullName { get; set; }
        public int Index { get; set; } = -1;  // Location in the listview
        public bool HasProjectedImage { get; set; }

        public override string ToString() => Name;

        public string CachedRawImagePath => Path.Combine(MainForm.TempDirectory, Path.ChangeExtension(Path.GetFileName(FullName), ".png"));
        public string CachedProjectedImagePath => Path.Combine(MainForm.ProjectedDirectory, Path.ChangeExtension(Path.GetFileName(FullName), ".png"));

        public Size Size
        {
            get
            {
                if (_size.HasValue)
                    return _size.Value;
                if (_cached_raw_image != null)
                    return _cached_raw_image.Size;
                using (var ds = Gdal.Open(FullName, Access.GA_ReadOnly))  // Path might be null
                {
                    _size = new Size(ds.RasterXSize, ds.RasterYSize);
                    return _size.Value;
                }
            }
        }
        
        public string Projection
        {
            get
            {
                using (var ds = Gdal.Open(FullName, Access.GA_ReadOnly))  // Path might be null
                    return ds.GetProjection();
            }
        }

        public RectangleF BoundsInProjection
        {
            get
            {
                using (var ds = Gdal.Open(FullName, Access.GA_ReadOnly))  // Path might be null
                {
                    var s = new Size(ds.RasterXSize, ds.RasterYSize);
                    _size = s;
                    double[] a = new double[6];
                    ds.GetGeoTransform(a);
                    return new RectangleF((float)a[0], (float)a[3], s.Width * (float)a[1], s.Height * (float)a[5]);
                }
            }
        }

        public unsafe Bitmap CachedRawImage
        {
            get
            {
                if (_cached_raw_image != null)
                    return _cached_raw_image;

                var cached_image_path = CachedRawImagePath;
                if (File.Exists(cached_image_path))
                    return _cached_raw_image = Image.FromFile(cached_image_path) as Bitmap;

                var (_data, noval, hasnoval) = ReadGeotiffAsFloatArray(FullName);
                if (_data == null) return null;
                var band_no_data = (float)noval;
                // Find range
                var height = _data.GetLength(0);
                var width = _data.GetLength(1);
                var min = float.MaxValue;
                var max = float.MinValue;
                if (hasnoval)
                    for (var row = 0; row < height; row++)
                        for (var col = 0; col < width; col++)
                        {
                            var v = _data[row, col];
                            if (v == band_no_data) continue;
                            min = Math.Min(min, v);
                            max = Math.Max(max, v);
                        }
                else
                    for (var row = 0; row < height; row++)
                        for (var col = 0; col < width; col++)
                        {
                            var v = _data[row, col];
                            min = Math.Min(min, v);
                            max = Math.Max(max, v);
                        }
                // Map [min,max]->[0,200]
                var a = 200f / (max - min);
                var b = -min;
                var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                for (var row = 0; row < height; row++)
                {
                    var rowptr = (byte*)(bmpdata.Scan0 + bmpdata.Stride * row);
                    for (var col = 0; col < width; col++)
                    {
                        var v1 = _data[row, col];
                        var v2 = (v1 + b) * a;
                        rowptr[col] = (byte)v2;
                    }
                }
                bmp.UnlockBits(bmpdata);
                var p = bmp.Palette;
                for (var i = 0; i < 256; i++)
                    p.Entries[i] = Color.FromArgb(i, i, i);
                bmp.Palette = p;
                bmp.Save(cached_image_path, ImageFormat.Png);
                return _cached_raw_image = bmp;
            }
        }

        public Bitmap CachedProjectedImage
        {
            get
            {
                if (_cached_projected_image != null) return _cached_projected_image;
                var path = CachedProjectedImagePath;
                if (File.Exists(path))
                {
                    _cached_projected_image = Image.FromFile(path) as Bitmap;
                    return _cached_projected_image;
                }                    
                return null;
            }
        }

        public unsafe LitPatches LitPatches
        {
            get
            {
                if (_lit_patches != null) return _lit_patches;
                var bmp = CachedProjectedImage;
                Debug.Assert(bmp.PixelFormat == PixelFormat.Format8bppIndexed);
                var height = bmp.Height;
                var width = bmp.Width;
                var lit_patches = new LitPatches(width, height);
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                for (var row = 0; row < height; row++)
                {
                    var rowptr = (byte*)(bmpdata.Scan0 + bmpdata.Stride * row);
                    for (var col = 0; col < width; col++)
                    {
                        var v = rowptr[col];
                        const int threshold = (int)(255 * .1);
                        if (v >= threshold)
                            lit_patches[row, col] = true;
                    }
                }
                bmp.UnlockBits(bmpdata);
                return _lit_patches = lit_patches;
            }
        }

        public unsafe static (float[,], double, bool) ReadGeotiffAsFloatArray(string path, float[,] buffer = null)
        {
            using (var ds = Gdal.Open(path, Access.GA_ReadOnly))
            {
                var height = ds.RasterYSize;
                var width = ds.RasterXSize;
                if (buffer == null || buffer.GetLength(0) != height || buffer.GetLength(1) != width)
                    buffer = new float[height, width];
                fixed (float* ary_ptr = &buffer[0, 0])
                {
                    var iptr = new IntPtr(ary_ptr);
                    var band = ds.GetRasterBand(1);
                    band.GetNoDataValue(out double band_no_data, out int hasval);
                    band.ReadRaster(0, 0, width, height, iptr, width, height, DataType.GDT_Float32, 0, 0);
                    return (buffer, band_no_data, hasval != 0);
                }
            }
        }

        public unsafe bool ProjectToPolarBoundingBox(Rectangle bounds)
        {
            const int reduce_resolution = 2;
            var width = bounds.Width / reduce_resolution;
            var height = bounds.Height / reduce_resolution;
            //Console.WriteLine($"w={width} h={height}");
            var path = CachedProjectedImagePath;
            File.Delete(path);

            using (var src_ds = Gdal.Open(FullName, Access.GA_ReadOnly))
            using (var dst_ds = GetMemDriver().Create(Path.ChangeExtension(path, ".tif"), width, height, 1, DataType.GDT_Float32, null))
            {
                var transform = GetAffineTransformOfBounds(bounds);
                transform[1] *= reduce_resolution;
                transform[5] *= reduce_resolution;
                dst_ds.SetGeoTransform(transform);
                dst_ds.SetProjection(PolarWkt);
                Gdal.ReprojectImage(src_ds, dst_ds, src_ds.GetProjection(), PolarWkt, ResampleAlg.GRA_Bilinear, 0d, 0d, null, null);

                var buffer = GetData(dst_ds);
                var (min, max) = GetMinMax(dst_ds, buffer);
                if (min == 0d && max == 0d)
                {
                    Console.WriteLine($"ignoring {FullName}");
                    return false;
                }


                //if (Name.Equals("M156229718LE.cal.echo.low_res_map.tif"))
                //{
                //    Console.WriteLine("here");
                //    var (src_min, src_max) = GetMinMax(src_ds);
                //}

                //src_ds.GetRasterBand(1).GetNoDataValue(out double band_no_data, out int hasval);

                // Map [min,max]->[0,200]
                var a = 200f / (max - min);
                var b = -min;
                var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                for (var row = 0; row < height; row++)
                {
                    var rowptr = (byte*)(bmpdata.Scan0 + bmpdata.Stride * row);
                    for (var col = 0; col < width; col++)
                    {
                        var v1 = buffer[row, col];
                        var v2 = (v1 + b) * a;
                        //rowptr[col] = (byte)v2;
                        if (v1 == 0f)
                            rowptr[col] = (byte)0;  // Hack.  I'm not sure whether this is correct.
                        else
                            rowptr[col] = (byte)v2;
                    }
                }
                bmp.UnlockBits(bmpdata);
                var p = bmp.Palette;
                for (var i = 0; i < 256; i++)
                    p.Entries[i] = Color.FromArgb(i, i, i);
                bmp.Palette = p;
                bmp.Save(path, ImageFormat.Png);
                return true;
            }

            (float, float) GetMinMax(Dataset d, float[,] buffer = null)
            {
                if (buffer == null) buffer = GetData(d, buffer);
                var d_height = buffer.GetLength(0);
                var d_width = buffer.GetLength(1);

                var min = float.MaxValue;
                var max = float.MinValue;
                for (var row = 0; row < d_height; row++)
                    for (var col = 0; col < d_width; col++)
                    {
                        var v = buffer[row, col];
                        min = Math.Min(min, v);
                        max = Math.Max(max, v);
                    }
                return (min, max);
            }

            float[,] GetData(Dataset d, float[,] buffer = null)
            {
                var d_height = d.RasterYSize;
                var d_width = d.RasterXSize;
                if (buffer == null || buffer.GetLength(0) != d_height || buffer.GetLength(1) != d_width)
                    buffer = new float[d_height, d_width];
                fixed (float* ary_ptr = &buffer[0, 0])
                {
                    var iptr = new IntPtr(ary_ptr);
                    var band = d.GetRasterBand(1);
                    band.GetNoDataValue(out double band_no_data, out int hasval);
                    band.ReadRaster(0, 0, d_width, d_height, iptr, d_width, d_height, DataType.GDT_Float32, 0, 0);
                }
                return buffer;
            }
        }

        public unsafe void ProjectToPolarBoundingBox2(Rectangle bounds)
        {
            var width = bounds.Width;
            var height = bounds.Height;
            Console.WriteLine($"w={width} h={height}");
            var path = CachedProjectedImagePath;
            File.Delete(path);

            using (var src_ds = Gdal.Open(FullName, Access.GA_ReadOnly))
            {
                var src_spatial = new OSGeo.OSR.SpatialReference(src_ds.GetProjection());
                var dst_spatial = new OSGeo.OSR.SpatialReference(PolarWkt2);
                var tx = new OSGeo.OSR.CoordinateTransformation(src_spatial, dst_spatial);

                var geo_t = new double[6];
                src_ds.GetGeoTransform(geo_t);
                var x_size = src_ds.RasterXSize;
                var y_size = src_ds.RasterYSize;

                double[] ul = new double[3], lr = new double[3];
                tx.TransformPoint(ul, geo_t[0], geo_t[3], 0d);
                tx.TransformPoint(lr, geo_t[0] + geo_t[1] * x_size, geo_t[3] + geo_t[5] * y_size, 0d);

                var src_data_type = src_ds.GetRasterBand(1).DataType;

                using (var dst_ds = GetGeotiffDriver().Create(Path.ChangeExtension(path, ".tif"), width, height, 1, DataType.GDT_Float32, null))
                {
                    var transform = GetAffineTransformOfBounds(bounds);
                    dst_ds.SetGeoTransform(transform);
                    dst_ds.SetProjection(PolarWkt);
                    Gdal.ReprojectImage(src_ds, dst_ds, src_ds.GetProjection(), PolarWkt, ResampleAlg.GRA_Bilinear, 0d, 0d, null, null);

                    var (src_min, src_max) = GetMinMax(src_ds);
                    var (dst_min, dst_max) = GetMinMax(dst_ds);
                    Console.WriteLine($"src min={src_min} max={src_max}  dst min={dst_min} max={dst_max}");

                    //using (var png_ds = GetPngDriver().CreateCopy(path, dst_ds, 1, null, null, null)) { }
                }
            }

            (float, float) GetMinMax(Dataset d)
            {
                var d_height = d.RasterYSize;
                var d_width = d.RasterXSize;
                float[,] buffer = null;
                if (buffer == null || buffer.GetLength(0) != d_height || buffer.GetLength(1) != d_width)
                    buffer = new float[d_height, d_width];
                fixed (float* ary_ptr = &buffer[0, 0])
                {
                    var iptr = new IntPtr(ary_ptr);
                    var band = d.GetRasterBand(1);
                    band.GetNoDataValue(out double band_no_data, out int hasval);
                    band.ReadRaster(0, 0, d_width, d_height, iptr, d_width, d_height, DataType.GDT_Float32, 0, 0);

                    var min = float.MaxValue;
                    var max = float.MinValue;
                    for (var row = 0; row < d_height; row++)
                        for (var col = 0; col < d_width; col++)
                        {
                            var v = buffer[row, col];
                            min = Math.Min(min, v);
                            max = Math.Max(max, v);
                        }
                    return (min, max);
                }
            }
        }

        public void ProjectToPolarBoundingBoxExternal(Rectangle r)
        {
            const int metersPerPixel = 20;
            var width = r.Width * (20d / metersPerPixel);
            var height = r.Height * (20d / metersPerPixel);
            var t = GetAffineTransformOfBounds(r);
            var xmin = t[0];
            var xmax = xmin + width * metersPerPixel;
            var ymax = t[3];
            var ymin = ymax - height * metersPerPixel;

            var of = "GTIFF";
            var compression = "-co COMPRESS=JPEG -co TILED=YES";
            var src_path = FullName;
            var dst_path = CachedProjectedImagePath;

            var cmd = $"gdalwarp -overwrite -te {xmin} {ymin} {xmax} {ymax} -tr {metersPerPixel} {metersPerPixel} -of {of} {compression} {src_path} {dst_path}";
            Console.WriteLine(cmd);
            RunGDALCommand(cmd);
        }

        public static void RunGDALCommand(string cmd)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            process.StandardInput.WriteLine("c:/Apps/gdal/SDKShell.bat");
            process.StandardInput.WriteLine(cmd);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            var std_msgs = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            Console.WriteLine(std_msgs);
            var err_msgs = process.StandardError.ReadToEnd();
            Console.WriteLine($"stderr: {err_msgs}");
        }

        public static double[] GetAffineTransformOfBounds(Rectangle bounds)
        {
            var r = GetGeoExtent(bounds);
            return new double[] { r.Left, 20d, 0d, r.Top, 0d, -20d };
        }

        public static RectangleD GetGeoExtent(Rectangle r)
        {
            var ul = PixelToCoordinate(r.Top, r.Left);  // line,sample
            var lr = PixelToCoordinate(r.Bottom, r.Right);
            return new RectangleD(ul, lr);
        }

        public static PointD PixelToCoordinate(int line, int sample)
        {
            const double S0 = 15199.5d;             // PDS SAMPLE_PROJECTION_OFFSET
            const double L0 = 15199.5d;             // PDS LINE_PROJECTION_OFFSET
            const double Scale = 20d;

            var x = (sample - S0) * Scale;
            var y = (L0 - line) * Scale;
            return new PointD(x, y);
        }

        #region Utilities

        Driver GetPngDriver()
        {
            if (_png_driver != null) return _png_driver;
            _png_driver = Gdal.GetDriverByName("PNG");
            Debug.Assert(_png_driver != null);
            return _png_driver;
        }

        Driver GetGeotiffDriver()
        {
            if (_geotiff_driver != null) return _geotiff_driver;
            _geotiff_driver = Gdal.GetDriverByName("GTiff");
            Debug.Assert(_geotiff_driver != null);
            return _geotiff_driver;
        }

        Driver GetMemDriver()
        {
            if (_mem_driver != null) return _mem_driver;
            _mem_driver = Gdal.GetDriverByName("MEM");
            Debug.Assert(_mem_driver != null);
            return _mem_driver;
        }

        public void EmptyCaches()
        {
            _cached_raw_image?.Dispose();
            _cached_raw_image = null;
            _data = null;
            _cached_projected_image?.Dispose();
            _cached_projected_image = null;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EmptyCaches();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SourceImage()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
