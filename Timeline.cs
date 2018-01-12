using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lumenati
{
    public partial class Timeline : UserControl
    {
        public LumenEditor Editor;

        const int FrameWidth = 8;
        const int FrameHeight = 20;
        const int TickHeight = 3;
        const int HeaderHeight = 30;
        const int PlayheadHeight = 15;

        Color _PlayheadColor = Color.Red;
        Color PlayheadBGColor = Color.FromArgb(128, 255, 0, 0);

        bool Scrubbing = false;
        int ScrubFrame;
        bool PreviousPlayState;

        public Color PlayheadColor
        {
            get
            {
                return _PlayheadColor;
            }

            set
            {
                _PlayheadColor = value;
                PlayheadBGColor = Color.FromArgb(128, value.R, value.G, value.B);
            }
        }

        bool Usable
        {
            get
            {
                return (Editor != null && Editor.lm != null && Editor.SelectedSprite != null);
            }
        }

        public Timeline()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnClick(EventArgs e)
        {
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Let's just deal with scrubbing for the time being...
            if (Usable)
            {
                if (e.Y < HeaderHeight)
                {
                    Scrubbing = true;
                    PreviousPlayState = Editor.SelectedSprite.Playing;
                    Editor.SelectedSprite.Playing = false;
                    OnMouseMove(e);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (Usable)
            {
                if (Scrubbing)
                {
                    Scrubbing = false;
                    Editor.SelectedSprite.Playing = PreviousPlayState;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Usable)
            {
                if (Scrubbing)
                {
                    var mc = Editor.SelectedSprite.Sprite;

                    ScrubFrame = (e.X / FrameWidth);
                    ScrubFrame = Math.Min(Math.Max(ScrubFrame, 0), mc.Frames.Count-1);
                    Editor.SelectedSprite.GotoFrame(ScrubFrame);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var pen = new Pen(ForeColor);
            var g = e.Graphics;

            var textBrush = new SolidBrush(ForeColor);

            int numFrames = 100;

            for (int i = 0; i < numFrames; i++)
            {
                if ((i % 5) == 0)
                {
                    var text = i.ToString();
                    var textX = (i * FrameWidth) + (FrameWidth / 2) - (g.MeasureString(text, Font).Width / 2);

                    g.DrawString(text, Font, textBrush, textX, 0);
                }

                var x = (i + 1) * FrameWidth;
                var y = 20;
                g.DrawLine(pen, x, y, x, y+TickHeight);
            }

            if (Usable)
            {
                var lm = Editor.lm;
                var frameBrush = new SolidBrush(Color.Gray);
                var frameBorderPen = new Pen(ForeColor);

                var mc = Editor.SelectedSprite.Sprite;

                var x = 0;
                for (int i = 0; i < mc.labels.Count; i++)
                {
                    var label = mc.labels[i];
                    int numLabelFrames;

                    if (i < mc.labels.Count - 1) // not last
                        numLabelFrames = mc.labels[i + 1].StartFrame - label.StartFrame;
                    else
                        numLabelFrames = mc.Frames.Count - label.StartFrame;

                    var labelWidth = numLabelFrames * FrameWidth;

                    g.FillRectangle(frameBrush, x, HeaderHeight, labelWidth, FrameHeight);
                    x += labelWidth;
                }

                // FIXME: lol
                x = 0;
                for (int i = 0; i < mc.labels.Count; i++)
                {
                    var label = mc.labels[i];
                    int numLabelFrames;

                    if (i < mc.labels.Count - 1) // not last
                        numLabelFrames = mc.labels[i + 1].StartFrame - label.StartFrame;
                    else
                        numLabelFrames = mc.Frames.Count - label.StartFrame;

                    var labelWidth = numLabelFrames * FrameWidth;

                    g.DrawString(lm.Strings[label.NameId], Font, textBrush, x, HeaderHeight);
                    g.DrawLine(frameBorderPen, x + labelWidth, HeaderHeight, x + labelWidth, HeaderHeight + FrameHeight);
                    g.DrawLine(frameBorderPen, x, HeaderHeight + FrameHeight, x + labelWidth, HeaderHeight + FrameHeight);

                    x += labelWidth;
                }
                
                if (Scrubbing)
                {
                    var scrubPen = new Pen(Color.Black, 2);
                    var scrubX = ScrubFrame * FrameWidth + FrameWidth / 2;
                    g.DrawLine(scrubPen, scrubX, PlayheadHeight, scrubX, 60);
                }
                else
                {
                    var playheadPen = new Pen(PlayheadColor);
                    var playheadBrush = new SolidBrush(PlayheadBGColor);
                    var playheadX = Editor.SelectedSprite.CurrentFrame * FrameWidth + FrameWidth / 2;
                    g.DrawLine(playheadPen, playheadX, PlayheadHeight, playheadX, 60);
                    g.FillRectangle(playheadBrush, playheadX - FrameWidth / 2, 0, FrameWidth, PlayheadHeight);
                    g.DrawRectangle(playheadPen, playheadX - FrameWidth / 2, 0, FrameWidth, PlayheadHeight);
                }
            }
        }
    }
}
