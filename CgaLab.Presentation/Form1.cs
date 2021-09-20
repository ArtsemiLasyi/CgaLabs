using CgaLab.Api;
using CgaLab.Api.Bitmaps;
using CgaLab.Api.Camera;
using CgaLab.Api.ObjFormat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CgaLab.Presentation
{
    public partial class FormACG : Form
    {
        MatrixTransformator transformator;
        BitmapDrawer bitmapDrawer;
        CameraManipulator manipulator;
        WatchModel model;
        List<Vector3> points;

        public FormACG()
        {
            InitializeComponent();

            manipulator = new CameraManipulator();
            transformator = new MatrixTransformator(Size.Width, Size.Height);
            bitmapDrawer = new BitmapDrawer(Size.Width, Size.Height);

            InitPictureBox();
        }

        public void InitPictureBox()
        {
            UpdateSize();
        }

        private void FormACG_Resize(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            ModelPictureBox.Location = new Point(0, 0);
            ModelPictureBox.Width = Size.Width;
            ModelPictureBox.Height = Size.Height;

            transformator.Height = Size.Height;
            transformator.Width = Size.Width;

            bitmapDrawer = new BitmapDrawer(Size.Width, Size.Height);
        }

        private async void FormACG_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                if (ModelOpenDialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                ObjParser parser = new ObjParser();
                string filename = ModelOpenDialog.FileName;
                ObjModel objModel = await parser.ParseAsync(filename);
                model = new WatchModel(objModel);
                DrawTimer.Start();
            }
        }

        private void FormACG_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void FormACG_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void FormACG_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            manipulator.Camera.Eye = Vector3.Transform(manipulator.Camera.Eye, Matrix4x4.CreateRotationY(0.10f));
            points = transformator.Transform(manipulator.Camera, model);
            ModelPictureBox.Image = bitmapDrawer.GetBitmap(points, model);
        }
    }
}
