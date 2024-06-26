﻿using CgaLab.Api.Camera;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CgaLab.Api
{
    public class MatrixTransformator
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public MatrixTransformator(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public List<Vector3> Transform(CameraModel camera, WatchModel model)
        {
            //Мировые координаты
            Matrix4x4 worldMatrix = GetWorldSpace(model);

            //Координаты наблюдателя
            Matrix4x4 viewMatrix = GetViewSpace(camera);

            //Координаты проекции(перспектива)
            Matrix4x4 projectionMatrix = GetPerspectiveSpace(camera.Fov, Width, Height);

            Matrix4x4 transformMatrix = worldMatrix * viewMatrix * projectionMatrix;

            //Координаты окна
            var vetixes = GetWindowSpace(transformMatrix, model.Vertixes);

            return vetixes;
        }

        private Matrix4x4 GetWorldSpace(WatchModel model)
        {
            return GetTranslation(model.Position) 
                * Matrix4x4.CreateScale(model.Scale)
                * GetRotation(model.Rotation);
        }

        private Matrix4x4 GetTranslation(Vector3 vector)
        {
            return Matrix4x4.CreateTranslation(vector);
        }

        private Matrix4x4 GetRotation(Vector3 vector)
        {
            return Matrix4x4.CreateRotationY(vector.Y)
                * Matrix4x4.CreateRotationX(vector.X)
                * Matrix4x4.CreateRotationZ(vector.Z);
        }

        private Matrix4x4 GetViewSpace(CameraModel camera)
        {
            return Matrix4x4.CreateLookAt(camera.Eye, camera.Target, camera.Up);
        }

        private Matrix4x4 GetPerspectiveSpace(float fov, float width, float height)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(fov, width / height, 0.1f, 200.0f);
        }

        private Matrix4x4 GetViewPortSpace
            (
            float width,
            float height,
            float Xmin = 0,
            float Ymin = 0)
        {
            return new Matrix4x4(
                width / 2, 0, 0, 0,
                0, -height / 2, 0, 0,
                0, 0, 1, 0,
                Xmin + width / 2, Ymin + height / 2, 0, 1
            );
        }

        private List<Vector3> GetWindowSpace(Matrix4x4 transformMatrix, List<Vector4> vertexes)
        {
            Vector3[] windowPoints = new Vector3[vertexes.Count];

            //Координаты в соотв с шириной и высотой экрана
            Matrix4x4 viewPortMatrix = GetViewPortSpace(Width, Height);

            Parallel.ForEach(Partitioner.Create(0, vertexes.Count), range => {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vector4 transformedPoint = Vector4.Transform(vertexes[i], transformMatrix);
                    transformedPoint /= transformedPoint.W;
                    Vector4 displayedPoint = Vector4.Transform(transformedPoint, viewPortMatrix);
                    windowPoints[i] = new Vector3(
                        displayedPoint.X,
                        displayedPoint.Y,
                        displayedPoint.Z
                    );
                }
            });
            
            return windowPoints.ToList();
        }
    }
}
