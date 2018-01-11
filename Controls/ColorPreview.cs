using System.Drawing;
using System.Windows.Forms;

namespace Lumenati
{
    public partial class ColorPreview : UserControl
    {
        Color _color = Color.FromArgb(0, 0, 0, 0);

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                Invalidate();
            }
        }

        public ColorPreview()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var brush = new SolidBrush(Color);
            e.Graphics.FillRectangle(brush, Width / 2, 0, Width / 2, Height);

            brush.Color = Color.FromArgb(Color.R, Color.G, Color.B);
            e.Graphics.FillRectangle(brush, 0, 0, Width / 2, Height);
        }
    }
}
