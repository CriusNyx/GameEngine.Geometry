using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    public interface IPath : IEnumerable<Vector3>, IDrawable
    {
        int Resolution { get; }
        Vector3 GetPathPoint(int index);
    }

    public static class IPathExtensions
    {
        public static void GenericDraw(this IPath path, Color color, float time)
        {
            for (int i = 0; i < path.Resolution - 1; i++)
            {
                Debug.DrawLine(path.GetPathPoint(i), path.GetPathPoint(i + 1), color, time);
            }
        }

        /// <summary>
        /// Returns the point along the path at the specified distance.
        /// Returns the last point in the path if distance runs over the path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 GetPointAtDistance(this IPath path, float distance)
        {
            Vector3 nextPoint = path.GetPathPoint(0);
            for(int i = 0; i < path.Resolution - 1; i++)
            {
                Vector3 point = nextPoint;
                nextPoint = path.GetPathPoint(i + 1);
                Vector3 v = nextPoint - point;
                Vector3 n = v.normalized;
                float dot = Vector3.Dot(n, v);
                if(dot > distance)
                {
                    return point + n * distance;
                }
                distance -= dot;
            }
            return nextPoint;
        }

        /// <summary>
        /// Returns the point along the path at the specified distance.
        /// Returns the last point in the path if distance runs over the path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 GetPointAtDistance(this IPath path, float distance, out Vector3 output)
        {
            Vector3 nextPoint = path.GetPathPoint(0); 
            for(int i = 0; i < path.Resolution - 1; i++)
            {
                Vector3 point = nextPoint;
                nextPoint = path.GetPathPoint(i + 1);
                Vector3 v = nextPoint - point;
                Vector3 n = v.normalized;
                float dot = Vector3.Dot(n, v);
                if(dot > distance)
                {
                    output = n;
                    return point + n * distance;
                }
                distance -= dot;
            }
            output = Vector3.zero;
            return nextPoint;
        }
    }
}