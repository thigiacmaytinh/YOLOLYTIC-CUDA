namespace Yololytic
{
    partial class FormExtract
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
            this.pnCamera1 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.lbl_count = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txt_numThread = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_output = new AltoControls.BrowseDir();
            this.circle1 = new AltoControls.ProcessingControl();
            this.btn_extract = new AltoControls.AltoButton();
            this.txt_inputVideo = new AltoControls.BrowseFile();
            this.label3 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnCamera1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_numThread)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnCamera1
            // 
            this.pnCamera1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnCamera1.Controls.Add(this.pictureBox1);
            this.pnCamera1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCamera1.Location = new System.Drawing.Point(0, 124);
            this.pnCamera1.Name = "pnCamera1";
            this.pnCamera1.Size = new System.Drawing.Size(738, 349);
            this.pnCamera1.TabIndex = 21;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(240)))), ((int)(((byte)(250)))));
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Controls.Add(this.lbl_count);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.txt_numThread);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_output);
            this.panel1.Controls.Add(this.circle1);
            this.panel1.Controls.Add(this.btn_extract);
            this.panel1.Controls.Add(this.txt_inputVideo);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1083, 124);
            this.panel1.TabIndex = 38;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.radioButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.radioButton2.Location = new System.Drawing.Point(863, 51);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(167, 21);
            this.radioButton2.TabIndex = 20;
            this.radioButton2.Text = "Save frame has object";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Visible = false;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.radioButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.radioButton1.Location = new System.Drawing.Point(863, 24);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(116, 21);
            this.radioButton1.TabIndex = 19;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Save all frame";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.Visible = false;
            // 
            // lbl_count
            // 
            this.lbl_count.AutoSize = true;
            this.lbl_count.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lbl_count.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.lbl_count.Location = new System.Drawing.Point(676, 92);
            this.lbl_count.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_count.Name = "lbl_count";
            this.lbl_count.Size = new System.Drawing.Size(47, 19);
            this.lbl_count.TabIndex = 18;
            this.lbl_count.Text = "Count";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(106, 90);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(563, 23);
            this.progressBar1.TabIndex = 17;
            // 
            // txt_numThread
            // 
            this.txt_numThread.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txt_numThread.Location = new System.Drawing.Point(770, 60);
            this.txt_numThread.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.txt_numThread.Name = "txt_numThread";
            this.txt_numThread.Size = new System.Drawing.Size(52, 25);
            this.txt_numThread.TabIndex = 16;
            this.txt_numThread.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txt_numThread.ValueChanged += new System.EventHandler(this.txt_numThread_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.label4.Location = new System.Drawing.Point(676, 63);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 19);
            this.label4.TabIndex = 15;
            this.label4.Text = "Num thread";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.label1.Location = new System.Drawing.Point(8, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 19);
            this.label1.TabIndex = 13;
            this.label1.Text = "Output folder";
            // 
            // txt_output
            // 
            this.txt_output.BackColor = System.Drawing.Color.Transparent;
            this.txt_output.BackgroundColor = System.Drawing.Color.White;
            this.txt_output.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(133)))), ((int)(((byte)(200)))));
            this.txt_output.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txt_output.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txt_output.Location = new System.Drawing.Point(106, 54);
            this.txt_output.Name = "txt_output";
            this.txt_output.Padding = new System.Windows.Forms.Padding(5);
            this.txt_output.Size = new System.Drawing.Size(563, 30);
            this.txt_output.TabIndex = 12;
            this.txt_output.TextChanged += new System.EventHandler(this.txt_output_TextChanged);
            // 
            // circle1
            // 
            this.circle1.BackColor = System.Drawing.Color.Transparent;
            this.circle1.IndexColor = System.Drawing.Color.DeepSkyBlue;
            this.circle1.Interval = 50;
            this.circle1.Location = new System.Drawing.Point(780, 17);
            this.circle1.Name = "circle1";
            this.circle1.NCircle = 8;
            this.circle1.Others = System.Drawing.Color.LightGray;
            this.circle1.Radius = 3;
            this.circle1.Size = new System.Drawing.Size(39, 31);
            this.circle1.TabIndex = 11;
            this.circle1.Text = "processingControl1";
            this.circle1.Visible = false;
            // 
            // btn_extract
            // 
            this.btn_extract.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_extract.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_extract.BackColor = System.Drawing.Color.Transparent;
            this.btn_extract.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_extract.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btn_extract.ForeColor = System.Drawing.Color.White;
            this.btn_extract.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(188)))), ((int)(((byte)(210)))));
            this.btn_extract.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(167)))), ((int)(((byte)(188)))));
            this.btn_extract.Location = new System.Drawing.Point(675, 16);
            this.btn_extract.Name = "btn_extract";
            this.btn_extract.Radius = 6;
            this.btn_extract.Size = new System.Drawing.Size(98, 33);
            this.btn_extract.Stroke = false;
            this.btn_extract.StrokeColor = System.Drawing.Color.Gray;
            this.btn_extract.TabIndex = 10;
            this.btn_extract.Text = "Extract";
            this.btn_extract.Transparency = false;
            this.btn_extract.Click += new System.EventHandler(this.btn_extract_Click);
            // 
            // txt_inputVideo
            // 
            this.txt_inputVideo.BackColor = System.Drawing.Color.Transparent;
            this.txt_inputVideo.BackgroundColor = System.Drawing.Color.White;
            this.txt_inputVideo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(133)))), ((int)(((byte)(200)))));
            this.txt_inputVideo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txt_inputVideo.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txt_inputVideo.Location = new System.Drawing.Point(106, 17);
            this.txt_inputVideo.Name = "txt_inputVideo";
            this.txt_inputVideo.Padding = new System.Windows.Forms.Padding(5);
            this.txt_inputVideo.Pattern = "";
            this.txt_inputVideo.Size = new System.Drawing.Size(563, 30);
            this.txt_inputVideo.TabIndex = 9;
            this.txt_inputVideo.TextChanged += new System.EventHandler(this.txt_imagePath_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.label3.Location = new System.Drawing.Point(9, 24);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 19);
            this.label3.TabIndex = 8;
            this.label3.Text = "Input video";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Right;
            this.listView1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(738, 124);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(345, 349);
            this.listView1.TabIndex = 39;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Idx";
            this.columnHeader1.Width = 40;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Image path";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Num object";
            this.columnHeader3.Width = 100;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Highlight;
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(653, 313);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // FormExtract
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1083, 473);
            this.Controls.Add(this.pnCamera1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.panel1);
            this.Name = "FormExtract";
            this.Text = "FormExtract";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormExtract_FormClosing);
            this.Load += new System.EventHandler(this.FormExtract_Load);
            this.SizeChanged += new System.EventHandler(this.FormExtract_SizeChanged);
            this.pnCamera1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_numThread)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnCamera1;
        private System.Windows.Forms.Panel panel1;
        private AltoControls.ProcessingControl circle1;
        private AltoControls.AltoButton btn_extract;
        private AltoControls.BrowseFile txt_inputVideo;
        private System.Windows.Forms.Label label3;
        private AltoControls.BrowseDir txt_output;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txt_numThread;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbl_count;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}