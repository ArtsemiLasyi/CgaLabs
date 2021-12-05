using CgaLab.Api.Lighting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace CgaLab.Api.Bitmaps
{
    public class TextureBitmapDrawer : BitmapDrawer
    {
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

        public TextureBitmapDrawer(int width, int height)
        {
            bitmap = new ExtendedBitmap(width, height);
            ZBuf = new ZBuffer(bitmap.Width, bitmap.Height);
            Light = new PhongLighting(activeColor, Color.WhiteSmoke, Color.DarkGreen);
        }

        public Bitmap GetBitmap(
            List<Vector3> windowVertices, 
            WatchModel model, 
            Vector3 lightVector, 
            Vector3 viewVector
        )
        {
            int width = Width;
            int height = Height;
            bitmap = new ExtendedBitmap(width, height);
            ZBuf = new ZBuffer(bitmap.Width, bitmap.Height);

            this.windowVertices = windowVertices;
            this.model = model;

            bitmap.LockBits();
            model.DiffuseTexture?.LockBits();
            model.NormalsTexture?.LockBits();
            model.SpecularTexture?.LockBits();

            DrawAllPixels(lightVector, viewVector);

            model.DiffuseTexture?.UnlockBits();
            model.NormalsTexture?.UnlockBits();
            model.SpecularTexture?.UnlockBits();
            bitmap.UnlockBits();

            return bitmap.Source;
        }

        private void DrawAllPixels(Vector3 lightVector, Vector3 viewVector)
        {
            List<List<Vector3>> poligonsList = model.Poligons;

            Parallel.ForEach(
                Partitioner.Create(0, poligonsList.Count), 
                range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        List<Vector3> poligon = poligonsList[i];
                        if (IsPoligonVisible(poligon))
                        {
                            DrawPoligon(poligon, lightVector, viewVector);
                        }
                    }
                }
            );
        }

        protected void DrawPoligon(List<Vector3> vertexIndexes, Vector3 lightVector, Vector3 viewVector)
        {
            List<Pixel> sidesList = new List<Pixel>();

            for (int i = 0; i < vertexIndexes.Count - 1; i++)
            {
                DrawLine(i, i + 1, vertexIndexes, sidesList, lightVector, viewVector);
            }

            DrawLine(vertexIndexes.Count - 1, 0, vertexIndexes, sidesList, lightVector, viewVector);

            DrawPixelForRasterization(sidesList, lightVector, viewVector);
        }

        private void DrawLine(
            int from,
            int to,
            List<Vector3> indexes,
            List<Pixel> sidesList,
            Vector3 lightVector,
            Vector3 viewVector)
        {
            int indexFrom = (int)indexes[from].X - 1;
            int indexTo = (int)indexes[to].X - 1;

            int indexNormalFrom = (int)indexes[from].Z - 1;
            int indexNormalTo = (int)indexes[to].Z - 1;

            int textureIndexFrom = (int)indexes[from].Y - 1;
            int textureIndexTo = (int)indexes[to].Y - 1;

            Vector3 vertexFrom = windowVertices[indexFrom];
            Vector3 vertexTo = windowVertices[indexTo];

            Pixel pixelFrom = new Pixel()
            {
                Point = new Vector3(
                    (int)Math.Round(vertexFrom.X),
                    (int)Math.Round(vertexFrom.Y),
                    vertexFrom.Z
                ),
                Normal = model.Normals[indexNormalFrom],
                World = model.Vertixes[indexFrom],
                Texture = model.Textures[textureIndexFrom]
            };
            Pixel pixelTo = new Pixel()
            {
                Point = new Vector3(
                    (int)Math.Round(vertexTo.X),
                    (int)Math.Round(vertexTo.Y),
                    vertexTo.Z
                ),
                Normal = model.Normals[indexNormalTo],
                World = model.Vertixes[indexTo],
                Texture = model.Textures[textureIndexTo]
            };

            IEnumerable<Pixel> drawnPixels = LineDrawer.DrawLinePoints(pixelFrom, pixelTo);

            foreach (Pixel pixel in drawnPixels)
            {
                sidesList.Add(pixel);

                DrawPixel(pixel, lightVector, viewVector);
            }
        }

        protected void DrawPixelForRasterization(List<Pixel> sidesList, Vector3 lightVector, Vector3 viewVector)
        {
            int minY, maxY;
            Pixel pixelFrom, pixelTo;
            FindMinAndMaxY(sidesList, out minY, out maxY);

            for (int y = minY + 1; y < maxY; y++)
            {
                FindStartAndEndXByY(sidesList, y, out pixelFrom, out pixelTo);

                IEnumerable<Pixel> drawnPixels = LineDrawer.DrawLinePoints(pixelFrom, pixelTo);

                foreach (Pixel pixel in drawnPixels)
                {
                    DrawPixel(pixel, lightVector, viewVector);
                }
            }
        }

        protected void DrawPixel(Pixel pixel, Vector3 lightVector, Vector3 viewVector)
        {
            Vector3 point = pixel.Point;

            if (point.X > 0
                && point.X < ZBuf.Width
                && point.Y > 0
                && point.Y < ZBuf.Height)
            {
                if (point.Z <= ZBuf[(int)point.X, (int)point.Y])
                {
                    Vector4 world4 = pixel.World / pixel.World.W;
                    Vector3 world3 = new Vector3(world4.X, world4.Y, world4.Z);

                    Color color = Light.GetPointColorWithTexture(
                        pixel.Normal, 
                        lightVector, 
                        viewVector - world3,
                        model, 
                        pixel.Texture
                    );

                    ZBuf[(int)point.X, (int)point.Y] = point.Z;
                    bitmap[(int)point.X, (int)point.Y] = color;
                }
            }
        }
    }
}
