using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CgaLab.Api.ObjFormat
{
    public class ObjParser
    {
        public async Task<ObjModel> ParseAsync(string path)
        {
            char[] separator = Environment.NewLine.ToCharArray();

            using StreamReader reader = new StreamReader(path);
            string text = await reader.ReadToEndAsync();
            List<string> lines = text.Split(separator).ToList();
            return GetModel(lines);
        }

        public ObjModel GetModel(List<string> lines)
        {
            ObjModel model = new ObjModel();
            lines.ForEach(
                line =>
                {
                    if (line.Contains(Constants.Comment ))
                    {
                        return;
                    }
                    if (line.StartsWith(Constants.VertexTexture + " "))
                    {
                        model.Vt.Add(GetVt(line));
                        return;
                    }
                    if (line.StartsWith(Constants.VertexNormal + " "))
                    {
                        model.Vn.Add(GetVn(line));
                        return;
                    }
                    if (line.StartsWith(Constants.Polygon + " "))
                    {
                        model.F.Add(GetF(line));
                        return;
                    }
                    if (line.StartsWith(Constants.Vertex + " "))
                    {
                        model.V.Add(GetV(line));
                        return;
                    }
                }
            );
            return model;
        }

        public List<Vector3> GetF(string line)
        {
            List<Vector3> values = new List<Vector3>();
            string[] vectors = line
                .Remove(0, Constants.Polygon.Length + 1)
                .Trim()
                .Split(" ");
            foreach (string vector in vectors)
            {
                string[] coordinates = vector.Split("/");
                if (coordinates.Length == 2)
                {
                    values.Add(
                        new Vector3(
                            int.Parse(coordinates[0]),
                            int.Parse(coordinates[1]),
                            0
                        )
                    );
                }
                if (coordinates.Length == 3)
                {
                    if (string.IsNullOrEmpty(coordinates[1]))
                    {
                        values.Add(
                            new Vector3(
                                int.Parse(coordinates[0]),
                                0,
                                int.Parse(coordinates[2])
                            )
                        );
                    }
                    else
                    {
                        values.Add(
                            new Vector3(
                                int.Parse(coordinates[0]),
                                int.Parse(coordinates[1]),
                                int.Parse(coordinates[2])
                            )
                        );
                    }
                }
            }
            return values;
        }

        public Vector3 GetVt(string line)
        {
            string[] coordinates = line
                .Remove(0, Constants.VertexTexture.Length + 1)
                .Trim()
                .Split(" ");
            if (coordinates.Length == 3)
            {
                return new Vector3(
                    float.Parse(coordinates[0]),
                    float.Parse(coordinates[1]),
                    float.Parse(coordinates[2])
                );
            }
            if (coordinates.Length == 2)
            {
                return new Vector3(
                    float.Parse(coordinates[0]),
                    float.Parse(coordinates[1]),
                    1
                );
            }
            return new Vector3(
                float.Parse(coordinates[0]),
                1,
                1
            );
        }

        public Vector4 GetV(string line)
        {
            string[] coordinates = line
                .Remove(0, Constants.Vertex.Length + 1)
                .Trim()
                .Split(" ");
            if (coordinates.Length == 4)
            {
                return new Vector4(
                    float.Parse(coordinates[0]),
                    float.Parse(coordinates[1]),
                    float.Parse(coordinates[2]),
                    float.Parse(coordinates[3])
                );
            }
            return new Vector4(
                float.Parse(coordinates[0]),
                float.Parse(coordinates[1]),
                float.Parse(coordinates[2]),
                1
            );
        }

        public Vector3 GetVn(string line)
        {
            string[] coordinates = line
                .Remove(0, Constants.VertexNormal.Length + 1)
                .Trim()
                .Split(" ");
            return new Vector3(
                float.Parse(coordinates[0]),
                float.Parse(coordinates[1]),
                float.Parse(coordinates[2])
            );
        }
    }
}
