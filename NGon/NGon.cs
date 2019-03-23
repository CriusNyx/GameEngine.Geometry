using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Geometry.Advanced;
using System.Linq;

namespace GameEngine.Geometry {
    /// <summary>
    /// An implementation of IPoly, which optimizes over many of IPolys features, trading off higher memory use for lower computation time.
    /// This class is idea when constants such as area, and perimeter need to be referenced often
    /// </summary>
    public class NGon : IPoly, IPolyExtensionArea, IPolyExtensionNormal, IPolyExtensionPerimeter, IPolyExtensionSurfaceNormal
    {
        Vector3[] points;

        public NGon()
        {

        }

        public NGon(params Vector3[] points)
        {
            this.points = points;
        }

        public NGon(IEnumerable<Vector3> points)
        {
            this.points = points.ToArray();
        }

        public NGon(IPoly poly)
        {
            int r = poly.Resolution;
            points = new Vector3[r];
            for(int i = 0; i < r; i++)
            {
                points[i] = poly.GetPoint(i);
            }
        }

        #region Poly
        public Vector3 GetPoint(int index)
        {
            return points[index];
        }

        public Vector3[] GetPoints()
        {
            return points;
        }

        public int Resolution
        {
            get
            {
                return points.Length;
            }
        }

        public IPoly Clone()
        {
            return new NGon(points);
        }

        public IPoly Clone(IEnumerable<Vector3> points)
        {
            return new NGon(points);
        }
        #endregion

        #region PolyAdv
        float area = -1f;
        public float AdvGetArea()
        {
            if(area == -1f)
            {
                area = this.CalcArea();
            }
            return area;
        }

        float perimeter = -1f;
        public float AdvGetPerimeter()
        {
            if (perimeter == -1f) perimeter = this.CalcPerimeter();
            return perimeter;
        }

        bool calcNormal = false;
        Vector3 normal;
        public Vector3 AdvGetNormal()
        {
            if (!calcNormal)
            {
                normal = this.CalcNormal();
                calcNormal = true;
            }
            return normal;
        }

        bool[] calcSurfaceNormals;
        Vector3[] surfaceNormal;
        public Vector3 AdvGetSurfaceNormal(int index)
        {
            if(calcSurfaceNormals == null)
            {
                calcSurfaceNormals = new bool[Resolution];
                surfaceNormal = new Vector3[Resolution];
            }
            if (!calcSurfaceNormals[index])
            {
                calcSurfaceNormals[index] = true;
                surfaceNormal[index] = this.CalcSurfaceNormal(index);
            }
            return surfaceNormal[index];
        }
        #endregion

        #region Drawable
        public void Draw(float time = -1f)
        {
            Draw(Color.white, time);
        }

        public void Draw(Color color, float time = -1f)
        {
            this.DrawPoly(color, time);
        }
        #endregion

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            foreach(var point in points)
            {
                sb.Append(point.ToString());
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            return sb.ToString();
        }
    }
}