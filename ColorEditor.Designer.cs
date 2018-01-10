namespace Lumenati
{
    partial class ColorEditor
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.trackBarAlpha = new System.Windows.Forms.TrackBar();
            this.trackBarBlue = new System.Windows.Forms.TrackBar();
            this.trackBarGreen = new System.Windows.Forms.TrackBar();
            this.trackBarRed = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelHex = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxHex = new System.Windows.Forms.TextBox();
            this.labelRedValue = new System.Windows.Forms.Label();
            this.labelGreenValue = new System.Windows.Forms.Label();
            this.labelBlueValue = new System.Windows.Forms.Label();
            this.labelAlphaValue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRed)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(221, 405);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 169;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 3);
            this.pictureBox1.Location = new System.Drawing.Point(3, 253);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(364, 152);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // trackBarAlpha
            // 
            this.trackBarAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarAlpha.Enabled = false;
            this.trackBarAlpha.Location = new System.Drawing.Point(60, 153);
            this.trackBarAlpha.Maximum = 255;
            this.trackBarAlpha.Name = "trackBarAlpha";
            this.trackBarAlpha.Size = new System.Drawing.Size(266, 44);
            this.trackBarAlpha.TabIndex = 3;
            this.trackBarAlpha.Scroll += new System.EventHandler(this.colorTrackBar_Scroll);
            // 
            // trackBarBlue
            // 
            this.trackBarBlue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarBlue.Enabled = false;
            this.trackBarBlue.Location = new System.Drawing.Point(60, 103);
            this.trackBarBlue.Maximum = 255;
            this.trackBarBlue.Name = "trackBarBlue";
            this.trackBarBlue.Size = new System.Drawing.Size(266, 44);
            this.trackBarBlue.TabIndex = 2;
            this.trackBarBlue.Scroll += new System.EventHandler(this.colorTrackBar_Scroll);
            // 
            // trackBarGreen
            // 
            this.trackBarGreen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarGreen.Enabled = false;
            this.trackBarGreen.Location = new System.Drawing.Point(60, 53);
            this.trackBarGreen.Maximum = 255;
            this.trackBarGreen.Name = "trackBarGreen";
            this.trackBarGreen.Size = new System.Drawing.Size(266, 44);
            this.trackBarGreen.TabIndex = 1;
            this.trackBarGreen.Scroll += new System.EventHandler(this.colorTrackBar_Scroll);
            // 
            // trackBarRed
            // 
            this.trackBarRed.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarRed.Enabled = false;
            this.trackBarRed.Location = new System.Drawing.Point(60, 3);
            this.trackBarRed.Maximum = 255;
            this.trackBarRed.Name = "trackBarRed";
            this.trackBarRed.Size = new System.Drawing.Size(266, 44);
            this.trackBarRed.TabIndex = 0;
            this.trackBarRed.Scroll += new System.EventHandler(this.colorTrackBar_Scroll);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 50);
            this.label1.TabIndex = 4;
            this.label1.Text = "Red";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.55952F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.44048F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Controls.Add(this.labelAlphaValue, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelBlueValue, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelGreenValue, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelRedValue, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelHex, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.trackBarRed, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.trackBarAlpha, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.trackBarGreen, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.trackBarBlue, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxHex, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(239, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(370, 405);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // labelHex
            // 
            this.labelHex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHex.AutoSize = true;
            this.labelHex.Location = new System.Drawing.Point(3, 200);
            this.labelHex.Name = "labelHex";
            this.labelHex.Size = new System.Drawing.Size(51, 50);
            this.labelHex.TabIndex = 8;
            this.labelHex.Text = "Hex";
            this.labelHex.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 50);
            this.label4.TabIndex = 7;
            this.label4.Text = "Alpha";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 50);
            this.label2.TabIndex = 5;
            this.label2.Text = "Green";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 50);
            this.label3.TabIndex = 6;
            this.label3.Text = "Blue";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxHex
            // 
            this.textBoxHex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxHex, 2);
            this.textBoxHex.Enabled = false;
            this.textBoxHex.Location = new System.Drawing.Point(60, 214);
            this.textBoxHex.Name = "textBoxHex";
            this.textBoxHex.Size = new System.Drawing.Size(307, 22);
            this.textBoxHex.TabIndex = 9;
            this.textBoxHex.Text = "#000000";
            this.textBoxHex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxHex_KeyDown);
            this.textBoxHex.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxHex_Validating);
            this.textBoxHex.Validated += new System.EventHandler(this.textBoxHex_Validated);
            // 
            // labelRedValue
            // 
            this.labelRedValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRedValue.AutoSize = true;
            this.labelRedValue.Location = new System.Drawing.Point(332, 0);
            this.labelRedValue.Name = "labelRedValue";
            this.labelRedValue.Size = new System.Drawing.Size(35, 50);
            this.labelRedValue.TabIndex = 10;
            this.labelRedValue.Text = "0";
            this.labelRedValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelGreenValue
            // 
            this.labelGreenValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGreenValue.AutoSize = true;
            this.labelGreenValue.Location = new System.Drawing.Point(332, 50);
            this.labelGreenValue.Name = "labelGreenValue";
            this.labelGreenValue.Size = new System.Drawing.Size(35, 50);
            this.labelGreenValue.TabIndex = 11;
            this.labelGreenValue.Text = "0";
            this.labelGreenValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelBlueValue
            // 
            this.labelBlueValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBlueValue.AutoSize = true;
            this.labelBlueValue.Location = new System.Drawing.Point(332, 100);
            this.labelBlueValue.Name = "labelBlueValue";
            this.labelBlueValue.Size = new System.Drawing.Size(35, 50);
            this.labelBlueValue.TabIndex = 12;
            this.labelBlueValue.Text = "0";
            this.labelBlueValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelAlphaValue
            // 
            this.labelAlphaValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAlphaValue.AutoSize = true;
            this.labelAlphaValue.Location = new System.Drawing.Point(332, 150);
            this.labelAlphaValue.Name = "labelAlphaValue";
            this.labelAlphaValue.Size = new System.Drawing.Size(35, 50);
            this.labelAlphaValue.TabIndex = 13;
            this.labelAlphaValue.Text = "0";
            this.labelAlphaValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ColorEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 429);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorEditor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ColorEditor";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRed)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TrackBar trackBarAlpha;
        private System.Windows.Forms.TrackBar trackBarBlue;
        private System.Windows.Forms.TrackBar trackBarGreen;
        private System.Windows.Forms.TrackBar trackBarRed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelHex;
        private System.Windows.Forms.TextBox textBoxHex;
        private System.Windows.Forms.Label labelAlphaValue;
        private System.Windows.Forms.Label labelBlueValue;
        private System.Windows.Forms.Label labelGreenValue;
        private System.Windows.Forms.Label labelRedValue;
    }
}