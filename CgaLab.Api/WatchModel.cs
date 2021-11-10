using CgaLab.Api.ObjFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CgaLab.Api
{
    public class WatchModel
    {
        public List<Vector4> Vertixes = new List<Vector4>();
        public List<Vector3> Textures = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<List<Vector3>> Poligons = new List<List<Vector3>>();

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public int Scale = 1;

        public WatchModel(ObjModel objModel)
        {
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;

            Vertixes = objModel.V;
            Textures = objModel.Vt;
            Normals = objModel.Vn;
            Poligons = objModel.F;
            int max = GetMax();
            Scale = 300 / max;
        }

        public int GetMax()
        {
            int deltaX = (int)Math.Abs(Vertixes.Max(v => v.X) - Vertixes.Min(v => v.X));
            int deltaY = (int)Math.Abs(Vertixes.Max(v => v.Y) - Vertixes.Min(v => v.Y));
            int deltaZ = (int)Math.Abs(Vertixes.Max(v => v.Z) - Vertixes.Min(v => v.Z));

            int max = Math.Max(deltaX, deltaY);
            max = Math.Max(max, deltaZ);
            return max;
        }
    }
}
