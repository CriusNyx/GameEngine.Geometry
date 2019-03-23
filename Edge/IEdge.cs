using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    public interface IEdge : IDrawable
    {
        Vector3 A { get; }
        Vector3 B { get; }
        IEdge Clone();
        IEdge Clone(Vector3 a, Vector3 b);
        Vector3 Normal { get; }
        Vector3 SurfaceNormal { get; }
    }

    static class IEdgeExtensions
    {
        public static Vector3 Direction(this IEdge edge)
        {
            return edge.B - edge.A;
        }

        public static float Length(this IEdge edge)
        {
            return edge.Direction().magnitude;
        }

        public static Vector3 Lerp(this IEdge edge, float t)
        {
            return Vector3.Lerp(edge.A, edge.B, t);
        }

        public static Vector3 Center(this IEdge edge)
        {
            return edge.Lerp(0.5f);
        }

        public static bool CompareLine(this IEdge edge, IEdge other)
        {
            Vector3 a = edge.Direction().normalized;
            Vector3 b = other.Direction().normalized;
            float dot = Vector3.Dot(a, b);
            if(Mathf.Abs(dot) > 0.99f)
            {
                Vector3 offset = edge.A - Math3d.LinePlaneIntersection(other.A, b, edge.A, a);
                if (offset.sqrMagnitude < 0.0001f * 0.0001f)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public static IEdge GetIntersecting(this IEdge a, IEdge b)
        {
            float aa = 0f;
            Vector3 direction = a.Direction().normalized;
            float ab = Vector3.Dot(a.B - a.A, direction);
            float ba = Vector3.Dot(direction, b.A - a.A);
            float bb = Vector3.Dot(direction, b.B - a.A);
            if (ba > ab && bb > ab) return null;
            if (ba < 0 && bb < 0) return null;
            float[] keys = new float[] { aa, ab, ba, bb };
            System.Array.Sort(keys);
            return new Edge(a.A + direction * keys[1], a.A + direction * keys[2]);
        }

        public static IEdge[] RemoveIntersecting(this IEdge a, IEdge b)
        {
            IEdge[] output = new IEdge[2];
            Vector3 direction = a.Direction();
            Vector3[] points = new Vector3[] { a.A, a.B, b.A, b.B };
            float[] keys = new float[] { 0f, Vector3.Dot(a.A - a.B, direction), Vector3.Dot(a.A - b.A, direction), Vector3.Dot(a.A - b.B, direction) };
            System.Array.Sort(keys, points);
            if((points[1] - points[0]).sqrMagnitude > 0.0001f * 0.0001f)
            {
                output[0] = a.Clone(points[0], points[1]);
            }
            if ((points[3] - points[2]).sqrMagnitude > 0.0001f * 0.0001f)
            {
                output[1] = a.Clone(points[3], points[2]);
            }
            return output;
        }

        public static bool ContainsPoint(this IEdge e, Vector3 point)
        {
            float l = (point - e.A).magnitude;
            float r = (point - e.B).magnitude;
            return (Mathf.Abs(l + r - e.Length()) <= 0.0001f);
        }

        public static bool ContainsEdge(this IEdge a, IEdge b)
        {
            return a.ContainsPoint(b.A) && a.ContainsPoint(b.B);
        }
    }
}