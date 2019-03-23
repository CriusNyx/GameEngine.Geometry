using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Geometry.Advanced;
using System;

namespace GameEngine.Geometry
{
    /// <summary>
    /// A utility class with extension methods for IPoly
    /// </summary>
    public static class IPolyExtensions
    {
        /// <summary>
        /// Get the area of the polygon
        /// May be optimized for PolyAdv
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static float GetArea(this IPoly poly)
        {
            if (poly is IPolyExtensionArea)
            {
                return ((IPolyExtensionArea)poly).AdvGetArea();
            }
            else if (poly is IPolyForwarder)
            {
                return ((IPolyForwarder)poly).GetPoly().GetArea();
            }
            else
            {
                return poly.CalcArea();
            }
        }

        /// <summary>
        /// Get the perimeter of the polygon.
        /// May be oprimized for PolyAdv
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static float GetPerimeter(this IPoly poly)
        {
            if (poly is IPolyExtensionPerimeter)
            {
                return ((IPolyExtensionPerimeter)poly).AdvGetPerimeter();
            }
            else if (poly is IPolyForwarder)
            {
                return ((IPolyForwarder)poly).GetPoly().GetPerimeter();
            }
            else
            {
                return poly.CalcPerimeter();
            }
        }

        /// <summary>
        /// Get the normal for the polygon.
        /// May be optimized for PolyAdv.
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static Vector3 GetNormal(this IPoly poly)
        {
            if (poly is IPolyExtensionNormal)
            {
                return ((IPolyExtensionNormal)poly).AdvGetNormal();
            }
            else if (poly is IPolyForwarder)
            {
                return ((IPolyForwarder)poly).GetPoly().GetNormal();
            }
            else
            {
                return poly.CalcNormal();
            }
        }

        /// <summary>
        /// Returns a vector which is on the same plane as the polygon, and perpendiculat to the nth edge
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector3 GetSurfaceNormal(this IPoly poly, int index)
        {
            if (poly is IPolyExtensionSurfaceNormal)
            {
                return ((IPolyExtensionSurfaceNormal)poly).AdvGetSurfaceNormal(index);
            }
            else if (poly is IPolyForwarder)
            {
                return ((IPolyForwarder)poly).GetPoly().GetSurfaceNormal(index);
            }
            else
            {
                return poly.CalcSurfaceNormal(index);
            }
        }

        public static Vector3 Center(this IPoly poly)
        {
            return Average(poly);
        }

        /// <summary>
        /// Returns the average of all the points in the polygon.
        /// This is a useful approximation for the center of the polygon.
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static Vector3 Average(this IPoly poly)
        {
            Vector3 point = Vector3.zero;
            int l = poly.Resolution;
            for (int i = 0; i < l; i++)
            {
                point += poly.GetPoint(i);
            }
            return (1f / l) * point;
        }

        public static IEdge GetEdge(this IPoly poly, int index)
        {
            return new IPolyEdge(poly, index);
        }

        /// <summary>
        /// Expands a polygon by the specified distance.
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="distance"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static IPoly Expand(this IPoly poly, float distance, Func<IEnumerable<Vector3>, IPoly> constructor = null)
        {
            if (constructor == null)
            {
                constructor = poly.Clone;
            }
            Vector3[] points = new Vector3[poly.Resolution];
            Vector3[] normals = new Vector3[poly.Resolution];
            Vector3[] direction = new Vector3[poly.Resolution];
            for (int i = 0; i < poly.Resolution; i++)
            {
                points[i] = poly.GetPoint(i);
                normals[i] = poly.GetSurfaceNormal(i).normalized;
                direction[i] = poly.GetPoint((i + 1) % poly.Resolution) - poly.GetPoint(i);
            }

            Vector3[] outputPoints = new Vector3[poly.Resolution];
            for (int i = 0; i < poly.Resolution; i++)
            {
                Vector3 p0 = points[i] + normals[i] * distance;
                int iMinus = (i - 1 + poly.Resolution) % poly.Resolution;
                Vector3 l0 = points[iMinus] + normals[iMinus] * distance;
                outputPoints[i] = Math3d.LinePlaneIntersection(l0, direction[iMinus], p0, normals[i]);
            }

            return constructor(outputPoints);
        }

