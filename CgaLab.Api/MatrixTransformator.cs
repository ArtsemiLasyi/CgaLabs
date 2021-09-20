using CgaLab.Api.Camera;
using System.Collections.Generic;
using System.Numerics;

namespace CgaLab.Api
{
    public class MatrixTransformator
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private Matrix4x4 transformMatrix;

        public MatrixTransformator(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public List<Vector3> Transform(CameraModel camera, WatchModel model)
        {
            Matrix4x4 worldMatrix = CommonMatrixes.GetWorld(model);
            Matrix4x4 viewMatrix = CommonMatrixes.GetView(camera);
            Matrix4x4 projectionMatrix = CommonMatrixes.GetPerspective(camera.Fov, Width, Height);
            transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

            List<Vector3> windowVertices = GetWindow(model.Vertices);

            return windowVertices;
        }

        private List<Vector3> GetWindow(List<Vector4> vertixes)
        {
            List<Vector3> windowPoints = new List<Vector3>();
            Matrix4x4 viewPortMatrix = CommonMatrixes.GetViewPort(0, 0, Width, Height);

            foreach (Vector4 vertex in vertixes)
            {
                Vector4 transformedPoint = Vector4.Transform(vertex, transformMatrix);
                transformedPoint /= transformedPoint.W;
                Vector4 displayedPoint = Vector4.Transform(transformedPoint, viewPortMatrix);
                windowPoints.Add(
                    new Vector3(
                        displayedPoint.X,
                        displayedPoint.Y,
                        displayedPoint.Z
                    )
                );
            }

            return windowPoints;
        }
    }
}
