using System.Collections.Generic;
using System.Numerics;

namespace CgaLab.Api
{
    public class DrawModel
    {
        public List<Vector4> V = new List<Vector4>();
        public List<Vector3> Vt = new List<Vector3>();
        public List<Vector3> Vn = new List<Vector3>();
        public List<Vector<Vector3>> F = new List<Vector<Vector3>>();
    }
}
