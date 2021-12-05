using System;

namespace CgaLab.Api.Extensions
{
    public static class FloatExtensions
    {
        public static float Clamp(this float coordinate, float parameter)
        {
            if (coordinate < 0)
            {
                return 0;
            }

            if (coordinate > parameter)
            {
                return parameter - 1;
            }
            return coordinate;
        }
    }
}
