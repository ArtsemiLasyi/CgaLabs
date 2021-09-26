using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

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

        public Bitmap GetBitmap(List<Vector3> windowVertices, WatchModel model)
        {
            int width = Width;
            int height = Height;
            bitmap = new ExtendedBitmap(width, height);

            this.windowVertices = windowVertices;

            bitmap.LockBits();
            Parallel.ForEach(Partitioner.Create(0, model.Poligons.Count), range => {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    DrawLines(model.Poligons[i]);
                }
            });
          

            bitmap.UnlockBits();

            return bitmap.Source;
        }

        private void DrawLines(List<Vector3> vertexIndexes)
        {
            for (int i = 0; i < vertexIndexes.Count - 1; i++)
            {
                DrawLine(i, i + 1, vertexIndexes);
            }

            DrawLine(0, vertexIndexes.Count - 1, vertexIndexes);
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
                if ((point.X > 0) && (point.X < bitmap.Width)
                    && (point.Y > 0) && (point.Y < bitmap.Height))
                {
                    bitmap[point.X, point.Y] = activeColor;
                }
            }
        }

        private Point GetPoint(Vector3 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
    }
}
