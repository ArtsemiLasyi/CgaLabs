using System;
using System.Numerics;

namespace CgaLab.Api.Camera
{
    public class CameraManipulator
    {
        public CameraModel Camera { get; private set; }

        public CameraManipulator()
        {
            Camera = new CameraModel(
                new Vector3(0, 0, 500),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                (float)Math.PI / 3
            ); 
        }
    }
}
