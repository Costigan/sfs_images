using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace sfs_images.util
{
    public static class Extensions
    {
        public static float DistanceTo(this Point a, Point b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static float DistanceTo(this PointF a, PointF b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static Form GetParentForm(this Control c)
        {
            while (c != null && !(c is Form))
                c = c.Parent;
            return c as Form;
        }

        public static PointF ToPointF(this Point a) => new PointF(a.X.ToCoord(), a.Y.ToCoord());
        public static PointF ToPointF(this PointD a) => new PointF((float)a.X, (float)a.Y);

        public static Point ToPoint(this PointF p) => new Point(p.X.ToFixed(), p.Y.ToFixed());

        public static PointF Add(this PointF a, PointF b) => new PointF(a.X + b.X, a.Y + b.Y);

        public static PointF Mult(this PointF p, float f) => new PointF(p.X * f, p.Y * f);

        public static float ToCoord(this int i) => i;
        public static int ToFixed(this float f) => (int)(f + 0.5f);

        public static int Rem(this int a, int b)
        {
            Math.DivRem(a, b, out int result);
            return result;
        }

        public static Regex LatitudeRegex = new Regex("^\\s*(\\d+)d\\s*(\\d+)'\\s*(\\d*\\.\\d*)\"([NS])$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

        public static Regex LongitudeRegex = new Regex("^\\s*(\\d+)d\\s*(\\d+)'\\s*(\\d*\\.\\d*)\"([EW])$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

        public static bool TryToLatitude(this string s, out double r)
        {
            r = 0d;
            if (double.TryParse(s, out r)) return true;
            var match = LatitudeRegex.Match(s);
            if (!match.Success) return false;
            System.Diagnostics.Debug.Assert(match.Groups.Count == 5);
            var deg = match.Groups[1].Value;
            var min = match.Groups[2].Value;
            var sec = match.Groups[3].Value;
            var dir = match.Groups[4].Value;
            if (!int.TryParse(deg, out int degi) || !int.TryParse(min, out int mini) || !double.TryParse(sec, out double secd)) return false;
            r = degi + mini / 60d + secd / 3600d;
            if (dir.ToUpperInvariant().Equals("S"))
                r = -r;
            if (-90d <= r && r <= 90d) return true;
            r = 0d;
            return false;
        }

        public static bool TryToLongitude(this string s, out double r)
        {
            r = 0d;
            if (double.TryParse(s, out r)) return true;
            var match = LongitudeRegex.Match(s);
            if (!match.Success) return false;
            System.Diagnostics.Debug.Assert(match.Groups.Count == 5);
            var deg = match.Groups[1].Value;
            var min = match.Groups[2].Value;
            var sec = match.Groups[3].Value;
            var dir = match.Groups[4].Value;
            if (!int.TryParse(deg, out int degi) || !int.TryParse(min, out int mini) || !double.TryParse(sec, out double secd)) return false;
            r = degi + mini / 60d + secd / 3600d;
            if (dir.ToUpperInvariant().Equals("W"))
                r = -r;
            if (-180d <= r && r <= 180d) return true;
            r = 0d;
            return false;
        }

        public static IEnumerable<T> Enumerate<T>(this T[,] data) where T : struct
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    yield return data[y, x];
        }

        public static int Round(double v) => (int)Math.Round(v);

        public static Rectangle ToRectangle(this RectangleF r) => new Rectangle(Round(r.Left), Round(r.Top), Round(r.Width), Round(r.Height));
        public static RectangleF ToRectangleF(this Rectangle r) => new RectangleF(r.Left, r.Top, r.Width, r.Height);

        public static Point Center(this Rectangle r) => new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
        public static Rectangle SpanFrom(this Point c, Point span) => new Rectangle(c.X - span.X, c.Y - span.Y, span.X * 2 + 1, span.Y * 2 + 1);

        public static Rectangle Intersection(this Rectangle a, Rectangle b)
        {
            if (!a.IntersectsWith(b))
                return (new Rectangle(a.X, a.Y, 0, 0));
            var x = Math.Max(a.X, b.X);
            var y = Math.Max(a.Y, b.Y);
            var bottom = Math.Min(a.Bottom, b.Bottom);
            var right = Math.Min(a.Right, b.Right);
            return new Rectangle(x, y, right - x, bottom - y);
        }

        public static string FilenameAppend(this string path, string postfix, string extension = null) => Path.Combine(Path.GetDirectoryName(path) ?? ".", Path.GetFileNameWithoutExtension(path) + postfix + (extension ?? Path.GetExtension(path)));

        public unsafe static void SetPixels8Bit(this Bitmap target, Func<int, int, byte> func)
        {
            if (target.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new Exception("The bitmap is not Format8bppIndexed");
            int width = target.Width, height = target.Height;
            var bmpdata = target.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, target.PixelFormat);
            for (var row = 0; row < height; row++)
            {
                var rowptr = (byte*)(bmpdata.Scan0 + row * bmpdata.Stride);
                for (var col = 0; col < width; col++)
                    rowptr[col] = func(row, col);
            }
            target.UnlockBits(bmpdata);
        }

        public unsafe static void CombineInto(this Bitmap target, Bitmap source, Point offset, Func<int, int, byte, byte, byte> func)
        {
            if (target.PixelFormat != PixelFormat.Format8bppIndexed || source.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new Exception("The bitmap is not Format8bppIndexed");
            int target_width = target.Width, target_height = target.Height, source_width = source.Width, source_height = source.Height;
            var target_data = target.LockBits(new Rectangle(0, 0, target_width, target_height), ImageLockMode.ReadWrite, target.PixelFormat);
            var source_data = source.LockBits(new Rectangle(0, 0, source_width, source_height), ImageLockMode.ReadOnly, source.PixelFormat);
            for (var source_row = 0; source_row < source_height; source_row++)
            {
                var target_row = source_row + offset.Y;
                if (target_row < 0 || target_row >= target_height) continue;
                var source_rowptr = (byte*)(source_data.Scan0 + source_row * source_data.Stride);
                var target_rowptr = (byte*)(target_data.Scan0 + target_row * target_data.Stride);
                for (var source_col = 0; source_col < source_width; source_col++)
                {
                    var target_col = source_col + offset.X;
                    if (target_col < 0 || target_col >= target_width) continue;
                    target_rowptr[target_col] = func(source_row, source_col, source_rowptr[source_col], target_rowptr[target_col]);
                }
            }
            source.UnlockBits(source_data);
            target.UnlockBits(target_data);
        }

        public static byte[] ToByteArray(this Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format8bppIndexed)
                return null;
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            var length = bitmapData.Stride * bitmapData.Height;
            var bytes = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
            bmp.UnlockBits(bitmapData);
            return bytes;
        }

        public static unsafe byte[,] ToByteArray2(this Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format8bppIndexed)
                return null;
            var height = bmp.Height;
            var width = bmp.Width;
            var bytes = new byte[height, width];
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            for (var row = 0; row < height; row++)
            {
                var rowptr = (byte*)(bmpdata.Scan0 + row * bmpdata.Stride);
                for (var col = 0; col < width; col++)
                    bytes[row, col] = rowptr[col];
            }
            bmp.UnlockBits(bmpdata);
            return bytes;
        }

        public static unsafe int[,] ToIntArray2(this Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
                return null;
            var height = bmp.Height;
            var width = bmp.Width;
            var result = new int[height, width];
            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            for (var row = 0; row < height; row++)
            {
                var rowptr = (int*)(bmpdata.Scan0 + row * bmpdata.Stride);
                for (var col = 0; col < width; col++)
                    result[row, col] = rowptr[col];
            }
            bmp.UnlockBits(bmpdata);
            return result;
        }

        public static float[,] Mult(this byte[,] a, float f)
        {
            var height = a.GetLength(0);
            var width = a.GetLength(1);
            var r = new float[height, width];
            for (var row = 0; row < height; row++)
                for (var col = 0; col < width; col++)
                    r[row, col] = f * a[row, col];
            return r;
        }

        public static IEnumerable<byte> EnumerateBytes(this Bitmap bmp) => bmp.ToByteArray();

        public static long[] GetByteHistogram(this Bitmap bmp)
        {
            var buf = new long[256];
            foreach (var b in bmp.EnumerateBytes())
                buf[b]++;
            return buf;
        }

        public static (int, int) GetMinMax(this Bitmap bmp)
        {
            var max = int.MinValue;
            var min = int.MaxValue;
            foreach (var v in bmp.EnumerateBytes())
            {
                max = Math.Max(max, v);
                min = Math.Min(min, v);
            }
            return (min, max);
        }

    }

    public struct RectangleD
    {
        public RectangleD(double x, double y, double w, double h) { X = x; Y = y; Width = w; Height = h; }
        public RectangleD(PointD ul, PointD lr) { X = ul.X; Y = ul.Y; Width = lr.X - ul.X; Height = lr.Y - ul.Y; }
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public double Left => X;
        public double Top => Y;
        public double Right => X + Width;
        public double Bottom => Y + Height;
    }

    public struct PointD
    {
        public PointD(double x, double y) { X = x; Y = y; }
        public double X;
        public double Y;
    }
}
