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

        PhongBitmapDrawer phongBitmapDrawer;
        LambertBitmapDrawer lambertBitmapDrawer;

        CameraManipulator cameraManipulator;
        LightSourceManipulator lightManipulator;
        WatchModel model;
        List<Vector3> points;

        private bool leftMouseDown = false;
        private bool rightMouseDown = false;

        private Point leftMousePosition = new Point(0, 0);
        private Point rightMousePosition = new Point(0, 0);

        public FormACG()
        {
            InitializeComponent();

            cameraManipulator = new CameraManipulator();
            lightManipulator = new LightSourceManipulator();
            transformator = new MatrixTransformator(Size.Width, Size.Height);

            //lambertBitmapDrawer = new LambertBitmapDrawer(Size.Width, Size.Height);
            phongBitmapDrawer = new PhongBitmapDrawer(Size.Width, Size.Height);

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

            phongBitmapDrawer = new PhongBitmapDrawer(Size.Width, Size.Height);
            //lambertBitmapDrawer = new LambertBitmapDrawer(Size.Width, Size.Height);
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

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            points = transformator.Transform(cameraManipulator.Camera, model);
            ModelPictureBox.Image = phongBitmapDrawer.GetBitmap(points, model, lightManipulator.LightSource, cameraManipulator.Camera.Eye);
            //ModelPictureBox.Image = lambertBitmapDrawer.GetBitmap(points, model, lightManipulator.LightSource);
        }

        private void ModelPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMouseDown = true;
            }
            
            if (e.Button == MouseButtons.Right)
            {
                rightMouseDown = true;
            }

            SaveMousePosition(e);
        }

        private void ModelPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftMouseDown)
            {
                leftMouseDown = false;
            }
            
            if (rightMouseDown)
            {
                rightMouseDown = false;
            }
        }

        private void ModelPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftMouseDown)
            {
                int xOffset = e.X - leftMousePosition.X;
                int yOffset = leftMousePosition.Y - e.Y;
				        SaveMousePosition(e);

                cameraManipulator.RotateX(yOffset);
                cameraManipulator.RotateY(xOffset);
            }

            if (rightMouseDown)
            {
                int xOffset = e.X - rightMousePosition.X;
                int yOffset = rightMousePosition.Y - e.Y;
                SaveMousePosition(e);

                lightManipulator.RotateX(yOffset);
                lightManipulator.RotateY(xOffset);
            }
        }
		
		private void SaveMousePosition(MouseEventArgs e)
		{
            if (leftMouseDown)
            {
                leftMousePosition.X = e.X;
                leftMousePosition.Y = e.Y;
            }

            if (rightMouseDown)
            {
                rightMousePosition.X = e.X;
                rightMousePosition.Y = e.Y;
            }	
    }
}
