using System;
using System.Drawing;
using System.Numerics;

namespace CgaLab.Api.Lighting
{
    public class PhongLighting
    {
        private Color objectColor;
        private Color lightColor;
        private Color ambientColor;

        public PhongLighting(
            Color objectColor,
            Color lightColor,
            Color ambientColor)
        {
            this.objectColor = objectColor;
            this.lightColor = lightColor;
            this.ambientColor = ambientColor;
        }

        public Color GetPointColor(Vector3 normal, Vector3 light)
        {
            throw new NotImplementedException();
        }

        public Color GetPointColor(Vector3 normal, Vector3 light, Vector3 view)
        {
            var normalVector = Vector3.Normalize(normal);
            var lightVector = Vector3.Normalize(light);
            var viewVector = Vector3.Normalize(view);

            Vector3 Ia = 0.1f * ambientColor.ToVector3();
            Vector3 Id = Math.Max(Vector3.Dot(normalVector, lightVector), 0) * objectColor.ToVector3();
            Vector3 reflectVector = Vector3.Normalize(Vector3.Reflect(-lightVector, normalVector));
            Vector3 Is = 0.1f * (float)Math.Pow(Math.Max(0, Vector3.Dot(reflectVector, viewVector)), 32) * lightColor.ToVector3();
            Vector3 color = Ia + Id + Is;

            byte r = color.X <= 255 ? (byte)color.X : (byte)255;
            byte g = color.Y <= 255 ? (byte)color.Y : (byte)255;
            byte b = color.Z <= 255 ? (byte)color.Z : (byte)255;

            return Color.FromArgb(255, r, g, b);
        }
    }

    public static class ColorExtension
    {
        public static Vector3 ToVector3(this Color color)
        {
            return new Vector3(color.R, color.G, color.B);
        }
    }
}
