using System.Numerics;

namespace CgaLab.Api
{
    public static class CommonMatrixes
    {
        public static Matrix4x4 GetTransformationMatrix(Vector3 vector)
        {
            return new Matrix4x4(
                1, 0, 0, vector.X,
                0, 1, 0, vector.Y,
                0, 0, 1, vector.Z,
                0, 0, 0, 1
            );
        }
    }
}
