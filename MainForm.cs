using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lumenati
{
    public partial class MainForm : Form
    {
        LumenEditor Editor = new LumenEditor();

        bool swappingHack = false;
        bool panning = false;
        bool dragging = false;
        RectangleF selectionRect = new Rectangle();
        Vector3 mousePosition = Vector3.Zero;

        Vector3 _viewportPosVertex = Vector3.Zero;
        Vector3 _viewportPosUV = Vector3.Zero;
        float _cameraZoomVertex = 1;
        float _cameraZoomUV = 1;
        long lastRenderTime = 0;
        float frameLength;

        Vector3 ViewportPosition
        {
            get
            {
                if (Editor.Mode == EditorMode.Vertex)
                    return _viewportPosVertex;
                else if (Editor.Mode == EditorMode.UV)
                    return _viewportPosUV;
                else
                    return Vector3.Zero;
            }

            set
            {
                if (Editor.Mode == EditorMode.Vertex)
                    _viewportPosVertex = value;
                else if (Editor.Mode == EditorMode.UV)
                    _viewportPosUV = value;
            }
        }

        float ViewportZoom
        {
            get
            {
                if (Editor.Mode == EditorMode.Vertex)
                    return _cameraZoomVertex;
                else if (Editor.Mode == EditorMode.UV)
                    return _cameraZoomUV;
                else
                    return 1;
            }

            set
            {
                if (Editor.Mode == EditorMode.Vertex)
                    _cameraZoomVertex = value;
                else if (Editor.Mode == EditorMode.UV)
                    _cameraZoomUV = value;
            }
        }

        bool ShiftHeld = false;

        RuntimeSprite SelectedSprite = null;

        Lumen.Shape SelectedShape = null;
        List<Lumen.Graphic> SelectedGraphics = new List<Lumen.Graphic>();
        RuntimeSprite rootMc;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            glControl.MakeCurrent();
            glControl.Invalidate();

            long renderTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if ((renderTime - lastRenderTime) <= frameLength)
                return;

            lastRenderTime = renderTime;


            var ortho = Matrix4.CreateOrthographicOffCenter(
                0,
                glControl.ClientRectangle.Width,
                glControl.ClientRectangle.Height,
                0,
                0, 10
            );

            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.UseProgram(0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.LoadMatrix(ref ortho);
            GL.Viewport(glControl.ClientRectangle);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            //GL.Translate(ViewportPosition);
            GL.Scale(ViewportZoom, ViewportZoom, 0);

            if (SelectedSprite != null)
            {
                GL.Translate(glControl.ClientRectangle.Width / 2 / ViewportZoom, glControl.ClientRectangle.Height / 2 / ViewportZoom, 0);
                SelectedSprite.Update();
                SelectedSprite.Render();
            }
            else
            {
                rootMc.Update();
                rootMc.Render();
            }


            //if (SelectedShape != null)
            //{
            //    foreach (var graphic in SelectedGraphics)
            //    {
            //        Editor.DrawGraphicHandles(graphic);
            //    }
            //}

            //if (dragging && (Editor.SelectedVerts.Count == 0 || ShiftHeld))
            //{
            //    GL.Begin(PrimitiveType.LineLoop);
            //    GL.Vertex2(selectionRect.X, selectionRect.Y);
            //    GL.Vertex2(selectionRect.X + Editor.dragPosition.X,  selectionRect.Y);
            //    GL.Vertex2(selectionRect.X + Editor.dragPosition.X,  selectionRect.Y + Editor.dragPosition.Y);
            //    GL.Vertex2(selectionRect.X,  selectionRect.Y + Editor.dragPosition.Y);
            //    GL.End();
            //}

            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopAttrib();

            glControl.SwapBuffers();
        }

        void PopulateShapeTree()
        {
            shapeTree.Nodes.Clear();
            foreach (var shape in Editor.lm.Shapes)
            {
                var shapeNode = new TreeNode($"Shape c{shape.CharacterId}");
                shapeNode.Tag = shape;

                if (shape.Graphics.Length > 1)
                {
                    for (int graphicId = 0; graphicId < shape.Graphics.Length; graphicId++)
                    {
                        var graphicNode = new TreeNode($"Graphic {graphicId}");
                        graphicNode.Tag = shape.Graphics[graphicId];
                        shapeNode.Nodes.Add(graphicNode);
                    }
                }

                shapeTree.Nodes.Add(shapeNode);
            }
        }

        void loadFile (string filename)
        {
            Editor.LoadFile(filename);
            listView1.Items.Clear();
            foreach (var mc in Editor.lm.Sprites)
            {
                var mcItem = new ListViewItem($"Sprite c{mc.CharacterId}");
                mcItem.Tag = mc;

                listView1.Items.Add(mcItem);
            }

            rootMc = Editor.RuntimeSprites[Editor.RuntimeSprites.Count-1];
            frameLength = 1000f / Editor.lm.properties.framerate;

            EnableControls();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count == 0)
                SelectedSprite = null;
            else
                SelectedSprite = Editor.RuntimeSprites[listView1.SelectedIndices[0]];

            //trackBar1.Maximum = SelectedSprite.Frames.Count - 1;
            //PopulateShapeTree();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            //CurrentFrame = trackBar1.Value;
        }

        private void EnableControls()
        {
            numericUpDownAtlasId.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveasToolStripMenuItem.Enabled = true;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
#if DEBUG
            var name = "main";
            loadFile($@"C:\s4explore\extract\data\ui\lumen\{name}\{name}.lm");
#endif
            glControl.MouseWheel += glControl_MouseWheel;

            GL.ClearColor(Color.CornflowerBlue);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // FIXME: this is a really stupid way of doing this.
            _viewportPosVertex = new Vector3(glControl.DisplayRectangle.Width * ViewportZoom / 2, glControl.DisplayRectangle.Height / 2, 0);

            Application.Idle += Application_Idle;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectedGraphics.Clear();

            if (e.Node.Tag is Lumen.Shape)
            {
                SelectedShape = (Lumen.Shape)e.Node.Tag;
                SelectedGraphics.AddRange(SelectedShape.Graphics);
            }
            else if (e.Node.Tag is Lumen.Graphic)
            {
                SelectedShape = (Lumen.Shape)e.Node.Parent.Tag;
                SelectedGraphics.Add((Lumen.Graphic)e.Node.Tag);
            }
        }

        private void numericUpDownAtlasId_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedShape == null)
                return;

            if (swappingHack)
                return;

            foreach (var graphic in SelectedShape.Graphics)
            {
                graphic.AtlasId = (int)numericUpDownAtlasId.Value;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Lumen File|*.lm";

            var result = ofd.ShowDialog();
            if (result != DialogResult.OK)
                return;

            loadFile(ofd.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.SaveFile();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            string[] draggedFiles = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (draggedFiles.Length > 1)
                return;

            if (!draggedFiles[0].EndsWith(".lm"))
                return;

            e.Effect = DragDropEffects.Move;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] draggedFiles = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            loadFile(draggedFiles[0]);
        }

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            const float minScale = 0.01f;

            ViewportZoom += e.Delta / 1500.0f;
            if (ViewportZoom < minScale)
                ViewportZoom = minScale;
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (SelectedShape == null)
                return;

            if (e.Button == MouseButtons.Right)
                panning = true;

            var mouseX = (e.X - ViewportPosition.X) / ViewportZoom;
            var mouseY = (e.Y - ViewportPosition.Y) / ViewportZoom;
            selectionRect.X = mouseX;
            selectionRect.Y = mouseY;

            if (e.Button == MouseButtons.Left)
            {
                if (!ShiftHeld)
                    Editor.SelectedVerts.Clear();

                if (Editor.Mode == EditorMode.Vertex)
                {
                    foreach (var graphic in SelectedShape.Graphics)
                    {
                        var done = false;

                        for (int vertId = 0; vertId < graphic.Verts.Length; vertId++)
                        {
                            const int squareSize = 8;
                            var halfSize = squareSize / 2 /*/ ViewportZoom*/;
                            var vert = graphic.Verts[vertId];

                            if (
                                vert.X - halfSize <= mouseX &&
                                vert.X + halfSize >= mouseX &&
                                vert.Y - halfSize <= mouseY &&
                                vert.Y + halfSize >= mouseY
                            )
                            {
                                selectVert(graphic, vertId);
                                done = true;
                                break;
                            }
                        }

                        if (done)
                            break;
                    }
                }
                else if (Editor.Mode == EditorMode.UV)
                {
                    var graphic = SelectedShape.Graphics[0];
                    for (int vertId = 0; vertId < graphic.Verts.Length; vertId++)
                    {
                        const int squareSize = 8;
                        var halfSize = squareSize / 2/* / ViewportZoom*/;
                        var vert = graphic.Verts[vertId];
                        var x = vert.U * Editor.lm.Atlases[graphic.AtlasId].width;
                        var y = vert.V * Editor.lm.Atlases[graphic.AtlasId].height;

                        if (
                            x - halfSize <= mouseX &&
                            x + halfSize >= mouseX &&
                            y - halfSize <= mouseY &&
                            y + halfSize >= mouseY
                        )
                        {
                            selectVert(graphic, vertId);
                            break;
                        }
                    }
                }

                dragging = true;
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (SelectedShape == null)
                return;

            if (e.Button == MouseButtons.Right)
                panning = false;

            if (e.Button == MouseButtons.Left)
            {
                dragging = false;

                // Was box selecting?
                if (Editor.SelectedVerts.Count == 0)
                {
                    var mouseX = (e.X - ViewportPosition.X) / ViewportZoom;
                    var mouseY = (e.Y - ViewportPosition.Y) / ViewportZoom;

                    selectionRect = new RectangleF(
                        Math.Min(selectionRect.X, mouseX),
                        Math.Min(selectionRect.Y, mouseY),
                        Math.Abs(selectionRect.X - mouseX),
                        Math.Abs(selectionRect.Y - mouseY)
                    );


                    foreach (var graphic in SelectedGraphics)
                    {
                        for (int vertId = 0; vertId < graphic.Verts.Length; vertId++)
                        {
                            var vertRect = getVertSelectionRect(graphic.Verts[vertId], graphic);

                            if (selectionRect.IntersectsWith(vertRect))
                                selectVert(graphic, vertId);
                        }
                    }
                }
                else
                {
                    Editor.MoveSelectionBy(Editor.dragPosition.X, Editor.dragPosition.Y);
                }

                Editor.dragPosition = Vector3.Zero;
            }
        }

        void selectVert(Lumen.Graphic graphic, int vertId)
        {
            // TODO: there must be a cleaner way of doing this.
            foreach (var vert in Editor.SelectedVerts)
            {
                if (vert.graphic == graphic && vert.vertId == vertId)
                    return;
            }

            Editor.SelectedVerts.Add(new SelectedVertex() { graphic = graphic, vertId = vertId });
        }

        RectangleF getVertSelectionRect(Lumen.Vertex vert, Lumen.Graphic graphic)
        {
            const int squareSize = 8;
            var halfSize = squareSize / 2 / ViewportZoom;

            if (Editor.Mode == EditorMode.Vertex)
                return new RectangleF(vert.X - halfSize, vert.Y - halfSize, squareSize, squareSize);
            else if (Editor.Mode == EditorMode.UV)
                return new RectangleF(
                    (vert.U * Editor.lm.Atlases[graphic.AtlasId].width) - halfSize,
                    (vert.V * Editor.lm.Atlases[graphic.AtlasId].height) - halfSize,
                    squareSize, squareSize
                );

            return new RectangleF();
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            var newPosition = new Vector3(e.X, e.Y, 0);
            var delta = newPosition - mousePosition;

            if (panning)
                ViewportPosition += (delta * ViewportZoom);

            if (dragging)
                Editor.dragPosition += (delta / ViewportZoom);

            mousePosition = newPosition;
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Lumen File|*.lm|All Files|*.*";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            using (var stream = new FileStream(sfd.FileName, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
                writer.Write(Editor.lm.Rebuild());
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
                openToolStripMenuItem_Click(null, null);

            if (e.Control && e.KeyCode == Keys.S)
                saveToolStripMenuItem_Click(null, null);

            if (e.Control && e.Shift && e.KeyCode == Keys.S)
                saveasToolStripMenuItem_Click(null, null);

            if (e.Shift)
                ShiftHeld = true;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Shift)
                ShiftHeld = false;

            if (e.KeyCode == Keys.Tab)
            {
                if (Editor.Mode == EditorMode.UV)
                    Editor.Mode = EditorMode.Vertex;
                else
                    Editor.Mode = EditorMode.UV;
            }

            if (e.KeyCode == Keys.Left)
                Editor.MoveSelectionBy(ShiftHeld ? -10 : -1, 0);
            if (e.KeyCode == Keys.Right)
                Editor.MoveSelectionBy(ShiftHeld ? 10 : 1, 0);
            if (e.KeyCode == Keys.Up)
                Editor.MoveSelectionBy(0, ShiftHeld ? -10 : -1);
            if (e.KeyCode == Keys.Down)
                Editor.MoveSelectionBy(0, ShiftHeld ? 10 : 1);
        }

        private void atlasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var atlasForm = new AtlasForm(Editor);
            atlasForm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
