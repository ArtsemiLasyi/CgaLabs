using CgaLab.Api.Camera;
using System.Numerics;

namespace CgaLab.Api
{
    public static class CommonMatrixes
    {
        public static Matrix4x4 GetWorld(WatchModel model)
        {
            return Matrix4x4.CreateScale(model.Scale) 
                * GetRotation(model.Rotation)
                * GetTranslation(model.Position);
        }

        public static Matrix4x4 GetTranslation(Vector3 vector)
        {
            return Matrix4x4.CreateTranslation(vector);
        }

        public static Matrix4x4 GetRotation(Vector3 vector)
        {
            return Matrix4x4.CreateRotationY(vector.Y)
                * Matrix4x4.CreateRotationX(vector.X)
                * Matrix4x4.CreateRotationZ(vector.Z);
        }

        public static Matrix4x4 GetView(CameraModel camera)
        {
            return Matrix4x4.CreateLookAt(camera.Eye, camera.Target, camera.Up);
        }

        public static Matrix4x4 GetPerspective(float fov, float width, float height)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(fov, width / height, 0.1f, 200.0f);
        }

        public static Matrix4x4 GetViewPort(float Xmin, float Ymin, float width, float height)
        {
            return new Matrix4x4(
                width / 2, 0, 0, 0,
                0, -height / 2, 0, 0,
                0, 0, 1, 0,
                Xmin + width / 2, Ymin + height / 2, 0, 1
            );
        }
    }
}
