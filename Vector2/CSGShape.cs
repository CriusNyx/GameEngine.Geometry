using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Geometry;

namespace GameEngine.CSG2D
{
    /// <summary>
    /// A shape with an RValue.
    /// The basis of a CSGVector2
    /// </summary>
    public class CSGShape : IPoly, IPolyForwarder
    {
        public IPoly poly;
        public float rValue;

        public CSGShape(IPoly poly, float rValue = 1f)
        {
            this.poly = poly;
            this.rValue = rValue;
        }

        #region Interfaces
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

        public Vector3[] GetPoints()
        {
            return poly.GetPoints();
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
            return new CSGShape(poly.Clone(), rValue);
        }

        public IPoly Clone(IEnumerable<Vector3> points)
        {
            return new CSGShape(poly.Clone(points), rValue);
        }
        #endregion

        public IPoly GetPoly()
        {
            return poly;
        }
        #endregion

        public static CSGShape operator *(float a, CSGShape b)
        {
            return new CSGShape(b.poly.Clone(), a * b.rValue);
        }
    }
}