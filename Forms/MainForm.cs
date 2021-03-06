﻿using OpenTK;
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

        Vector3 _viewportPosSprite = Vector3.Zero;
        Vector3 _viewportPosVertex = Vector3.Zero;
        Vector3 _viewportPosUV = Vector3.Zero;
        float _cameraZoomSprite = 1;
        float _cameraZoomVertex = 1;
        float _cameraZoomUV = 1;
        long lastRenderTime = 0;
        float frameLength;

        Vector3 ViewportPosition
        {
            get
            {
                if (Editor.SelectedSprite != null)
                    return _viewportPosSprite;
                if (Editor.Mode == EditorMode.ShapeVertex)
                    return _viewportPosVertex;
                if (Editor.Mode == EditorMode.ShapeUV)
                    return _viewportPosUV;

                return Vector3.Zero;
            }

            set
            {
                if (Editor.SelectedSprite != null)
                    _viewportPosSprite = value;
                else if (Editor.Mode == EditorMode.ShapeVertex)
                    _viewportPosVertex = value;
                else if (Editor.Mode == EditorMode.ShapeUV)
                    _viewportPosUV = value;
            }
        }

        float ViewportZoom
        {
            get
            {
                if (Editor.SelectedSprite != null)
                    return _cameraZoomSprite;
                if (Editor.Mode == EditorMode.ShapeVertex)
                    return _cameraZoomVertex;
                if (Editor.Mode == EditorMode.ShapeUV)
                    return _cameraZoomUV;

                return 1;
            }

            set
            {
                if (Editor.SelectedSprite != null)
                    _cameraZoomSprite = value;
                else if (Editor.Mode == EditorMode.ShapeVertex)
                    _cameraZoomVertex = value;
                else if (Editor.Mode == EditorMode.ShapeUV)
                    _cameraZoomUV = value;
            }
        }

        bool ShiftHeld = false;

        Lumen.Shape SelectedShape = null;
        List<Lumen.Graphic> SelectedGraphics = new List<Lumen.Graphic>();
        DisplaySprite rootMc;

        public MainForm()
        {
            InitializeComponent();
            Preferences.Init();
            timeline.Editor = Editor;

            glControl.Focus();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            glControl.Invalidate();
            timeline.Invalidate();

            long renderTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if ((renderTime - lastRenderTime) <= frameLength)
                return;

            lastRenderTime = renderTime;

            glControl.MakeCurrent();
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

            GL.Translate(ViewportPosition);
            GL.Scale(ViewportZoom, ViewportZoom, 0);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (Editor.SelectedSprite != null)
            {
                GL.UseProgram(Editor.Shader.ProgramID);

                if (Editor.SelectedSprite != rootMc)
                    GL.Translate(glControl.ClientRectangle.Width / 2 / ViewportZoom, glControl.ClientRectangle.Height / 2 / ViewportZoom, 0);

                Editor.SelectedSprite.Update();
                Editor.SelectedSprite.Render(new RenderState());
            }

            if (SelectedShape != null)
            {
                foreach (var graphic in SelectedGraphics)
                {
                    Editor.DrawGraphicHandles(graphic);
                }
            }

            if (dragging && (Editor.SelectedVerts.Count == 0 || ShiftHeld))
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(selectionRect.X, selectionRect.Y);
                GL.Vertex2(selectionRect.X + Editor.dragPosition.X, selectionRect.Y);
                GL.Vertex2(selectionRect.X + Editor.dragPosition.X, selectionRect.Y + Editor.dragPosition.Y);
                GL.Vertex2(selectionRect.X, selectionRect.Y + Editor.dragPosition.Y);
                GL.End();
            }

            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopAttrib();

            glControl.SwapBuffers();
        }

        void loadFile (string filename)
        {
            Editor.LoadFile(filename);
            treeView1.Nodes.Clear();

            var spritesNode = new TreeNode("Sprites");
            spritesNode.Tag = rootMc;
            var shapesNode = new TreeNode("Shapes");

            foreach (var mc in Editor.lm.Sprites)
            {
                var mcNode = new TreeNode($"characterId 0x{mc.CharacterId:X3}");
                mcNode.Tag = (DisplaySprite)Editor.CharacterDict[mc.CharacterId];

                spritesNode.Nodes.Add(mcNode);
            }

            foreach (var shape in Editor.lm.Shapes)
            {
                var shapeNode = new TreeNode($"characterId 0x{shape.CharacterId:X3}");
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

                shapesNode.Nodes.Add(shapeNode);
            }

            treeView1.Nodes.Add(spritesNode);
            treeView1.Nodes.Add(shapesNode);

            spritesNode.Expand();
            shapesNode.Expand();

            // FIXME: oh wow lmao
            rootMc = (DisplaySprite)((DisplaySprite)Editor.CharacterDict[(int)Editor.lm.properties.maxCharacterId]).Clone();
            rootMc.Init();

            if (filename.EndsWith("stage.lm"))
            {
                rootMc.GotoLabel("in_end");
                rootMc.Stop();

                rootMc.GetPathMC("img_group.create_mc").Visible = false;
                rootMc.GetPathMC("title_group.return_btn.press_area").Visible = false;
            }

            if (filename.EndsWith("main.lm"))
            {
                var hitMc = rootMc.SearchChild("hit_01");
                hitMc.Visible = false;
            }

            //rootMc.GetPathMC("title_group").Visible = false;
            //rootMc.GetPathMC("btn_01.anim.txt_mc").Visible = false;
            //rootMc.SearchChild("hit_02").Visible = false;

            //rootMc.GotoLabel("read");
            //rootMc.SearchChild("title_group").Stop();

            //Editor.SelectSprite(rootMc);

            frameLength = 1000f / Editor.lm.properties.framerate;

            EnableControls();
        }

        private void EnableControls()
        {
            numericUpDownAtlasId.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveasToolStripMenuItem.Enabled = true;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            var name = Preferences.Instance.StartupFile;
            if (name != null)
                loadFile($@"data\ui\lumen\{name}\{name}.lm");

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
            Editor.SelectSprite(null);

            if (e.Node.Tag is DisplaySprite)
            {
                Editor.SelectSprite((DisplaySprite)e.Node.Tag);
            }
            else if (e.Node.Tag is Lumen.Shape)
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
            if (e.Button == MouseButtons.Right)
                panning = true;

            if (SelectedShape == null)
                return;

            var mouseX = (e.X - ViewportPosition.X) / ViewportZoom;
            var mouseY = (e.Y - ViewportPosition.Y) / ViewportZoom;
            selectionRect.X = mouseX;
            selectionRect.Y = mouseY;

            if (e.Button == MouseButtons.Left)
            {
                if (!ShiftHeld)
                    Editor.SelectedVerts.Clear();

                if (Editor.Mode == EditorMode.ShapeVertex)
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
                else if (Editor.Mode == EditorMode.ShapeUV)
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
            if (e.Button == MouseButtons.Right)
                panning = false;

            if (SelectedShape == null)
                return;

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

            if (Editor.Mode == EditorMode.ShapeVertex)
                return new RectangleF(vert.X - halfSize, vert.Y - halfSize, squareSize, squareSize);
            else if (Editor.Mode == EditorMode.ShapeUV)
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
            timeline.zOnKeyDown(e);

            if (e.Control && e.KeyCode == Keys.O)
                openToolStripMenuItem_Click(null, null);

            if (e.Control && e.KeyCode == Keys.S)
                saveToolStripMenuItem_Click(null, null);

            if (e.Control && e.Shift && e.KeyCode == Keys.S)
                saveasToolStripMenuItem_Click(null, null);

            if (e.KeyCode == Keys.OemPeriod)
            {
                if (Editor.SelectedSprite != null)
                {
                    if (Editor.SelectedSprite.CurrentFrame < Editor.SelectedSprite.Sprite.Frames.Count - 1)
                    Editor.SelectedSprite.GotoFrame(Editor.SelectedSprite.CurrentFrame+1);
                }
            }

            if (e.KeyCode == Keys.Oemcomma)
            {
                if (Editor.SelectedSprite != null)
                {
                    if (Editor.SelectedSprite.CurrentFrame > 0)
                        Editor.SelectedSprite.GotoFrame(Editor.SelectedSprite.CurrentFrame-1);
                }
            }

            if (e.Shift)
                ShiftHeld = true;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            timeline.zOnKeyUp(e);

            if (!e.Shift)
                ShiftHeld = false;

            if (e.KeyCode == Keys.Tab)
            {
                if (Editor.Mode == EditorMode.ShapeUV)
                    Editor.Mode = EditorMode.ShapeVertex;
                else
                    Editor.Mode = EditorMode.ShapeUV;
            }

            if (e.KeyCode == Keys.Space)
            {
                if (Editor.SelectedSprite != null)
                {
                    if (Editor.SelectedSprite.Playing)
                    {
                        Editor.SelectedSprite.Stop();
                    }
                    else
                    {
                        if (Editor.SelectedSprite.CurrentFrame >= Editor.SelectedSprite.Sprite.Frames.Count - 1)
                            Editor.SelectedSprite.Reset();
                        Editor.SelectedSprite.Play();
                    }
                }
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
            var atlasEditor = new AtlasEditor(Editor);
            atlasEditor.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void colorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorEditor = new ColorEditor(Editor.lm.Colors);
            colorEditor.Show();
        }

        private void dumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.lm.Rebuild(true);
        }
    }
}
