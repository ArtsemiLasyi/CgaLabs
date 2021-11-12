using System.Collections.Generic;
using System.Numerics;
using System;
using System.Drawing;
using System.Linq;

namespace CgaLab.Api.Bitmaps
{
    public abstract class BitmapDrawer
    {
        protected ExtendedBitmap bitmap;
        protected List<Vector3> windowVertices;
        public ZBuffer ZBuf { get; protected set; }
        protected Color activeColor = Color.Green;
        protected WatchModel model;

        protected bool IsPoligonVisible(List<Vector3> poligon)
        {
            bool result = true;

            Vector3 normal = GetPoligonNormal(poligon);

            if (normal.Z >= 0)
            {
                result = false;
            }

            return result;
        }

        protected void FindStartAndEndXByY(List<Pixel> sidesList, int y, out Pixel pixelFrom, out Pixel pixelTo)
        {
            List<Pixel> sameYList = sidesList
                .Where(x => (int)x.Point.Y == y)
                .OrderBy(x => (int)x.Point.X)
                .ToList();

            pixelFrom = sameYList[0];
            pixelTo = sameYList[sameYList.Count - 1];
        }

        protected void FindMinAndMaxY(List<Pixel> sidesList, out int min, out int max)
        {
            var list = sidesList.OrderBy(x => (int)x.Point.Y).ToList();
            min = (int)list[0].Point.Y;
            max = (int)list[sidesList.Count - 1].Point.Y;
        }

        protected Vector3 GetPoligonNormal(List<Vector3> Poligon)
        {
            int indexPoint1 = (int)Math.Round(Poligon[0].X - 1);
            int indexPoint2 = (int)Math.Round(Poligon[1].X - 1);
            int indexPoint3 = (int)Math.Round(Poligon[2].X - 1);

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
