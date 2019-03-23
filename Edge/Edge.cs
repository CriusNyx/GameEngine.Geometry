using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry {
    public class Edge : IEdge
    {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }

        public Vector3 Normal
        {
            get
            {
                return Vector3.zero;
            }
        }
        public Vector3 SurfaceNormal
        {
            get
            {
                return Vector3.zero;
            }
        }

        public Edge(Vector3 a, Vector3 b)
        {
            this.A = a;
            this.B = b;
        }

        public virtual IEdge Clone()
        {
            return new Edge(A, B);
        }
        public virtual IEdge Clone(Vector3 a, Vector3 b)
        {
            return new Edge(a, b);
        }

        public void Draw(float time = -1f)
        {
            Draw(Color.white, -1f);
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

    public class EdgeWithNormal : IEdge
    {
        Vector3 a, b, normal;

        public Vector3 A { get { return a; } }
        public Vector3 B { get { return b; } }
        public Vector3 Normal { get { return normal; } }
        public Vector3 SurfaceNormal { get { return Vector3.Cross(this.Direction(), normal); } }

        public EdgeWithNormal(Vector3 a, Vector3 b, Vector3 normal)
        {
            this.a = a;
            this.b = b;
            this.normal = normal;
        }

        public void Draw(float time = -1f)
        {
            Draw(Color.white, time);
        }

        public void Draw(Color color, float time = -1f)
        {
            Debug.DrawLine(A, B, color, time);
            Debug.DrawRay(A, Normal, Color.red, time);
        }

        public IEdge Clone()
        {
            return new EdgeWithNormal(a, b, normal);
        }
        
        public IEdge Clone(Vector3 a, Vector3 b)
        {
            return new EdgeWithNormal(a, b, normal);
        }
    }
}