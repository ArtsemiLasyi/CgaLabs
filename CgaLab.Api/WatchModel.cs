using CgaLab.Api.ObjFormat;
using System.Collections.Generic;
using System.Numerics;

namespace CgaLab.Api
{
    public class WatchModel
    {
        public List<Vector4> Vertices = new List<Vector4>();
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

            Vertices = objModel.V;
            Textures = objModel.Vt;
            Normals = objModel.Vn;
            Poligons = objModel.F;
        }
    }
}
