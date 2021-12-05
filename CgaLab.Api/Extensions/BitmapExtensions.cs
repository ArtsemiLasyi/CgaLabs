using CgaLab.Api.Bitmaps;
using System.Numerics;

namespace CgaLab.Api.Extensions
{
    public static class BitmapExtensions
    {
        public static Vector3 Bilinear(this ExtendedBitmap texture, float x, float y)
        {
            int x1 = (int)x;
            int y1 = (int)y;

            float deltaX = x - x1;
            float deltaY = y - y1;

            if (deltaX == 0 && deltaY == 0)
            {
                return texture.GetRGBVector(x1, y1);
            }

            if (deltaX == 0)
            {
                return (-deltaY + 1) * texture.GetRGBVector(x1, y1)
                    + deltaY * texture.GetRGBVector(x1, y1 + 1);
            }

            if (deltaY == 0)
            {
                return (-deltaX + 1) * texture.GetRGBVector(x1, y1)
                    + deltaX * texture.GetRGBVector(x1 + 1, y1);
            }

            Vector3 y1Vector = (-deltaX + 1) * texture.GetRGBVector(x1, y1)
                + deltaX * texture.GetRGBVector(x1 + 1, y1);
            Vector3 y2Vector = (-deltaX + 1) * texture.GetRGBVector(x1, y1 + 1)
                + deltaX * texture.GetRGBVector(x1 + 1, y1 + 1);
            return (-deltaX + 1) * y1Vector
                + deltaX * y2Vector;
        }
    }
}
