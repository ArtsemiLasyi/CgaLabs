using System.Numerics;

namespace CgaLab.Api.Lighting
{
    public class LightSourceManipulator
    {
        public Vector3 LightSource { get; private set; }

        private readonly float sensitivity = 10f;

        public LightSourceManipulator()
        {
            LightSource = new Vector3(0, 500, 0);
        }

        public void MoveUp()
        {
            LightSource += Vector3.UnitY * sensitivity;
        }

        public void MoveDown()
        {
            LightSource -= Vector3.UnitY * sensitivity;
        }

        public void MoveRight()
        {
            LightSource += Vector3.UnitX * sensitivity;
        }

        public void MoveLeft()
        {
            LightSource -= Vector3.UnitX * sensitivity;
        }

        public void MoveFront()
        {
            LightSource -= Vector3.UnitZ * sensitivity;
        }

        public void MoveBack()
        {
            LightSource += Vector3.UnitZ * sensitivity;
        }
    }
}
