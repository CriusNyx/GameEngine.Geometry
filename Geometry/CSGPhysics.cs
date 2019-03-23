using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.CSG2D;
using GameEngine.Geometry;

namespace GameEngine.CSG
{
    public static class CSGPhysics
    {
        public enum CollisionType
        {
            Colliding,
            NotColliding,
            AEnclosedInB,
            BEnclosedInA
        }

        /// <summary>
        /// Calculates the collision status of two polygons.
        /// Returns colliding is the polygons are intersecting, not colliding if they are not, AEnclosedInB is A is inside B, and BEnclosedInA for ...
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static CollisionType CalcCollision2D(IPoly a, IPoly b, float threshold = 0.001f)
        {
            int temp;
            return CalcCollision2D(a, b, out temp, threshold);
        }

        /// <summary>
        /// Calculates the collision status of two polygons.
        /// Returns colliding is the polygons are intersecting, not colliding if they are not, AEnclosedInB is A is inside B, and BEnclosedInA for ...
        /// This method also returns the offending index
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="offendingIndex"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static CollisionType CalcCollision2D(IPoly a, IPoly b, out int offendingIndex, float threshold = 0.001f)
        {
            offendingIndex = -1;

            //check for a
            bool enclosed = true;
            for (int i = 0; i < a.Resolution; i++)
            {
                Vector3 point = a.GetPoint(i);
                Vector3 surfaceNormal = a.GetSurfaceNormal(i);
                bool inside, outside;
                CheckPoly(b, point, surfaceNormal, out inside, out outside, threshold);
                if (outside)
                {
                    enclosed = false;
                }
                if (!inside)
                    return CollisionType.NotColliding;
                if (inside && outside)
                    offendingIndex = i;
            }
            if (enclosed)
                return CollisionType.BEnclosedInA;

            //check for b
            enclosed = true;
            for (int i = 0; i < b.Resolution; i++)
            {
                Vector3 point = b.GetPoint(i);
                Vector3 surfaceNormal = b.GetSurfaceNormal(i);
                bool inside, outside;
                CheckPoly(a, point, surfaceNormal, out inside, out outside, threshold);
                if (outside)
                    enclosed = false;
                if (!inside)
                    return CollisionType.NotColliding;
            }
            if (enclosed)
                return CollisionType.AEnclosedInB;

            return CollisionType.Colliding;
        }

        /// <summary>
        /// Calculates the collision status of two blocks.
        /// Returns colliding is the polygons are intersecting, not colliding if they are not, AEnclosedInB is A is inside B, and BEnclosedInA for ...
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static CollisionType CalcCollision3D(IBlock a, IBlock b, float threshold = 0.001f)
        {
            IPoly temp;
            return CalcCollision3D(a, b, out temp, threshold);
        }

        /// <summary>
        /// Calculates the collision status of two blocks.
        /// Returns colliding is the polygons are intersecting, not colliding if they are not, AEnclosedInB is A is inside B, and BEnclosedInA for ...
        /// The offending face is also returned, for the sake of other algorithms.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="offendingFace"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static CollisionType CalcCollision3D(IBlock a, IBlock b, out IPoly offendingFace, float threshold = 0.001f)
        {
            offendingFace = null;

            //check for a
            bool enclosed = true;
            foreach(var poly in a.GetFaces())
            {
                Vector3 point = poly.GetPoint(0);
                Vector3 surfaceNormal = poly.GetNormal();
                bool inside, outside;
                CheckBlock(b, point, surfaceNormal, out inside, out outside, threshold);
                if (outside)
                {
                    enclosed = false;
                }
                if (!inside)
                    return CollisionType.NotColliding;
                if (inside && outside)
                    offendingFace = poly;
            }
            if (enclosed)
                return CollisionType.BEnclosedInA;

            //check for b
            enclosed = true;
            foreach(var poly in b.GetFaces())
            {
                Vector3 point = poly.GetPoint(0);
                Vector3 surfaceNormal = poly.GetNormal();
                bool inside, outside;
                CheckBlock(a, point, surfaceNormal, out inside, out outside, threshold);
                if (outside)
                    enclosed = false;
                if (!inside)
                    return CollisionType.NotColliding;
            }
            if (enclosed)
                return CollisionType.AEnclosedInB;

            return CollisionType.Colliding;
        }

        /// <summary>
        /// Checks the collision status of a polygon.
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="p0"></param>
        /// <param name="pn"></param>
        /// <param name="inside"></param>
        /// <param name="outside"></param>
        /// <param name="threshold"></param>
        public static void CheckPoly(IPoly poly, Vector3 p0, Vector3 pn, out bool inside, out bool outside, float threshold = 0.001f)
        {
            inside = false;
            outside = false;
            for (int i = 0; i < poly.Resolution; i++)
            {
                Vector3 point = poly.GetPoint(i);
                float dis = Math3d.SignedDistancePlanePoint(pn, p0, point);
                if (dis < -threshold)
                    inside = true;
                if (dis > threshold)
                    outside = true;
                if (inside && outside)
                    return;
            }
        }

        /// <summary>
        /// Checks the collision status of a 3D block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="p0"></param>
        /// <param name="pn"></param>
        /// <param name="inside"></param>
        /// <param name="outside"></param>
        /// <param name="threshold"></param>
        public static void CheckBlock(IBlock block, Vector3 p0, Vector3 pn, out bool inside, out bool outside, float threshold = 0.001f)
        {
            inside = false;
            outside = false;
            foreach(var face in block.GetFaces())
            {
                bool tempin, tempout;
                CheckPoly(face, p0, pn, out tempin, out tempout, threshold);
                //compare poly against block
                if (tempin)
                    inside = true;
                if (tempout)
                    outside = true;
                //short circuit
                if (inside && outside)
                    return;
            }
        }
    }
}