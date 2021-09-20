using System.Collections.Generic;
using System.Numerics;

namespace CgaLab.Api.ObjFormat
{
    public class ObjModel
    {
        public List<Vector4> V = new List<Vector4>();
        public List<Vector3> Vt = new List<Vector3>();
        public List<Vector3> Vn = new List<Vector3>();
        public List<List<Vector3>> F = new List<List<Vector3>>();
    }
}
