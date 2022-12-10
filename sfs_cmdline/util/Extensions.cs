using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace sfs_cmdline.util
{
    public static class Extensions
    {
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

        public static string FilenameAppend(this string path, string postfix, string extension = null) => Path.Combine(Path.GetDirectoryName(path) ?? ".", Path.GetFileNameWithoutExtension(path) + postfix + (extension ?? Path.GetExtension(path)));

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