        public static IPoly Contract(this IPoly poly, float tValue)
        {
            return poly.Contract(poly.Average(), tValue);
        }

        /// <summary>
        /// Contract the polygon toward a specified point.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="center"></param>
        /// <param name="lockList"></param>
        /// <returns></returns>
        public static IPoly Contract(this IPoly poly, Vector3 center, float tValue)
        {
            Vector3[] output = new Vector3[poly.Resolution];
            for(int i = 0; i < poly.Resolution; i++)
            {
                output[i] = Vector3.Lerp(poly.GetPoint(i), center, tValue);
            }
            return poly.Clone(output);
        }

        public static IPoly Contract(this IPoly poly, float tValues, bool[] locks)
        {
            return poly.Contract(poly.Average(), tValues, locks);
        }

        /// <summary>
        /// Contract the polygon toward a specified point.
        /// locks controlls which edges are not able to move to complete the operation
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="center"></param>
        /// <param name="tValue"></param>
        /// <param name="lockList"></param>
        /// <returns></returns>
        public static IPoly Contract(this IPoly poly, Vector3 center, float tValue, bool[] locks)
        {
            if (locks == null)
                return Contract(poly, center, tValue);
            else
            {
                Vector3[] output = new Vector3[poly.Resolution];
                for (int i = 0; i < poly.Resolution; i++)
                {
                    Vector3 point = poly.GetPoint(i);
                    bool rightLock = locks[i];
                    int minusIndex = (i + poly.Resolution - 1) % poly.Resolution;
                    bool leftLock = locks[minusIndex];
                    if (!rightLock && !leftLock)
                    {
                        output[i] = Vector3.Lerp(point, center, tValue);
                    }
                    else if (leftLock && rightLock)
                    {
                        output[i] = point;
                    }
                    else if (leftLock && !rightLock)
                    {
                        output[i] = Vector3.Lerp(point, poly.GetEdge(minusIndex).Center(), tValue);
                    }
                    else if (!leftLock && rightLock)
                    {
                        output[i] = Vector3.Lerp(point, poly.GetEdge(i).Center(), tValue);
                    }
                }
                return poly.Clone(output);
            }
        }

        public static IEnumerable<IEdge> Edges(this IPoly poly)
        {
            for (int i = 0; i < poly.Resolution; i++)
            {
                yield return poly.GetEdge(i);
            }
        }

        public static bool PointInPoly(this IPoly poly, Vector3 point)
        {
            return poly.PointInPoly(point, poly.GetNormal());
        }

        public static bool PointInPoly(this IPoly poly, Vector3 point, Vector3 projection)
        {
            for(int i = 1; i < poly.Resolution - 1; i++)
            {
                Vector3 a = poly.GetPoint(i) - poly.GetPoint(0);
                Vector3 b = poly.GetPoint(i + 1) - poly.GetPoint(0);
                Vector3 c = projection;
                Vector3 d = point - poly.GetPoint(0);

                var m11 = b.y * c.z - b.z * c.y;
                var m21 = b.x * c.z - b.z * c.x;
                var m31 = b.x * c.y - b.y * c.x;

                var m12 = a.y * c.z - a.z * c.y;
                var m22 = a.x * c.z - a.z * c.x;
                var m32 = a.x * c.y - a.y * c.x;

                float detT = a.x * m11 - a.y * m21 + a.z * m31;
                float detTx = d.x * m11 - d.y * m21 + d.z * m31;
                float detTy = -d.x * m12 + d.y * m22 - d.z * m32;

                float x = detTx / detT;
                float y = detTy / detT;
                //Debug.Log(a + ":" + b + ":" + c + ":" + d + ":" + x + ":" + y + ":" + (x + y));
                if(x >= 0 && y >= 0 && x + y <= 1f) return true;
            }
            return false;
        }

