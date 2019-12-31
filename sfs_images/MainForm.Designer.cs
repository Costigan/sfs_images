namespace sfs_images
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabOriginals = new System.Windows.Forms.TabPage();
            this.pnlImageOuter = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabCoveringSet = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.udMinimumCover = new System.Windows.Forms.NumericUpDown();
            this.btnGenerateCover = new System.Windows.Forms.Button();
            this.zedPlotScoreHistory = new ZedGraph.ZedGraphControl();
            this.lblCoveringCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlCovering = new System.Windows.Forms.Panel();
            this.pbProjected = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.lvProjected = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectedImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSizesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkProjectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateBoundingBoxInProjectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listProjectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToSiteBoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coveringSetToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testLoadimgFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabOriginals.SuspendLayout();
            this.tabCoveringSet.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMinimumCover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbProjected)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabOriginals);
            this.tabControl1.Controls.Add(this.tabCoveringSet);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(943, 509);
            this.tabControl1.TabIndex = 1;
            // 
            // tabOriginals
            // 
            this.tabOriginals.Controls.Add(this.pnlImageOuter);
            this.tabOriginals.Controls.Add(this.splitter1);
            this.tabOriginals.Controls.Add(this.lvFiles);
            this.tabOriginals.Location = new System.Drawing.Point(4, 22);
            this.tabOriginals.Name = "tabOriginals";
            this.tabOriginals.Padding = new System.Windows.Forms.Padding(3);
            this.tabOriginals.Size = new System.Drawing.Size(935, 483);
            this.tabOriginals.TabIndex = 0;
            this.tabOriginals.Text = "Originals";
            this.tabOriginals.UseVisualStyleBackColor = true;
            // 
            // pnlImageOuter
            // 
            this.pnlImageOuter.AutoScroll = true;
            this.pnlImageOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImageOuter.Location = new System.Drawing.Point(202, 3);
            this.pnlImageOuter.Name = "pnlImageOuter";
            this.pnlImageOuter.Size = new System.Drawing.Size(730, 477);
            this.pnlImageOuter.TabIndex = 4;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(199, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 477);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // lvFiles
            // 
            this.lvFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.GridLines = true;
            this.lvFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvFiles.HideSelection = false;
            this.lvFiles.Location = new System.Drawing.Point(3, 3);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(196, 477);
            this.lvFiles.TabIndex = 3;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.SelectedIndexChanged += new System.EventHandler(this.lvFiles_SelectedIndexChanged);
            this.lvFiles.SizeChanged += new System.EventHandler(this.lvFiles_SizeChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // tabCoveringSet
            // 
            this.tabCoveringSet.Controls.Add(this.panel1);
            this.tabCoveringSet.Controls.Add(this.splitter2);
            this.tabCoveringSet.Controls.Add(this.lvProjected);
            this.tabCoveringSet.Location = new System.Drawing.Point(4, 22);
            this.tabCoveringSet.Name = "tabCoveringSet";
            this.tabCoveringSet.Size = new System.Drawing.Size(935, 483);
            this.tabCoveringSet.TabIndex = 2;
            this.tabCoveringSet.Text = "Covering Set";
            this.tabCoveringSet.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.udMinimumCover);
            this.panel1.Controls.Add(this.btnGenerateCover);
            this.panel1.Controls.Add(this.zedPlotScoreHistory);
            this.panel1.Controls.Add(this.lblCoveringCount);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.pnlCovering);
            this.panel1.Controls.Add(this.pbProjected);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(173, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(762, 483);
            this.panel1.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(300, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Minimum Cover";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udMinimumCover
            // 
            this.udMinimumCover.Location = new System.Drawing.Point(385, 55);
            this.udMinimumCover.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udMinimumCover.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udMinimumCover.Name = "udMinimumCover";
            this.udMinimumCover.Size = new System.Drawing.Size(48, 20);
            this.udMinimumCover.TabIndex = 8;
            this.udMinimumCover.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // btnGenerateCover
            // 
            this.btnGenerateCover.Location = new System.Drawing.Point(303, 26);
            this.btnGenerateCover.Name = "btnGenerateCover";
            this.btnGenerateCover.Size = new System.Drawing.Size(130, 23);
            this.btnGenerateCover.TabIndex = 7;
            this.btnGenerateCover.Text = "Generate Covering Set";
            this.btnGenerateCover.UseVisualStyleBackColor = true;
            this.btnGenerateCover.Click += new System.EventHandler(this.btnGenerateCover_Click);
            // 
            // zedPlotScoreHistory
            // 
            this.zedPlotScoreHistory.Location = new System.Drawing.Point(11, 232);
            this.zedPlotScoreHistory.Name = "zedPlotScoreHistory";
            this.zedPlotScoreHistory.ScrollGrace = 0D;
            this.zedPlotScoreHistory.ScrollMaxX = 0D;
            this.zedPlotScoreHistory.ScrollMaxY = 0D;
            this.zedPlotScoreHistory.ScrollMaxY2 = 0D;
            this.zedPlotScoreHistory.ScrollMinX = 0D;
            this.zedPlotScoreHistory.ScrollMinY = 0D;
            this.zedPlotScoreHistory.ScrollMinY2 = 0D;
            this.zedPlotScoreHistory.Size = new System.Drawing.Size(706, 222);
            this.zedPlotScoreHistory.TabIndex = 6;
            this.zedPlotScoreHistory.UseExtendedPrintDialog = true;
            // 
            // lblCoveringCount
            // 
            this.lblCoveringCount.AutoSize = true;
            this.lblCoveringCount.Location = new System.Drawing.Point(145, 157);
            this.lblCoveringCount.Name = "lblCoveringCount";
            this.lblCoveringCount.Size = new System.Drawing.Size(97, 13);
            this.lblCoveringCount.TabIndex = 5;
            this.lblCoveringCount.Text = "<count of lit pixels>";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(145, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Covering";
            // 
            // pnlCovering
            // 
            this.pnlCovering.Location = new System.Drawing.Point(148, 26);
            this.pnlCovering.Name = "pnlCovering";
            this.pnlCovering.Size = new System.Drawing.Size(128, 128);
            this.pnlCovering.TabIndex = 4;
            this.pnlCovering.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCovering_Paint);
            // 
            // pbProjected
            // 
            this.pbProjected.Location = new System.Drawing.Point(11, 26);
            this.pbProjected.Name = "pbProjected";
            this.pbProjected.Size = new System.Drawing.Size(128, 128);
            this.pbProjected.TabIndex = 2;
            this.pbProjected.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Projected Image";
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(170, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 483);
            this.splitter2.TabIndex = 1;
            this.splitter2.TabStop = false;
            // 
            // lvProjected
            // 
            this.lvProjected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvProjected.Dock = System.Windows.Forms.DockStyle.Left;
            this.lvProjected.FullRowSelect = true;
            this.lvProjected.GridLines = true;
            this.lvProjected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvProjected.HideSelection = false;
            this.lvProjected.Location = new System.Drawing.Point(0, 0);
            this.lvProjected.Name = "lvProjected";
            this.lvProjected.Size = new System.Drawing.Size(170, 483);
            this.lvProjected.TabIndex = 0;
            this.lvProjected.UseCompatibleStateImageBehavior = false;
            this.lvProjected.View = System.Windows.Forms.View.Details;
            this.lvProjected.SelectedIndexChanged += new System.EventHandler(this.lvProjected_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 600;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.testToolStripMenuItem,
            this.coveringSetToolStripMenuItem1,
            this.testToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(943, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rawImageToolStripMenuItem,
            this.projectedImageToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // rawImageToolStripMenuItem
            // 
            this.rawImageToolStripMenuItem.Checked = true;
            this.rawImageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rawImageToolStripMenuItem.Name = "rawImageToolStripMenuItem";
            this.rawImageToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.rawImageToolStripMenuItem.Text = "Raw Image";
            this.rawImageToolStripMenuItem.Click += new System.EventHandler(this.rawImageToolStripMenuItem_Click);
            // 
            // projectedImageToolStripMenuItem
            // 
            this.projectedImageToolStripMenuItem.Name = "projectedImageToolStripMenuItem";
            this.projectedImageToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.projectedImageToolStripMenuItem.Text = "Projected Image";
            this.projectedImageToolStripMenuItem.Click += new System.EventHandler(this.projectedImageToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkSizesToolStripMenuItem,
            this.checkProjectionsToolStripMenuItem,
            this.generateBoundingBoxInProjectionToolStripMenuItem,
            this.listProjectionsToolStripMenuItem,
            this.projectToSiteBoundsToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.testToolStripMenuItem.Text = "Originals";
            // 
            // checkSizesToolStripMenuItem
            // 
            this.checkSizesToolStripMenuItem.Name = "checkSizesToolStripMenuItem";
            this.checkSizesToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.checkSizesToolStripMenuItem.Text = "Check Sizes";
            this.checkSizesToolStripMenuItem.Click += new System.EventHandler(this.checkSizesToolStripMenuItem_Click);
            // 
            // checkProjectionsToolStripMenuItem
            // 
            this.checkProjectionsToolStripMenuItem.Name = "checkProjectionsToolStripMenuItem";
            this.checkProjectionsToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.checkProjectionsToolStripMenuItem.Text = "Check Projections";
            this.checkProjectionsToolStripMenuItem.Click += new System.EventHandler(this.checkProjectionsToolStripMenuItem_Click);
            // 
            // generateBoundingBoxInProjectionToolStripMenuItem
            // 
            this.generateBoundingBoxInProjectionToolStripMenuItem.Name = "generateBoundingBoxInProjectionToolStripMenuItem";
            this.generateBoundingBoxInProjectionToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.generateBoundingBoxInProjectionToolStripMenuItem.Text = "Generate Bounding Box in Projection";
            this.generateBoundingBoxInProjectionToolStripMenuItem.Click += new System.EventHandler(this.generateBoundingBoxInProjectionToolStripMenuItem_Click);
            // 
            // listProjectionsToolStripMenuItem
            // 
            this.listProjectionsToolStripMenuItem.Name = "listProjectionsToolStripMenuItem";
            this.listProjectionsToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.listProjectionsToolStripMenuItem.Text = "List Projections";
            this.listProjectionsToolStripMenuItem.Click += new System.EventHandler(this.listProjectionsToolStripMenuItem_Click);
            // 
            // projectToSiteBoundsToolStripMenuItem
            // 
            this.projectToSiteBoundsToolStripMenuItem.Name = "projectToSiteBoundsToolStripMenuItem";
            this.projectToSiteBoundsToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.projectToSiteBoundsToolStripMenuItem.Text = "Project to Site Bounds";
            this.projectToSiteBoundsToolStripMenuItem.Click += new System.EventHandler(this.projectToSiteBoundsToolStripMenuItem_Click);
            // 
            // coveringSetToolStripMenuItem1
            // 
            this.coveringSetToolStripMenuItem1.Name = "coveringSetToolStripMenuItem1";
            this.coveringSetToolStripMenuItem1.Size = new System.Drawing.Size(86, 20);
            this.coveringSetToolStripMenuItem1.Text = "Covering Set";
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testLoadimgFileToolStripMenuItem});
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(40, 20);
            this.testToolStripMenuItem1.Text = "&Test";
            // 
            // testLoadimgFileToolStripMenuItem
            // 
            this.testLoadimgFileToolStripMenuItem.Name = "testLoadimgFileToolStripMenuItem";
            this.testLoadimgFileToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.testLoadimgFileToolStripMenuItem.Text = "Test load .img file";
            this.testLoadimgFileToolStripMenuItem.Click += new System.EventHandler(this.testLoadimgFileToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 533);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "SfS Images";
            this.tabControl1.ResumeLayout(false);
            this.tabOriginals.ResumeLayout(false);
            this.tabCoveringSet.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMinimumCover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbProjected)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabOriginals;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel pnlImageOuter;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkSizesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkProjectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateBoundingBoxInProjectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listProjectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectToSiteBoundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectedImageToolStripMenuItem;
        private System.Windows.Forms.TabPage tabCoveringSet;
        private System.Windows.Forms.ToolStripMenuItem coveringSetToolStripMenuItem1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ListView lvProjected;
        private System.Windows.Forms.Panel pnlCovering;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbProjected;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCoveringCount;
        private ZedGraph.ZedGraphControl zedPlotScoreHistory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown udMinimumCover;
        private System.Windows.Forms.Button btnGenerateCover;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem testLoadimgFileToolStripMenuItem;
    }
}

