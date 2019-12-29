using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace sfs_images
{
    public partial class MainForm : Form
    {
        public static string ImageDirectory = @"C:\projects\sfs_images\nobile\med";
        public static string TempDirectory = @"tmp";

        protected SingleSourceImageBox ImageBox;

        public MainForm()
        {
            InitializeComponent();
            if (!util.GeotiffHelper.GetGdalDrivers())
                throw new Exception("Couldn't get GDAL drivers");
            pnlImageOuter.Controls.Add(ImageBox = new SingleSourceImageBox { Location = new Point(0, 0), Size = new Size(100,100) });
            Directory.CreateDirectory(TempDirectory);
            lvFiles_SizeChanged(null, null);
            LoadImages(ImageDirectory);
        }

        void LoadImages(string imageDirectory)
        {
            var image_files = new DirectoryInfo(imageDirectory).EnumerateFiles("*.tif").Select(fi =>
            {
                var i = new SourceImage(fi);
                return new ListViewItem { Text = fi.Name, Tag = i };
            }).OrderBy(l => l.Name).ToArray();
            lvFiles.Items.AddRange(image_files);
        }

        private void lvFiles_SizeChanged(object sender, EventArgs e)
        {
            lvFiles.Columns[0].Width = lvFiles.Width;
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var items = lvFiles.SelectedItems;
            if (items.Count != 1)
            {
                SetImage(null);
                return;
            }
            var image_file = items[0].Tag as SourceImage;
            if (image_file == null)
            {
                SetImage(null);
                return;
            }
            SetImage(image_file.FullName);
        }

        float[,] _temp_buffer = null;

        unsafe void SetImage(string path)
        {
            if (ImageBox.Image != null)
                ImageBox.Image.Dispose();
            ImageBox.Image = null;
            if (path == null)
                return;
            ImageBox.Image = new SourceImage(path);
        }        
    }

    public class SourceImage : IDisposable
    {
        protected Bitmap _cached_image = null;
        protected Size? _size;
        protected float[,] _data = null;

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

        public override string ToString() => Name;

        public string CachedImagePath => Path.Combine(MainForm.TempDirectory, Path.ChangeExtension(Path.GetFileName(FullName), ".png"));

        public Size Size
        {
            get
            {
                if (_size.HasValue)
                    return _size.Value;
                if (_cached_image != null)
                    return _cached_image.Size;
                using (var ds = Gdal.Open(FullName, Access.GA_ReadOnly))  // Path might be null
                {
                    _size = new Size(ds.RasterXSize, ds.RasterYSize);
                    return _size.Value;
                }
            }
        }

        public unsafe Bitmap CachedImage
        {
            get
            {
                if (_cached_image != null)
                    return _cached_image;

                var cached_image_path = CachedImagePath;
                if (File.Exists(cached_image_path))
                    return _cached_image = Image.FromFile(cached_image_path) as Bitmap;

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
                return _cached_image = bmp;
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cached_image?.Dispose();
                    _cached_image = null;
                    _data = null;
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

    public class SingleSourceImageBox : Control
    {
        protected SourceImage _image = null; 

        public SingleSourceImageBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                base.OnPaint(e);
                return;
            }
            if (_image==null)
            {
                e.Graphics.FillRectangle(Brushes.Red, 0, 0, 20000, 20000);
                return;
            }
            e.Graphics.DrawImage(_image.CachedImage, 0, 0);
        }

        public SourceImage Image
        {
            get { return _image; }
            set
            {
                _image?.Dispose();
                _image = value;
                if (_image!=null)
                    Size = _image.Size;
                Invalidate();
            }
        }
    }
}
