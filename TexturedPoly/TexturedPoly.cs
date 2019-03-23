using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Geometry.Advanced;

namespace GameEngine.Geometry
{
    /// <summary>
    /// An impelementation of textured poly, that wraps another polygon.
    /// </summary>
    public class TexturedPoly : ITexturedPoly, IPolyForwarder
    {
        IPoly poly;
        Material material;
        Matrix4x4 matrix;

        public TexturedPoly(IPoly poly, Material material, Matrix4x4 matrix)
        {
            this.poly = poly;
            this.material = material;
            this.matrix = matrix;
        }

        #region ITexturedPoly
        public Material GetMaterial()
        {
            return material;
        }
        public void SetMaterial(Material material)
        {
            this.material = material;
        }
        public Vector2 GetTextureMapping(Vector3 point)
        {
            return matrix.MultiplyPoint(point);
        }
        #endregion

        #region IDrawable
        public void Draw(float time = -1F)
        {
            poly.Draw(time);
        }

        public void Draw(Color color, float time = -1F)
        {
            poly.Draw(color, time);
        }
        #endregion

        #region IPoly
        public Vector3 GetPoint(int index)
        {
            return poly.GetPoint(index);
        }

        public int Resolution
        {
            get
            {
                return poly.Resolution;
            }
        }

        public IPoly Clone()
        {
            return new TexturedPoly(poly.Clone(), material, matrix);
        }

        public IPoly Clone(IEnumerable<Vector3> points)
        {
            return new TexturedPoly(poly.Clone(points), material, matrix);
        }
        #endregion

        public IPoly GetPoly()
        {
            return poly;
        }

        public Vector3[] GetPoints()
        {
            return poly.GetPoints();
        }
    }
}