        public static bool PointInPoly(this IPoly poly, Vector3 point, Vector3 projection, out float distance)
        {
            distance = 0f;
            for(int i = 1; i < poly.Resolution - 1; i++)
            {
                Vector3 a = poly.GetPoint(i) - poly.GetPoint(0);
                Vector3 b = poly.GetPoint(i + 1) - poly.GetPoint(0);
                Vector3 c = projection;
                Vector3 d = point - poly.GetPoint(0);

                var m11 = b.y * c.z - b.z * c.y;
                var m21 = b.x * c.z - b.z * c.x;
                var m31 = b.x * c.y - b.y * c.x;

                var m12 = a.y * c.z - a.z * c.y;
                var m22 = a.x * c.z - a.z * c.x;
                var m32 = a.x * c.y - a.y * c.x;

                var m13 = a.y * b.z - a.z * b.y;
                var m23 = a.x * b.z - a.z * b.x;
                var m33 = a.x * b.y - a.y * b.x;

                float detT = a.x * m11 - a.y * m21 + a.z * m31;
                float detTx = d.x * m11 - d.y * m21 + d.z * m31;
                float detTy = -d.x * m12 + d.y * m22 - d.z * m32;
                float detTz = d.x * m13 - d.y * m23 + d.z * m33;

                float x = detTx / detT;
                float y = detTy / detT;
                distance = detTz / detT;
                //Debug.Log(a + ":" + b + ":" + c + ":" + d + ":" + x + ":" + y + ":" + (x + y));
                if(x >= 0 && y >= 0 && x + y <= 1f) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the radius of a circle with the same area.
        /// Useful for approximating the size of a polygon.
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static float GetRadius(this IPoly poly)
        {
            return Mathf.Sqrt(poly.CalcArea() / Mathf.PI);
        }

        /// <summary>
        /// Get a point, accounting for edges wrapping around the polygon
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector3 GetPointWrapped(this IPoly poly, int index)
        {
            if(index < 0)
            {
                return poly.GetPoint(-(index % poly.Resolution));
            }
            else
            {
                return poly.GetPoint(index % poly.Resolution);
            }
        }
    }
}

namespace GameEngine.Geometry.Advanced
{
    public static class IPolyExtensionsAdvanced
    {
        /// <summary>
        /// Calculate the perimeter of the polygon
        /// Always calculates the perimeter, so may be slow.
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static float CalcPerimeter(this IPoly poly)
        {
            float perim = 0f;
            int resolution = poly.Resolution;
            for (int i = 0; i < resolution; i++)
            {
                int j = (i + 1) % resolution;
                perim += (poly.GetPoint(i) - poly.GetPoint(j)).magnitude;
            }
            return perim;
        }

        /// <summary>
        /// Calculate the normal for the polygon
        /// Always calculates the normal, so may be slow.
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static Vector3 CalcNormal(this IPoly poly)
        {
            if (poly.Resolution < 3)
                throw new System.InvalidOperationException("Polygon must have at least 3 points");
            return Vector3.Cross(poly.GetPoint(1) - poly.GetPoint(0), poly.GetPoint(2) - poly.GetPoint(0)).normalized;
        }

        /// <summary>
        /// Get the area of the polygon.
        /// Always calculates the area, so may be expensive.
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static float CalcArea(this IPoly poly)
        {
            float area = 0f;
            int resolution = poly.Resolution;
            Vector3 p0 = poly.GetPoint(0);
            for (int i = 1; i < resolution - 1; i++)
            {
                Vector3 p1 = poly.GetPoint(i);
                Vector3 p2 = poly.GetPoint(i + 1);
                area += Vector3.Cross(p1 - p0, p2 - p0).magnitude / 2f;
            }
            return area;
        }

        /// <summary>
        /// Calculates the surface normal normal of the polygon using the cross product
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector3 CalcSurfaceNormal(this IPoly poly, int index)
        {
            return -Vector3.Cross(poly.GetNormal(), poly.GetPoint((index + 1) % poly.Resolution) - poly.GetPoint(index));
        }

        /// <summary>
        /// A default method for drawing the polygon
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="color"></param>
        /// <param name="time"></param>
        public static void DrawPoly(this IPoly poly, Color color, float time)
        {
            int r = poly.Resolution;
            for (int i = 0; i < r; i++)
            {
                int j = (i + 1) % r;
                Debug.DrawLine(poly.GetPoint(i), poly.GetPoint(j), color, time);
            }
        }
    }
}