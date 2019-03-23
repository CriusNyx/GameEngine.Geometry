using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameEngine.Geometry
{
    /// <summary>
    /// This class provides extension methods for IBlocks.
    /// </summary>
    static class IBlockExtensions
    {
        /// <summary>
        /// Creates a mesh, and mesh renderer out of the IBlock.
        /// This will only generate faces that are Textured Polys, and have a material that is not null.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="gameObject"></param>
        public static void MakeMesh(this IBlock block, GameObject gameObject)
        {
            //Initialization
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            Dictionary<Material, List<int>> subMeshes = new Dictionary<Material, List<int>>();

            //itterate through each face, and add it to the mesh if the face has a material
            foreach (var face in block.GetFaces())
            {
                if (face is ITexturedPoly)
                {
                    //initialization
                    ITexturedPoly texPoly = face as ITexturedPoly;
                    Vector3 normal = texPoly.GetNormal().normalized;
                    int offset = vertices.Count;
                    int l = face.Resolution;

                    //generate verticies, uvs and normals
                    for (int i = 0; i < l; i++)
                    {
                        Vector3 v = texPoly.GetPoint(i);
                        Vector2 uv = texPoly.GetTextureMapping(v);
                        vertices.Add(v);
                        uvs.Add(uv);
                        normals.Add(normal);
                    }

                    //Get material, and add it to list
                    Material mat = texPoly.GetMaterial();
                    if (!subMeshes.ContainsKey(mat))
                        subMeshes.Add(mat, new List<int>());

                    //Generate triangles
                    List<int> triangles = subMeshes[mat];
                    for (int i = 1; i < texPoly.Resolution - 1; i++)
                    {
                        triangles.Add(offset);
                        triangles.Add(offset + i);
                        triangles.Add(offset + i + 1);
                    }
                }
            }

            //generate mesh filter
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null)
                filter = gameObject.AddComponent<MeshFilter>();

            //generate mesh
            Mesh mesh = filter.sharedMesh;
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            int index = 0;
            mesh.subMeshCount = subMeshes.Count;
            foreach (var pair in subMeshes)
            {
                mesh.SetIndices(pair.Value.ToArray(), MeshTopology.Triangles, index++);
            }
            mesh.normals = normals.ToArray();

            //generate mesh renderer
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer == null)
                renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.materials = subMeshes.Keys.ToArray();
        }

        public static void MakeMesh(this IBlock block, GameObject gameObject, Matrix4x4 transformation)
        {
            //Initialization
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            Dictionary<Material, List<int>> subMeshes = new Dictionary<Material, List<int>>();

            //itterate through each face, and add it to the mesh if the face has a material
            foreach (var face in block.GetFaces())
            {
                if (face is ITexturedPoly)
                {
                    //initialization
                    ITexturedPoly texPoly = face as ITexturedPoly;
                    Vector3 normal = texPoly.GetNormal().normalized;
                    int offset = vertices.Count;
                    int l = face.Resolution;

                    //generate verticies, uvs and normals
                    for (int i = 0; i < l; i++)
                    {
                        Vector3 v = texPoly.GetPoint(i);
                        Vector2 uv = texPoly.GetTextureMapping(v);
                        vertices.Add(transformation.MultiplyPoint(v));
                        uvs.Add(uv);
                        normals.Add(normal);
                    }

                    //Get material, and add it to list
                    Material mat = texPoly.GetMaterial();
                    if (!subMeshes.ContainsKey(mat))
                        subMeshes.Add(mat, new List<int>());

                    //Generate triangles
                    List<int> triangles = subMeshes[mat];
                    for (int i = 1; i < texPoly.Resolution - 1; i++)
                    {
                        triangles.Add(offset);
                        triangles.Add(offset + i);
                        triangles.Add(offset + i + 1);
                    }
                }
            }

            //generate mesh filter
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null)
                filter = gameObject.AddComponent<MeshFilter>();

            //generate mesh
            Mesh mesh = filter.sharedMesh;
            if (mesh == null)
                mesh = filter.mesh;
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            int index = 0;
            mesh.subMeshCount = subMeshes.Count;
            foreach (var pair in subMeshes)
            {
                mesh.SetIndices(pair.Value.ToArray(), MeshTopology.Triangles, index++);
            }
            mesh.normals = normals.ToArray();

            //generate mesh renderer
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer == null)
                renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.materials = subMeshes.Keys.ToArray();
        }

        public static void MakeMeshCrude(this IBlock block, GameObject gameObject)
        {
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            if(filter == null)
            {
                filter = gameObject.AddComponent<MeshFilter>();
            }
            if (filter.mesh == null)
                filter.mesh = new Mesh();
            block.MakeMeshCrude(filter.mesh);
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            if(renderer == null)
            {
                renderer = gameObject.AddComponent<MeshRenderer>();
            }
        }

        public static void MakeMeshCrude(this IBlock block, Mesh mesh)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            int offset = 0;

            foreach (var face in block)
            {
                foreach (Vector3 v in face.GetPoints())
                {
                    vertices.Add(v);
                    uvs.Add(Vector3.zero);
                    normals.Add(face.GetNormal());
                }
                for (int i = 1; i < face.Resolution - 1; i++)
                {
                    triangles.Add(offset);
                    triangles.Add(offset + i);
                    triangles.Add(offset + i + 1);
                }
                offset += face.Resolution;
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = normals.ToArray();
        }
    }
}