﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace CgaLab.Api.Bitmaps
{
    public class BitmapDrawer
    {
        private ExtendedBitmap bitmap;
        private List<Vector3> windowVertices;
        private Color activeColor = Color.Green;

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

        public BitmapDrawer(int width, int height)
        {
            bitmap = new ExtendedBitmap(width, height);
        }

        public void SetActiveColor(Color color)
        {
            activeColor = color;
        }

        public Bitmap GetBitmap(List<Vector3> windowVertices, WatchModel model)
        {
            int width = Width;
            int height = Height;
            bitmap = new ExtendedBitmap(width, height);

            this.windowVertices = windowVertices;

            bitmap.LockBits();

            foreach (List<Vector3> vertexIndexes in model.Poligons)
            {
                for (int i = 0; i < vertexIndexes.Count - 1; i++)
                {
                    DrawLine(i, i + 1, vertexIndexes);
                }

                DrawLine(0, vertexIndexes.Count - 1, vertexIndexes);
            }

            bitmap.UnlockBits();

            return bitmap.Source;
        }

        private void DrawLine(int from, int to, List<Vector3> indexes)
        {
            int indexFrom = (int)indexes[from].X - 1;
            int indexTo = (int)indexes[to].X - 1;

            Vector3 vertexFrom = windowVertices[indexFrom];
            Vector3 vertexTo = windowVertices[indexTo];

            Point pointFrom = GetPoint(vertexFrom);
            Point pointTo = GetPoint(vertexTo);

            IEnumerable<Point> drawnPoints = LineDrawer.DrawLinePoints(pointFrom, pointTo);

            foreach (Point point in drawnPoints)
            {
                if (IsVisiblePoint(point))
                {
                    bitmap[point.X, point.Y] = activeColor;
                }
            }
        }

        private Point GetPoint(Vector3 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        private bool IsVisiblePoint(Point point)
        {
            return ((point.X > 0) && (point.X < bitmap.Width)
               && (point.Y > 0) && (point.Y < bitmap.Height));
        }

        public void Clear()
        {
            bitmap = new ExtendedBitmap(bitmap.Width, bitmap.Height);
        }
    }
}