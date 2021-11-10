using CgaLab.Api.Lighting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace CgaLab.Api.Bitmaps
{
    public class PhongBitmapDrawer
    {
        private ExtendedBitmap bitmap;
        private List<Vector3> windowVertices;
        public ZBuffer ZBuf { get; protected set; }
        private Color activeColor = Color.Green;
        private WatchModel model;
        private PhongLighting Light { get; set; }

        public int Width
        {
            get
            {
                return bitmap.Width;
            }
        }
        public int Height
        {
            get
            {
                return bitmap.Height;
            }
        }

        public PhongBitmapDrawer(int width, int height)
        {
            bitmap = new ExtendedBitmap(width, height);
            ZBuf = new ZBuffer(bitmap.Width, bitmap.Height);
            Light = new PhongLighting(activeColor, Color.WhiteSmoke, Color.DarkGreen);
        }

        public Bitmap GetBitmap(List<Vector3> windowVertices, WatchModel model, Vector3 lightVector, Vector3 viewVector)
        {
            int width = Width;
            int height = Height;
            bitmap = new ExtendedBitmap(width, height);
            ZBuf = new ZBuffer(bitmap.Width, bitmap.Height);

            this.windowVertices = windowVertices;
            this.model = model;

            bitmap.LockBits();

            DrawAllPixels(lightVector, viewVector);

            bitmap.UnlockBits();

            return bitmap.Source;
        }

        private void DrawAllPixels(Vector3 lightVector, Vector3 viewVector)
        {
            List<List<Vector3>> facesList = model.Poligons;

            Parallel.ForEach(Partitioner.Create(0, facesList.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var face = facesList[i];
                    if (IsFaceVisible(face))
                    {
                        DrawFace(face, lightVector, viewVector);
                    }
                }
            });
        }

        protected void DrawFace(List<Vector3> vertexIndexes, Vector3 lightVector, Vector3 viewVector)
        {
            List<PixelInfo> sidesList = new List<PixelInfo>();

            for (int i = 0; i < vertexIndexes.Count - 1; i++)
            {
                DrawLine(i, i + 1, vertexIndexes, sidesList, lightVector, viewVector);
            }

            DrawLine(vertexIndexes.Count - 1, 0, vertexIndexes, sidesList, lightVector, viewVector);

            DrawPixelForRasterization(sidesList, lightVector, viewVector);
        }

        private void DrawLine(int from, int to, List<Vector3> indexes, List<PixelInfo> sidesList, Vector3 lightVector, Vector3 viewVector)
        {
            int indexFrom = (int)indexes[from].X - 1;
            int indexTo = (int)indexes[to].X - 1;

            int indexNormalFrom = (int)indexes[from].Z - 1;
            int indexNormalTo = (int)indexes[to].Z - 1;

            Vector3 vertexFrom = windowVertices[indexFrom];
            Vector3 vertexTo = windowVertices[indexTo];

            PixelInfo pixelFrom = new PixelInfo()
            {
                Point = new Vector3(
                    (int)Math.Round(vertexFrom.X),
                    (int)Math.Round(vertexFrom.Y),
                    vertexFrom.Z),
                Normal = model.Normals[indexNormalFrom],
                World = model.Vertixes[indexFrom]
            };
            PixelInfo pixelTo = new PixelInfo()
            {
                Point = new Vector3(
                    (int)Math.Round(vertexTo.X),
                    (int)Math.Round(vertexTo.Y),
                    vertexTo.Z),
                Normal = model.Normals[indexNormalTo],
                World = model.Vertixes[indexTo]
            };

            IEnumerable<PixelInfo> drawnPixels = LineDrawer.DrawLinePoints(pixelFrom, pixelTo);

            foreach (PixelInfo pixel in drawnPixels)
            {
                sidesList.Add(pixel);

                var point = pixel.Point;

                if (point.X > 0 && point.X < ZBuf.Width && point.Y > 0 && point.Y < ZBuf.Height)
                {
                    if (point.Z <= ZBuf[(int)point.X, (int)point.Y])
                    {
                        var world4 = pixel.World / pixel.World.W;
                        var world3 = new Vector3(world4.X, world4.Y, world4.Z);
                        Color color = Light.GetPointColor(pixel.Normal, lightVector, viewVector - world3);
                        ZBuf[(int)point.X, (int)point.Y] = point.Z;
                        bitmap[(int)point.X, (int)point.Y] = color;
                    }
                }
            }
        }

        protected void FindMinAndMaxY(List<PixelInfo> sidesList, out int min, out int max)
        {
            var list = sidesList.OrderBy(x => (int)x.Point.Y).ToList();
            min = (int)list[0].Point.Y;
            max = (int)list[sidesList.Count - 1].Point.Y;
        }

        protected void FindStartAndEndXByY(List<PixelInfo> sidesList, int y, out PixelInfo pixelFrom, out PixelInfo pixelTo)
        {
            List<PixelInfo> sameYList = sidesList
                .Where(x => (int)x.Point.Y == y)
                .OrderBy(x => (int)x.Point.X)
                .ToList();

            pixelFrom = sameYList[0];
            pixelTo = sameYList[sameYList.Count - 1];
        }

        protected void DrawPixelForRasterization(List<PixelInfo> sidesList, Vector3 lightVector, Vector3 viewVector)
        {
            int minY, maxY;
            PixelInfo pixelFrom, pixelTo;
            FindMinAndMaxY(sidesList, out minY, out maxY);

            for (int y = minY + 1; y < maxY; y++)
            {
                FindStartAndEndXByY(sidesList, y, out pixelFrom, out pixelTo);

                IEnumerable<PixelInfo> drawnPixels = LineDrawer.DrawLinePoints(pixelFrom, pixelTo);

                foreach (PixelInfo pixel in drawnPixels)
                {
                    var point = pixel.Point;

                    if (point.X > 0 && point.X < ZBuf.Width && point.Y > 0 && point.Y < ZBuf.Height)
                    {
                        if (point.Z <= ZBuf[(int)point.X, (int)point.Y])
                        {
                            var world4 = pixel.World / pixel.World.W;
                            var world3 = new Vector3(world4.X, world4.Y, world4.Z);
                            Color color = Light.GetPointColor(pixel.Normal, lightVector, viewVector - world3);
                            ZBuf[(int)point.X, (int)point.Y] = point.Z;
                            bitmap[(int)point.X, (int)point.Y] = color;
                        }
                    }
                }
            }
        }

        protected bool IsFaceVisible(List<Vector3> face)
        {
            bool result = true;

            Vector3 normal = GetFaceNormal(face);

            if (normal.Z >= 0)
            {
                result = false;
            }

            return result;
        }

        protected Vector3 GetFaceNormal(List<Vector3> face)
        {
            int indexPoint1 = (int)Math.Round(face[0].X - 1);
            int indexPoint2 = (int)Math.Round(face[1].X - 1);
            int indexPoint3 = (int)Math.Round(face[2].X - 1);

            Vector3 point1 = windowVertices[indexPoint1];
            Vector3 point2 = windowVertices[indexPoint2];
            Vector3 point3 = windowVertices[indexPoint3];

            Vector3 vector1 = point2 - point1;
            Vector3 vector2 = point3 - point1;
            Vector3 vector1XYZ = new Vector3(vector1.X, vector1.Y, vector1.Z);
            Vector3 vector2XYZ = new Vector3(vector2.X, vector2.Y, vector2.Z);

            return Vector3.Normalize(Vector3.Cross(vector1XYZ, vector2XYZ));
        }
    }
}
