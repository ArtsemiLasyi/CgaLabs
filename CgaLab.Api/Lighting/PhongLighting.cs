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
            Color ambientColor
        )
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

        public Color GetPointColorWithTexture(
            Vector3 normal, 
            Vector3 light, 
            Vector3 view,
            WatchModel model, 
            Vector3 texture
        )
        {
            Vector3 normalVector = Vector3.Normalize(normal);
            Vector3 lightVector = Vector3.Normalize(light);
            Vector3 viewVector = Vector3.Normalize(view);

            float x = texture.X * model.DiffuseTexture.Width;
            float y = (1 - texture.Y) * model.DiffuseTexture.Height;

            x = x.Clamp(model.DiffuseTexture.Width);
            y = y.Clamp(model.DiffuseTexture.Height);
            if (x < 0 || y < 0)
            {
                return Color.FromArgb(255, 0, 0, 0);
            }

            Vector3 pointNormal;
            if (model.NormalsTexture != null)
            {
                pointNormal = model.NormalsTexture.Bilinear(x, y);
                pointNormal.X -= 127.5f;
                pointNormal.Y -= 127.5f;
                pointNormal.Z -= 127.5f;
                pointNormal = Vector3.Normalize(pointNormal);
                pointNormal = Vector3.Normalize(
                    Vector3.TransformNormal(pointNormal, model.worldMatrix)
                );
            }
            else
            {
                pointNormal = normalVector;
            }

            Vector3 Ia = model.DiffuseTexture.Bilinear(x, y)
                * ka;
            Vector3 Id = model.DiffuseTexture.Bilinear(x, y)
                * kd
                * Math.Max(Vector3.Dot(pointNormal, lightVector), 0);
            Vector3 Is;

            if (model.SpecularTexture != null)
            {
                Vector3 reflectionVector = Vector3.Normalize(
                    Vector3.Reflect(lightVector, pointNormal)
                );

                Is = model.SpecularTexture.Bilinear(x, y)
                    * ks
                    * (float)Math.Pow(
                        Math.Max(0, Vector3.Dot(reflectionVector, viewVector)),
                        alpha
                    );
            }
            else
            {
                Is = Vector3.Zero;
            }

            Vector3 color = Ia + Id + Is;

            byte r = color.X <= 255
                ? (byte)color.X
                : (byte)255;
            byte g = color.Y <= 255
                ? (byte)color.Y
                : (byte)255;
            byte b = color.Z <= 255
                ? (byte)color.Z
                : (byte)255;

            return Color.FromArgb(255, r, g, b);
        }
    }
}
