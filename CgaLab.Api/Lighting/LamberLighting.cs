using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace CgaLab.Api.Lighting
{
    public class LamberLighting
    {
        private Color ObjectColor { get; set; }

        public LamberLighting(Color color)
        {
            ObjectColor = color;
        }

        public Color GetPointColor(Vector3 normalVector, Vector3 lightVector)
        {
            Vector3 normal = Vector3.Normalize(normalVector);
            Vector3 light = Vector3.Normalize(lightVector);

            double coef = Math.Max(Vector3.Dot(normal, light), 0);
            byte r = (Math.Round(ObjectColor.R * coef) <= 255) ? (byte)Math.Round(ObjectColor.R * coef) : (byte)255;
            byte g = (Math.Round(ObjectColor.G * coef) <= 255) ? (byte)Math.Round(ObjectColor.G * coef) : (byte)255;
            byte b = (Math.Round(ObjectColor.B * coef) <= 255) ? (byte)Math.Round(ObjectColor.B * coef) : (byte)255;

            return Color.FromArgb(255, r, g, b);
        }

        public Color GetAverageColor(IEnumerable<Color> colors)
        {
            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            int sumA = 0;

            foreach (var color in colors)
            {
                sumR += color.R;
                sumG += color.G;
                sumB += color.B;
                sumA += color.A;
            }

            int count = colors.Count();
            byte r = (byte)Math.Round((double)sumR / count);
            byte g = (byte)Math.Round((double)sumG / count);
            byte b = (byte)Math.Round((double)sumB / count);
            byte a = (byte)Math.Round((double)sumA / count);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
