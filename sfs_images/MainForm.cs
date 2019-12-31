using OSGeo.GDAL;
using sfs_images.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace sfs_images
{
    public partial class MainForm : Form
    {
        public static string ImageDirectory = @"C:\projects\sfs_images\nobile\low";
        public static string TempDirectory = @"tmp";
        public static string ProjectedDirectory = @"projected";

        protected LitPatches CoveringSet = null;

        public const int DefaultPatchSize = 32;

        // "Top": 9216.0,
        // "Left": 18688.0,
        // "Bottom": 9472.0,
        // "Right": 18944.0,
        public Rectangle NobileBounds = new Rectangle(18688, 9216, 18944 - 18688, 9472 - 9216);

        protected SingleSourceImageBox ImageBox;
        protected LitPatches _lit_patches = null;

        public MainForm()
        {
            InitializeComponent();
            if (!util.GeotiffHelper.GetGdalDrivers())
                throw new Exception("Couldn't get GDAL drivers");
            pnlImageOuter.Controls.Add(ImageBox = new SingleSourceImageBox { Location = new Point(0, 0), Size = new Size(100,100) });
            Directory.CreateDirectory(TempDirectory);
            Directory.CreateDirectory(ProjectedDirectory);
            lvFiles_SizeChanged(null, null);
            LoadImages(ImageDirectory);

            // Test
            //var lp = new LitPatches(10, 10);
            //lp.Test1();
        }

        void LoadImages(string imageDirectory)
        {
            var image_files = new DirectoryInfo(imageDirectory).EnumerateFiles("*.tif").Select(fi =>
            {
                var i = new SourceImage(fi);
                return new ListViewItem { Text = fi.Name, Tag = i };
            }).OrderBy(l => l.Name).ToArray();
            for (var i = 0; i < image_files.Length; i++)
                (image_files[i].Tag as SourceImage).Index = i;
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
                SetOriginalImage(null);
                return;
            }
            var image_file = items[0].Tag as SourceImage;
            if (image_file == null)
            {
                SetOriginalImage(null);
                return;
            }
            SetOriginalImage(image_file.FullName);
        }

        float[,] _temp_buffer = null;

        void SetOriginalImage(string path)
        {
            if (ImageBox.Image != null)
                ImageBox.Image.Dispose();
            ImageBox.Image = null;
            if (path == null)
                return;
            ImageBox.Image = new SourceImage(path);
        }

        void SetProjectedImage(SourceImage si)
        {
            pbProjected.Image = si?.CachedProjectedImage;
        }

        IEnumerable<SourceImage> EnumerateSourceImages()
        {
            foreach (var o in lvFiles.Items)
            {
                if (!(o is ListViewItem lvi)) continue;
                if (!(lvi.Tag is SourceImage si)) continue;
                yield return si;
            }
        }

        private void checkSizesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var si in EnumerateSourceImages())
                Console.WriteLine(si.Size);
        }

        private void checkProjectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string proj = null;
            foreach (var si in EnumerateSourceImages())
            {
                if (proj == null)
                    proj = si.Projection;
                else
                    Debug.Assert(proj.Equals(si.Projection));
            }
        }

        private void generateBoundingBoxInProjectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bb = GetBoundingBox();
            Console.WriteLine($"bb l={bb.Left} t={bb.Top} w={bb.Width} h={bb.Height}");
            var ib = GetInteriorBox();
            Console.WriteLine($"ib l={ib.Left} t={ib.Top} w={ib.Width} h={ib.Height}");
        }

        RectangleF GetBoundingBox()
        {
            var left = float.MaxValue;
            var right = float.MinValue;
            var top = float.MinValue;   // Assume +Y is up
            var bottom = float.MaxValue;
            foreach (var si in EnumerateSourceImages())
            {
                var b = si.BoundsInProjection;
                Debug.Assert(b.Height < 0f);
                left = Math.Min(left, b.Left);
                right = Math.Max(right, b.Right);
                top = Math.Max(top, b.Top);
                bottom = Math.Min(bottom, b.Bottom);
            }
            return new RectangleF(left, top, right - left, top - bottom);
        }

        RectangleF GetInteriorBox()
        {
            var left = float.MinValue;
            var right = float.MaxValue;
            var top = float.MaxValue;   // Assume +Y is up
            var bottom = float.MinValue;
            foreach (var si in EnumerateSourceImages())
            {
                var b = si.BoundsInProjection;
                Debug.Assert(b.Height < 0f);
                left = Math.Max(left, b.Left);
                right = Math.Min(right, b.Right);
                top = Math.Min(top, b.Top);
                bottom = Math.Max(bottom, b.Bottom);
            }
            return new RectangleF(left, top, right - left, top - bottom);
        }

        private void listProjectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var seen = new Dictionary<string, int>();
            foreach (var si in EnumerateSourceImages())
            {
                var proj = si.Projection;
                if (!seen.ContainsKey(proj))
                    seen.Add(proj, 1);
                else
                    seen[proj] += 1;
            }
            foreach (var kv in seen)
                Console.WriteLine($"c={kv.Value} proj={kv.Key}");
        }

        private void projectToSiteBoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var hasprojected = new List<SourceImage>();
            foreach (var si in EnumerateSourceImages())
            {
                si.HasProjectedImage = si.ProjectToPolarBoundingBox(NobileBounds);
                if (!si.HasProjectedImage)
                    lvFiles.Items[si.Index].ForeColor = Color.LightGray;
                else
                    hasprojected.Add(si);
            }
            lvProjected.Items.Clear();
            var toadd = hasprojected.Select(si => new ListViewItem { Text = si.Name, Tag = si }).ToArray();
            lvProjected.Items.AddRange(toadd);
        }

        private void rawImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rawImageToolStripMenuItem.Checked = true;
            projectedImageToolStripMenuItem.Checked = false;
            ImageBox.ShowProjectedImage = projectedImageToolStripMenuItem.Checked;
            foreach (var sv in EnumerateSourceImages()) sv.EmptyCaches();
        }

        private void projectedImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rawImageToolStripMenuItem.Checked = false;
            projectedImageToolStripMenuItem.Checked = true;
            ImageBox.ShowProjectedImage = projectedImageToolStripMenuItem.Checked;
            foreach (var sv in EnumerateSourceImages()) sv.EmptyCaches();
        }

        private void lvProjected_SelectedIndexChanged(object sender, EventArgs e)
        {
            var items = lvProjected.SelectedItems;
            if (items.Count != 1)
            {
                SetProjectedImage(null);
                return;
            }
            var image_file = items[0].Tag as SourceImage;
            if (image_file == null)
            {
                SetProjectedImage(null);
                return;
            }
            SetProjectedImage(image_file);
            SetLitPatches(image_file);
        }

        void SetLitPatches(SourceImage s)
        {
            _lit_patches = s?.LitPatches;
            lblCoveringCount.Text = _lit_patches == null ? "" : (_lit_patches.Count).ToString();
            pnlCovering.Invalidate();
        }

        private void pnlCovering_Paint(object sender, PaintEventArgs e)
        {
            if (_lit_patches == null)
                e.Graphics.FillRectangle(Brushes.White, 0, 0, pnlCovering.Width, pnlCovering.Height);
            else
            {                
                e.Graphics.FillRectangle(Brushes.White, 0, 0, pnlCovering.Width, pnlCovering.Height);
                var width = _lit_patches.Width;
                var height = _lit_patches.Height;
                var xm = pnlCovering.Width / width;
                var ym = pnlCovering.Height / height;
                var hack = 0;
                for (var row = 0; row < height; row++)
                    for (var col = 0; col < width; col++)
                        if (_lit_patches[row, col])
                        {
                            e.Graphics.FillRectangle(Brushes.Red, col * xm, row * ym, xm, ym);
                            hack++;
                        }
                Console.WriteLine($"count={_lit_patches.Count} hack={hack}");
            }
        }

        void btnGenerateCover_Click(object sender, EventArgs e)
        {
            GenerateCover((int)udMinimumCover.Value);
        }

        void GenerateCover(int minimum_cover = 2)
        {
            var list = new LinkedList<SourceImage>(EnumerateSourceImages().Where(s => s.HasProjectedImage));
            if (list.First == null) return;
            var covering_set = new LitPatches(list.First.Value.CachedProjectedImage.Width, list.First.Value.CachedProjectedImage.Height);
            var length = (double)covering_set.Length;
            var candidate = new LitPatches(covering_set.Width, covering_set.Height);
            var count = list.Count;
            var values = new PointPairList();
            for (var i = 0; i < count; i++)
            {
                var current_score = covering_set.Count;
                var best_node_score = current_score;
                Console.WriteLine($"{i:D5}/{count:D5}  current_score={current_score}");
                LinkedListNode<SourceImage> best_node = null;
                var node = list.First;
                while (node != null)
                {
                    var score = Score(node.Value);
                    if (score > best_node_score)
                    {
                        best_node_score = score;
                        best_node = node;
                        Console.WriteLine($"  best_score={best_node_score:D5} file={best_node.Value.Name}");
                    }
                    node = node.Next;
                }
                values.Add(values.Count, current_score / length);
                if (best_node == null)
                    break;
                covering_set.Or(best_node.Value.LitPatches);
                list.Remove(best_node);
            }
            values.Add(values.Count, covering_set.Count / length);

            Console.WriteLine($"values.count={values.Count}");

            // Plot
            var pane = zedPlotScoreHistory.GraphPane;
            pane.CurveList.Clear();
            pane.AddCurve("covering set rise", values, Color.Black, SymbolType.Plus);
            pane.Legend.IsVisible = false;
            pane.XAxis.Title.Text = "Image Count";
            pane.YAxis.Title.Text = "Coverage Fraction";
            pane.Title.IsVisible = false;
            zedPlotScoreHistory.AxisChange();
            zedPlotScoreHistory.Invalidate();


            int Score(SourceImage s)
            {
                candidate.Copy(covering_set);
                candidate.Or(s.LitPatches);
                return candidate.Count;
            }
        }

        private void testLoadimgFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = @"d:/viper/dem_images/nobile_1/M104270814LE.IMG";
            var path1 = @"d:/viper/dem_images/nobile_1/M104270814LE.cal.echo.cub";
            var path2 = @"C:\projects\sfs_images\nobile\low\M104270814LE.cal.echo.low_res_map.tif";
            using (var ds = Gdal.Open(path2, Access.GA_ReadOnly))
            {
                Console.WriteLine(@"opened");
                Console.WriteLine($"x={ds.RasterXSize} y={ds.RasterYSize}");
                Console.WriteLine($"proj={ds.GetProjection()}");
            }
        }
    }

    public class SingleSourceImageBox : Control
    {
        protected SourceImage _image = null;
        protected bool _ShowProjectedImage = false;

        public SingleSourceImageBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        public bool ShowProjectedImage
        {
            get { return _ShowProjectedImage; }
            set { 
                _ShowProjectedImage = value;
                Invalidate();
            }
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
            var bmp = ShowProjectedImage ? _image.CachedProjectedImage : _image.CachedRawImage;
            if (bmp == null)
                e.Graphics.DrawLine(Pens.Red, 0, 0, 100, 100);
            else
                e.Graphics.DrawImage(bmp, 0, 0);
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
