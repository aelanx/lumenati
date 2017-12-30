using System.Diagnostics;
using System.Windows.Forms;

namespace Lumenati
{
    public partial class AtlasForm : Form
    {
        LumenEditor Editor;

        public AtlasForm (LumenEditor editor)
        {
            InitializeComponent();

            Editor = editor;
            dataGridView1.Rows.Clear();

            Debug.Assert(Editor.lm.Atlases.Count == Editor.texlist.atlases.Count);

            for (int i = 0; i < Editor.lm.Atlases.Count; i++)
            {
                var lmAtlas = Editor.lm.Atlases[i];
                addAtlasUi(Editor.lm.Strings[lmAtlas.nameId], lmAtlas.width, lmAtlas.height, Editor.texlist.atlases[i]);
            }
        }

        void addAtlasUi(string name, float width, float height, Texlist.AtlasFlag flag)
        {
            dataGridView1.Rows.Add($"{dataGridView1.Rows.Count}", name, $"{width}x{height}", flag);
        }

        void addAtlas()
        {
            var lmAtlas = new Lumen.TextureAtlas();
            // FIXME: I have no clue if the name is used, and if so, how.
            lmAtlas.nameId = Editor.lm.Atlases[0].nameId;
            lmAtlas.id = Editor.lm.Atlases.Count;
            lmAtlas.width = 64;
            lmAtlas.height = 64;

            // If the texture already exists, we can fill in the size. Neat!
            var nut = Editor.GetAtlas(lmAtlas.id);
            if (nut != null)
            {
                lmAtlas.width = nut.width;
                lmAtlas.height = nut.height;
            }

            Editor.lm.Atlases.Add(lmAtlas);
            Editor.texlist.atlases.Add(Texlist.AtlasFlag.None);

            addAtlasUi(Editor.lm.Strings[lmAtlas.nameId], lmAtlas.width, lmAtlas.height, 0);
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            addAtlas();
        }
    }
}
