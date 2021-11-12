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
    public class LambertBitmapDrawer : BitmapDrawer
    {
        private LambertLighting Light { get; set; }

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

        public LambertBitmapDrawer(int width, int height)
        {
            bitmap = new ExtendedBitmap(width, height);
            ZBuf = new ZBuffer(bitmap.Width, bitmap.Height);
            Light = new LambertLighting(activeColor);
        }

        public Bitmap GetBitmap(List<Vector3> windowVertices, WatchModel model, Vector3 lightVector)
        {
            int width = Width;
            int height = Height;
            bitmap = new ExtendedBitmap(width, height);
            ZBuf = new ZBuffer(bitmap.Width, bitmap.Height);

            this.windowVertices = windowVertices;
            this.model = model;

            bitmap.LockBits();

            DrawAllPixels(lightVector);

            bitmap.UnlockBits();

            return bitmap.Source;
        }

        private void DrawAllPixels(Vector3 lightVector)
        {
            List<List<Vector3>> poligonsList = model.Poligons;

            Parallel.ForEach(Partitioner.Create(0, poligonsList.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    List<Vector3> poligon = poligonsList[i];
                    if (IsPoligonVisible(poligon))
                    {
                        DrawPoligon(poligon, lightVector);
                    }
                }
            });
        }

        protected void DrawPoligon(List<Vector3> vertexIndexes, Vector3 lightVector)
        {
            List<Pixel> sidesList = new List<Pixel>();
            Color color = GetColorForPoligon(vertexIndexes, lightVector);

            for (int i = 0; i < vertexIndexes.Count - 1; i++)
            {
                DrawLine(i, i + 1, vertexIndexes, color, sidesList);
            }

            DrawLine(vertexIndexes.Count - 1, 0, vertexIndexes, color, sidesList);

            DrawPixelForRasterization(sidesList, color);
        }

        private Color GetColorForPoligon(List<Vector3> poligon, Vector3 lightVector)
        {
            List<Color> colors = new List<Color>();
            foreach (Vector3 index in poligon)
            {
                int normalIndex = (int)index.Z - 1;
                Color pointColor = Light.GetPointColor(
                    model.Normals[normalIndex],
                    lightVector
                );
                colors.Add(pointColor);
            }

            return Light.GetAverageColor(colors);
        }

        private void DrawLine(
            int from, 
            int to, 
            List<Vector3> indexes, 
            Color color, 
            List<Pixel> sidesList)
        {
            int indexFrom = (int)indexes[from].X - 1;
            int indexTo = (int)indexes[to].X - 1;

            Vector3 vertexFrom = windowVertices[indexFrom];
            Vector3 vertexTo = windowVertices[indexTo];

            Pixel pixelFrom = new Pixel()
            {
                Point = new Vector3(
                    (int)Math.Round(vertexFrom.X),
                    (int)Math.Round(vertexFrom.Y),
                    vertexFrom.Z)
            };
            Pixel pixelTo = new Pixel()
            {
                Point = new Vector3(
                    (int)Math.Round(vertexTo.X),
                    (int)Math.Round(vertexTo.Y),
                    vertexTo.Z)
            };

            IEnumerable<Pixel> drawnPixels = LineDrawer.DrawLinePoints(pixelFrom, pixelTo);

            foreach (Pixel pixel in drawnPixels)
            {
                sidesList.Add(pixel);

                DrawPixel(pixel, color);
            }
        }

        protected void DrawPixelForRasterization(List<Pixel> sidesList, Color color)
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
                    DrawPixel(pixel, color);
                }
            }
        }

        protected void DrawPixel(Pixel pixel, Color color)
        {
            Vector3 point = pixel.Point;

            if (point.X > 0
                && point.X < ZBuf.Width
                && point.Y > 0
                && point.Y < ZBuf.Height)
            {
                if (point.Z <= ZBuf[(int)point.X, (int)point.Y])
                {
                    ZBuf[(int)point.X, (int)point.Y] = point.Z;
                    bitmap[(int)point.X, (int)point.Y] = color;
                }
            }
        }
    }
}
