using System.Numerics;

namespace CgaLab.Api.Lighting
{
    public class LightSourceManipulator
    {
        public Vector3 LightSource { get; private set; }

        private readonly float sensitivity = 0.1f;

        public LightSourceManipulator()
        {
            LightSource = new Vector3(0, 500, 0);
        }

        public void RotateY(int xOffset)
        {
            LightSource = Vector3.Transform(LightSource, Matrix4x4.CreateRotationY(sensitivity * -xOffset));
        }

        public void RotateX(int yOffset)
        {
            LightSource = Vector3.Transform(LightSource, Matrix4x4.CreateRotationX(sensitivity * yOffset));
        }
    }
}
