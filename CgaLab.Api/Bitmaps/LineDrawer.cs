using System;
using System.Collections.Generic;
using System.Numerics;

namespace CgaLab.Api.Bitmaps
{
    public static class LineDrawer
    {
        public static List<Pixel> DrawLinePoints(Pixel pixel1, Pixel pixel2)
        {
            List<Pixel> points = new List<Pixel>();

            Vector3 point1 = pixel1.Point;
            Vector3 point2 = pixel2.Point;
            
            Vector3 normal1 = pixel1.Normal;
            Vector4 world1 = pixel1.World;
            Vector3 texture1 = pixel1.Texture;
            
            int deltaX, deltaY;
            int signX = 1, signY = 1;
            
            float deltaZ;
            
            Vector3 deltaN;
            Vector4 deltaW;
            Vector3 deltaT;

            deltaX = Math.Abs((int)point2.X - (int)point1.X);
            deltaY = Math.Abs((int)point2.Y - (int)point1.Y);
            
            deltaZ = point2.Z - point1.Z;
            
            deltaN = pixel2.Normal - pixel1.Normal;
            deltaW = pixel2.World - pixel1.World;
            deltaT = pixel2.Texture - pixel1.Texture;


            if (deltaX > deltaY)
            {
                deltaZ /= deltaX;
                deltaN /= deltaX;
                deltaW /= deltaX;
                deltaT /= deltaX;
            }
            else
            {
                deltaZ /= deltaY;
                deltaN /= deltaY;
                deltaW /= deltaY;
                deltaT /= deltaY;
            }

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
                points.Add(new Pixel()
                {
                    Point = new Vector3(point1.X, point1.Y, point1.Z),
                    Normal = normal1,
                    World= world1,
                    Texture = texture1
                });

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

                point1.Z += deltaZ;
                normal1 += deltaN;
                world1 += deltaW;
                texture1 += deltaT;
            }

            points.Add(new Pixel()
            {
                Point = new Vector3(point1.X, point1.Y, point1.Z),
                Normal = normal1,
                World = world1,
                Texture = texture1
            });
            return points;
        }
    }
}
