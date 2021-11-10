using CgaLab.Api;
using CgaLab.Api.Bitmaps;
using CgaLab.Api.Camera;
using CgaLab.Api.Lighting;
using CgaLab.Api.ObjFormat;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace CgaLab.Presentation
{
    public partial class FormACG : Form
    {
        MatrixTransformator transformator;
        PhongBitmapDrawer bitmapDrawer;
        CameraManipulator cameraManipulator;
        LightSourceManipulator lightManipulator;
        WatchModel model;
        List<Vector3> points;

        private bool mouseDown = false;
        private Point mousePosition = new Point(0, 0);

        public FormACG()
        {
            InitializeComponent();

            cameraManipulator = new CameraManipulator();
            lightManipulator = new LightSourceManipulator();
            transformator = new MatrixTransformator(Size.Width, Size.Height);
            //bitmapDrawer = new BitmapDrawer(Size.Width, Size.Height);
            bitmapDrawer = new PhongBitmapDrawer(Size.Width, Size.Height);

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

            if (transformator != null)
            {
                transformator.Height = Size.Height;
                transformator.Width = Size.Width;
            }

            bitmapDrawer = new PhongBitmapDrawer(Size.Width, Size.Height);
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

            if (e.KeyCode == Keys.W)
            {
                lightManipulator.MoveFront();
            }

            if (e.KeyCode == Keys.S)
            {
                lightManipulator.MoveBack();
            }

            if (e.KeyCode == Keys.D)
            {
                lightManipulator.MoveRight();
            }

            if (e.KeyCode == Keys.A)
            {
                lightManipulator.MoveLeft();
            }

            if (e.KeyCode == Keys.Q)
            {
                lightManipulator.MoveUp();
            }

            if (e.KeyCode == Keys.E)
            {
                lightManipulator.MoveDown();
            }
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            points = transformator.Transform(cameraManipulator.Camera, model);
            ModelPictureBox.Image = bitmapDrawer.GetBitmap(points, model, lightManipulator.LightSource, cameraManipulator.Camera.Eye);
        }

        private void ModelPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            SaveMousePosition(e);
        }

        private void ModelPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void ModelPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                int xOffset = e.X - mousePosition.X;
                int yOffset = mousePosition.Y - e.Y;
				SaveMousePosition(e);

                cameraManipulator.RotateX(yOffset);
                cameraManipulator.RotateY(xOffset);
            }
        }
		
		private void SaveMousePosition(MouseEventArgs e)
		{
			mousePosition.X = e.X;
            mousePosition.Y = e.Y;
		}	
    }
}
