using System;

namespace CgaLab.Api
{
    public static class AngleConverter
    {
        public static float GetRadians(float degree)
        {
            return (float)(degree * Math.PI / 180f);
        }

        public static float GetDegrees(float radians)
        {
            return (float)(radians * 180f / Math.PI);
        }
    }
}
