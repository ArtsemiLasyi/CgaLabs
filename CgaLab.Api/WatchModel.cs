using CgaLab.Api.ObjFormat;
using System;
using System.Collections.Generic;
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
            Scale = 500 / (max * 3);
        }

        public int GetMax()
        {
            int max = int.MinValue;
            foreach (Vector4 vertex in Vertixes)
            {
                int max1 = (int)Math.Max(vertex.X, vertex.Y);
                int max2 = (int)Math.Max(vertex.Z, vertex.W);
                max1 = (int)Math.Max(max1, max2);
                if (max1 > max)
                {
                    max = max1;
                }
            }
            return max;
        }
    }
}
