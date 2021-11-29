using CgaLab.Api.Extensions;
using System;
using System.Drawing;
using System.Numerics;

namespace CgaLab.Api.Lighting
{
    public class PhongLighting
    {
        private Color objectColor;     // цвет рассеянного света
        private Color lightColor;      // цвет зеркального света
        private Color ambientColor;    // цвет фонового освещения

        private readonly int alpha = 32;   // коэффициент блеска поверхности
        
        private readonly float ka = 0.3f;  // коэффициент фонового освещения
        private readonly float kd = 0.9f;  // коэффициент рассеянного освещения
        private readonly float ks = 0.3f;  // коэффициент зеркального освещения

        public PhongLighting(
            Color objectColor,
            Color lightColor,
            Color ambientColor)
        {
            this.objectColor = objectColor;
            this.lightColor = lightColor;
            this.ambientColor = ambientColor;
        }

        public Color GetPointColor(Vector3 normal, Vector3 light, Vector3 view)
        {
            Vector3 normalVector = Vector3.Normalize(normal);
            Vector3 lightVector = Vector3.Normalize(light);
            Vector3 viewVector = Vector3.Normalize(view);

            Vector3 Ia = ka * ambientColor.ToVector3();
            Vector3 Id = kd * Math.Max(Vector3.Dot(normalVector, lightVector), 0) * objectColor.ToVector3();

            Vector3 reflectVector = Vector3.Normalize(Vector3.Reflect(-lightVector, normalVector));
            
            Vector3 Is = ks
                * (float)Math.Pow(
                    Math.Max(
                        0, 
                        Vector3.Dot(reflectVector, viewVector)
                    ), 
                    alpha
                 )
                * lightColor.ToVector3();

            Vector3 color = Ia + Id + Is;

            byte red = 
                color.X <= 255
                    ? (byte)color.X
                    : (byte)255;
            byte green = 
                color.Y <= 255
                    ? (byte)color.Y
                    : (byte)255;
            byte blue = 
                color.Z <= 255 
                    ? (byte)color.Z
                    : (byte)255;

            return Color.FromArgb(255, red, green, blue);
        }
    }
}
