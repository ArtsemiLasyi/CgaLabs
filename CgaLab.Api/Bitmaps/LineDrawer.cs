using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace CgaLab.Api.Bitmaps
{
    public static class LineDrawer
    {
        public static IEnumerable<Point> DrawLinePoints(Point point1, Point point2)
        {
            int deltaX, deltaY;
            int signX = 1, signY = 1;

            deltaX = Math.Abs(point2.X - point1.X);
            deltaY = Math.Abs(point2.Y - point1.Y);

            if (point1.X > point2.X)
            {
                signX = -1;
            }
            if (point1.Y > point2.Y)
            {
                signY = -1;
            }

            int error = deltaX - deltaY;

            while (point1.X != point2.X || point1.Y != point2.Y)
            {
                yield return new Point(point1.X, point1.Y);

                int error2 = error * 2;

                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    point1.X += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    point1.Y += signY;
                }
            }
            yield return new Point(point2.X, point2.Y);
        }
    }
}
