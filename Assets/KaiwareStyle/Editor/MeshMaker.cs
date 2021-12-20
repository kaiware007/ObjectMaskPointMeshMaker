using System.Collections.Generic;
using UnityEngine;

namespace KaiwareStyle
{
    public class MeshMaker
    {

        protected virtual bool IsInShape(Vector3 pos)
        {
            return true;
        }

        public Vector3[] GenerateVertices(int xnum, int ynum, int znum, Vector3 center, Vector3 scale, Vector3 offset, bool isFill = true)
        {
            int vertexCount = xnum * ynum * znum;

            int n = isFill ? 1 : 0;
            float xdiv = 1f / (xnum - n);
            float ydiv = 1f / (ynum - n);
            float zdiv = 1f / (znum - n);

            List<Vector3> vertices = new List<Vector3>();

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 pos = new Vector3((i % xnum) * xdiv, (i / xnum % ynum) * ydiv, (i / (xnum * ynum) % znum) * zdiv) + center;
                pos.Scale(scale);
                pos += offset;

                if (IsInShape(pos))
                {
                    pos -= offset;
                    vertices.Add(pos);
                }
            }

            return vertices.ToArray();
        }

        public Mesh GenerateMesh(int xnum, int ynum, int znum, Vector3 center, Vector3 scale, Vector3 offset, bool isFill = true)
        {
            Mesh mesh = new Mesh();

            List<int> indices = new List<int>();
            int index = 0;

            Vector3[] vertices = GenerateVertices(xnum, ynum, znum, center, scale, offset, isFill);

            for (int i = 0; i < vertices.Length; i++)
            {
                indices.Add(index++);
            }
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}