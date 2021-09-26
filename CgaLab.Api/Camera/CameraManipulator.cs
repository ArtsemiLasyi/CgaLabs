using System;
using System.Numerics;

namespace CgaLab.Api.Camera
{
    public class CameraManipulator
    {
        public CameraModel Camera { get; private set; }

        private readonly float sensitivity = 0.01f;

        public CameraManipulator()
        {
            Camera = new CameraModel(
                new Vector3(0, 0, 500),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                (float)Math.PI / 3
            ); 
        }

        public void RotateY(int xOffset)
        {
            Camera.Eye = Vector3.Transform(Camera.Eye, Matrix4x4.CreateRotationY(sensitivity * -xOffset));
        }

        public void RotateX(int yOffset)
        {
            Camera.Eye = Vector3.Transform(Camera.Eye, Matrix4x4.CreateRotationX(sensitivity * yOffset));
            Camera.Up = Vector3.Transform(Camera.Up, Matrix4x4.CreateRotationX(sensitivity * yOffset));
        }
    }
}
