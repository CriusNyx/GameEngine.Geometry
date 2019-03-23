using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry {
    public class IPolyEdge : IEdge
    {
        IPoly poly;
        int index;

        public IPolyEdge(IPoly poly, int index)
        {
            this.poly = poly;
            this.index = index;
        }

        public Vector3 A
        {
            get
            {
                return poly.GetPoint(index);
            }
        }

        public Vector3 B
        {
            get
            {
                return poly.GetPoint((index + 1) % poly.Resolution);
            }
        }

        public Vector3 Normal
        {
            get
            {
                return poly.GetNormal();
            }
        }
        public Vector3 SurfaceNormal
        {
            get
            {
                return poly.GetSurfaceNormal(index);
            }
        }

        public IEdge Clone()
        {
            return new EdgeWithNormal(A, B, Normal);
        }
        public IEdge Clone(Vector3 a, Vector3 b)
        {
            return new EdgeWithNormal(a, b, Normal);
        }

        public void Draw(float time = -1f)
        {
            Draw(Color.white, time);
        }

        public void Draw(Color color, float time = -1f)
        {
            Debug.DrawLine(A, B, color, time);
        }

        public override string ToString()
        {
            return "{" + A.ToString() + ", " + B.ToString() + "}";
        }
    }
}