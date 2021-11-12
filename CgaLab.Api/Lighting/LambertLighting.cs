using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace CgaLab.Api.Lighting
{
    public class LambertLighting
    {
        private Color objectColor;  // цвет рассеянного света

        private readonly float kd = 0.9f;  // коэффициент рассеянного освещения

        public LambertLighting(Color color)
        {
            objectColor = color;
        }

        public Color GetPointColor(Vector3 normalVector, Vector3 lightVector)
        {
            Vector3 normal = Vector3.Normalize(normalVector);
            Vector3 light = Vector3.Normalize(lightVector);

            double coefficient = Math.Max(
                Vector3.Dot(normal, light),
                0
            );
            byte red = 
                (Math.Round(objectColor.R * coefficient) <= 255) 
                    ? (byte)Math.Round(objectColor.R * coefficient) 
                    : (byte)255;
            byte green = 
                (Math.Round(objectColor.G * coefficient) <= 255) 
                    ? (byte)Math.Round(objectColor.G * coefficient) 
                    : (byte)255;
            byte blue = 
                (Math.Round(objectColor.B * coefficient) <= 255)
                    ? (byte)Math.Round(objectColor.B * coefficient)
                    : (byte)255;

            return Color.FromArgb(255, red, green, blue);
        }

        public Color GetAverageColor(IEnumerable<Color> colors)
        {
            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            int sumA = 0;

            foreach (Color color in colors)
            {
                sumR += color.R;
                sumG += color.G;
                sumB += color.B;
                sumA += color.A;
            }

            int count = colors.Count();

            byte red = (byte)Math.Round((double)sumR / count);
            byte green = (byte)Math.Round((double)sumG / count);
            byte blue = (byte)Math.Round((double)sumB / count);
            byte alpha = (byte)Math.Round((double)sumA / count);

            return Color.FromArgb(alpha, red, green, blue);
        }
    }
}
