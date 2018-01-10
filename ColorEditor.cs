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

            for (int colorId = 0; colorId < Colors.Count; colorId++)
            {
                addColor(colorId, Colors[colorId]);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        void addColor(int id, Vector4 color)
        {
            var item = new ListViewItem(new string[] { lumenColorToString(id, color), "    " });
            item.SubItems[1].BackColor = lumenColorToColor(color);
            item.UseItemStyleForSubItems = false;
            listView1.Items.Add(item);
        }

        string lumenColorToString(int id, Vector4 color)
        {
            return $"0x{id:X3}: #{(byte)(color.X * 255):X2}{(byte)(color.Z * 255):X2}{(byte)(color.Y * 255):X2}, {(byte)(color.W * 255):X2}";
        }

        Color lumenColorToColor(Vector4 color)
        {
            return Color.FromArgb((int)(color.W * 255), (int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                setControlsEnabledState(false);
                return;
            }

            setControlsEnabledState(true);

            var lmColor = Colors[listView1.SelectedIndices[0]];
            var color = lumenColorToColor(lmColor);
            pictureBox1.BackColor = color;

            trackBarRed.Value = color.R;
            trackBarGreen.Value = color.G;
            trackBarBlue.Value = color.B;
            trackBarAlpha.Value = color.A;
        }

        void setControlsEnabledState(bool state)
        {
            trackBarRed.Enabled = state;
            trackBarGreen.Enabled = state;
            trackBarBlue.Enabled = state;
            trackBarAlpha.Enabled = state;
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
            listView1.SelectedItems[0].SubItems[0].Text = lumenColorToString(idx, lmColor);
            listView1.SelectedItems[0].SubItems[1].BackColor = color;
            pictureBox1.BackColor = color;
        }

        private void colorTrackBar_Scroll(object sender, EventArgs e)
        {
            modifySelectedColor();
        }

        private void ColorEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
