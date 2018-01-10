using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;

namespace Lumenati
{
    public partial class ColorEditor : Form
    {
        List<Vector4> Colors;

        public ColorEditor(List<Vector4> colors)
        {
            InitializeComponent();

            Colors = colors;

            foreach (var color in Colors)
            {
                addColor(color);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        void addColor(Vector4 color)
        {
            var item = new ListViewItem(new string[] { lumenColorToHexString(color), "    " });
            item.SubItems[1].BackColor = lumenColorToColor(color);
            item.UseItemStyleForSubItems = false;
            listView1.Items.Add(item);
        }

        string lumenColorToHexString(Vector4 color)
        {
            return $"#{(byte)(color.X * 255):X2}{(byte)(color.Z * 255):X2}{(byte)(color.Y * 255):X2}, {(byte)(color.W * 255):X2}";
        }

        Color lumenColorToColor(Vector4 color)
        {
            return Color.FromArgb((int)(color.W * 255), (int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var lmColor = Colors[listView1.SelectedIndices[0]];
            var color = lumenColorToColor(lmColor);
            pictureBox1.BackColor = color;

            trackBarRed.Value = color.R;
            trackBarGreen.Value = color.G;
            trackBarBlue.Value = color.B;
            trackBarAlpha.Value = color.A;
        }

        void modifySelectedColor()
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            var idx = listView1.SelectedIndices[0];
            var lmColor = Colors[idx];

            lmColor.X = trackBarRed.Value / 255f;
            lmColor.Y = trackBarGreen.Value / 255f;
            lmColor.Z = trackBarBlue.Value / 255f;
            lmColor.W = trackBarAlpha.Value / 255f;
            Colors[idx] = lmColor;

            var color = lumenColorToColor(lmColor);
            listView1.SelectedItems[0].SubItems[0].Text = lumenColorToHexString(lmColor);
            listView1.SelectedItems[0].SubItems[1].BackColor = color;
            pictureBox1.BackColor = color;
        }

        private void colorTrackBar_Scroll(object sender, EventArgs e)
        {
            modifySelectedColor();
        }
    }
}
