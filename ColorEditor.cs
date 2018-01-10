using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using System.Text.RegularExpressions;

namespace Lumenati
{
    public partial class ColorEditor : Form
    {
        const string HexColorRegex = @"^#?([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})?$";

        List<Vector4> Colors;
        string LastHexValue;
        bool HexDirty = false;

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
            return $"0x{id:X3}: {lumenColorToHtmlColor(color)}, {(byte)(color.W * 255):X2}";
        }

        string lumenColorToHtmlColor(Vector4 color)
        {
            return $"#{(byte)(color.X * 255):X2}{(byte)(color.Z * 255):X2}{(byte)(color.Y * 255):X2}";
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
            labelRedValue.Text = trackBarRed.Value.ToString();
            labelGreenValue.Text = trackBarGreen.Value.ToString();
            labelBlueValue.Text = trackBarBlue.Value.ToString();
            labelAlphaValue.Text = trackBarAlpha.Value.ToString();

            textBoxHex.Text = lumenColorToHtmlColor(lmColor);
            LastHexValue = textBoxHex.Text;
        }

        void setControlsEnabledState(bool state)
        {
            trackBarRed.Enabled = state;
            trackBarGreen.Enabled = state;
            trackBarBlue.Enabled = state;
            trackBarAlpha.Enabled = state;
            textBoxHex.Enabled = state;
        }

        void modifySelectedColor()
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            var idx = listView1.SelectedIndices[0];

            labelRedValue.Text = trackBarRed.Value.ToString();
            labelGreenValue.Text = trackBarGreen.Value.ToString();
            labelBlueValue.Text = trackBarBlue.Value.ToString();
            labelAlphaValue.Text = trackBarAlpha.Value.ToString();

            var lmColor = Colors[idx];
            lmColor.X = trackBarRed.Value / 255f;
            lmColor.Y = trackBarGreen.Value / 255f;
            lmColor.Z = trackBarBlue.Value / 255f;
            lmColor.W = trackBarAlpha.Value / 255f;
            Colors[idx] = lmColor;

            textBoxHex.Text = lumenColorToHtmlColor(lmColor);
            LastHexValue = textBoxHex.Text;

            var color = lumenColorToColor(lmColor);
            listView1.SelectedItems[0].SubItems[0].Text = lumenColorToString(idx, lmColor);
            listView1.SelectedItems[0].SubItems[1].BackColor = color;
            pictureBox1.BackColor = color;
        }

        private void colorTrackBar_Scroll(object sender, EventArgs e)
        {
            modifySelectedColor();
        }

        private void textBoxHex_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var match = Regex.Match(textBoxHex.Text, HexColorRegex, RegexOptions.IgnoreCase | RegexOptions.ECMAScript);

            if (!match.Success)
                textBoxHex.Text = LastHexValue;
            else
                LastHexValue = textBoxHex.Text;
        }

        private void textBoxHex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidateChildren();
                e.Handled = true;
                e.SuppressKeyPress = true;

                // Give focus back to form. It just looks nicer
                labelHex.Focus();
            }
        }

        private void textBoxHex_Validated(object sender, EventArgs e)
        {
            textBoxHex.Text = textBoxHex.Text.ToUpper();
            if (!textBoxHex.Text.StartsWith("#"))
                textBoxHex.Text = ("#" + textBoxHex.Text);

            // FIXME: this triggers a million updates for no good reason.
            var match = Regex.Match(textBoxHex.Text, HexColorRegex, RegexOptions.IgnoreCase | RegexOptions.ECMAScript);

            trackBarRed.Value = Convert.ToByte(match.Groups[1].Value, 16);
            trackBarGreen.Value = Convert.ToByte(match.Groups[2].Value, 16);
            trackBarBlue.Value = Convert.ToByte(match.Groups[3].Value, 16);

            if (match.Groups[4].Success)
                trackBarAlpha.Value = Convert.ToByte(match.Groups[4].Value, 16);
        }
    }
}
