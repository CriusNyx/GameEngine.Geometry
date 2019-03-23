using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Geometry;
using System;
using System.Linq;
using GameEngine.CSG2D;
using GameEngine.CSG3D;

namespace GameEngine.CSG.Geometry
{
    public static class CSGGeometry
    {
        #region Divide 2D
        /// <summary>
        /// Divide a polygon using the specified plane.
        /// </summary>
        /// <param name="poly">
        /// The polygon to divide.
        /// </param>
        /// <param name="p0">
        /// Plane Origin.
        /// </param>
        /// <param name="pn">
        /// Plane Normal.
        /// </param>
        /// <param name="threshold">
        /// The percision threshold for comparing points to the plane.
        /// </param>
        /// <returns>
        /// outputs new object[]{IPoly insidePolygon, IPoly outsidePolygon, Vector3 newlyCreatedPoint};
        /// newly created point is useful for face concstruction for Block Divison, and texture maping.
        /// </returns>
        public static object[] Divide(IPoly poly, Vector3 p0, Vector3 pn, float threshold = 0.001f)
        {
            return Divide(poly, p0, pn, (x, y) => x.Clone(y), threshold);
        }

        /// <summary>
        /// Divide a polygon using the specified plane.
        /// </summary>
        /// <param name="poly">
        /// The polygon to divide.
        /// </param>
        /// <param name="p0">
        /// Plane Origin.
        /// </param>
        /// <param name="pn">
        /// Plane Normal.
        /// </param>
        /// <param name="constructor">
        /// The Polygon constructor can be specified for more flexability.
        /// Defaults to new NGon.
        /// </param>
        /// <param name="threshold">
        /// The percision threshold for comparing points to the plane.
        /// </param>
        /// <returns>
        /// outputs new object[]{IPoly insidePolygon, IPoly outsidePolygon, Vector3 newlyCreatedPoint};
        /// newly created point is useful for face concstruction for Block Divison, and texture maping.
        /// </returns>
        public static object[] Divide(
            IPoly poly,
            Vector3 p0,
            Vector3 pn,
            Func<IPoly, IEnumerable<Vector3>, IPoly> constructor,
            float threshold = 0.001f)
        {
            //The following algorithm is used to divide faces
            //Create a list for the inside, and outside points
            //for each point, append it to the appropriate list
            //if the point intersects the boundry, append it to the itnersection list
            //construct new faces from the list
            //the intersection point exiting the plane is returned, for use by other algorithms

            //initialization
            List<Vector3> insidePoints = new List<Vector3>();
            List<Vector3> outsidePoints = new List<Vector3>();
            bool hasOutputPont = false;
            Vector3 outputPoint = default(Vector3);
            int l = poly.Resolution;
            int[] signs = new int[l];

            //itterate through poitns and calculate their signs
            for (int i = 0; i < l; i++)
            {
                float f = Math3d.SignedDistancePlanePoint(pn, p0, poly.GetPoint(i));
                if (f < -threshold) signs[i] = -1;
                else if (f > threshold) signs[i] = 1;
                else signs[i] = 0;
            }

            //itterate through points, and append them to the appropriate list
            for (int i = 0; i < l; i++)
            {
                Vector3 l0 = poly.GetPoint(i);
                //inside point
                if (signs[i] == -1)
                {
                    insidePoints.Add(l0);
                }
                //intersecting point
                else if (signs[i] == 0)
                {
                    insidePoints.Add(l0);
                    outsidePoints.Add(l0);
                    if (signs[(i + 1) % l] == 1)
                    {
                        outputPoint = l0;
                        hasOutputPont = true;
                    }
                }
                //outside point
                else
                {
                    outsidePoints.Add(l0);
                }

                //detect intersections
                if (signs[i] * signs[(i + 1) % l] == -1)
                {
                    Vector3 ld = poly.GetPoint((i + 1) % l) - l0;
                    Vector3 intersection = Math3d.LinePlaneIntersection(l0, ld, p0, pn);
                    insidePoints.Add(intersection);
                    outsidePoints.Add(intersection);
                    if (signs[i] == -1)
                    {
                        outputPoint = intersection;
                        hasOutputPont = true;
                    }
                }
            }

            //construct output
            object point = null;
            if (hasOutputPont)
                point = outputPoint;
            return new object[] {
                insidePoints.Count >= 3 ? constructor(poly, insidePoints) : null,
                outsidePoints.Count >= 3 ? constructor(poly, outsidePoints) : null,
                point};
        }

        /// <summary>
        /// Divide a polygon using the specified plane.
        /// </summary>
        /// <param name="poly">
        /// The polygon to divide.
        /// </param>
        /// <param name="inside">
        /// The new polygon inside the plane.
        /// </param>
        /// <param name="outside">
        /// The new polygon outside the plane.
        /// </param>
        /// <param name="p0">
        /// Plane Origin.
        /// </param>
        /// <param name="pn">
        /// Plane Normal.
        /// </param>
        /// <param name="threshold">
        /// The percision threshold for comparing points to the plane.
        /// </param>
        public static void Divide(
            IPoly poly,
            out IPoly inside,
            out IPoly outside,
            Vector3 p0,
            Vector3 pn,
            float threshold = 0.001f)
        {
            Divide(poly, out inside, out outside, p0, pn, (x, y) => x.Clone(y), threshold);
        }

        /// <summary>
        /// Divide a polygon using the specified plane.
        /// </summary>
        /// <param name="poly">
        /// The polygon to divide.
        /// </param>
        /// <param name="inside">
        /// The new polygon inside the plane.
        /// </param>
        /// <param name="outside">
        /// The new polygon outside the plane.
        /// </param>
        /// <param name="p0">
        /// Plane Origin.
        /// </param>
        /// <param name="pn">
        /// <param name="constructor">
        /// The Polygon constructor can be specified for more flexability.
        /// Defaults to new NGon.
        /// </param>
        /// <param name="threshold">
        /// The percision threshold for comparing points to the plane.
        /// </param>
        public static void Divide(
            IPoly poly,
            out IPoly inside,
            out IPoly outside,
            Vector3 p0,
            Vector3 pn,
            Func<IPoly, IEnumerable<Vector3>, IPoly> constructor,
            float threshold = 0.001f)
        {
            object[] output = Divide(poly, p0, pn, constructor, threshold);
            inside = (IPoly)output[0];
            outside = (IPoly)output[1];
        }
        #endregion

        #region Divide 3D
        /// <summary>
        /// Divides a 3D block into two
        /// </summary>
        /// <param name="block">The block to divide</param>
        /// <param name="p0">The plane point for division</param>
        /// <param name="pn">The plane normal for division</param>
        /// <param name="newFaceConstructor">Constructor for the new face. Defaults to new NGon()</param>
        /// <param name="threshold">The floating point threshold for division, defaults to 0.001 or 1mm</param>
        /// <returns></returns>
        public static object[] Divide(IBlock block, Vector3 p0, Vector3 pn, Func<IEnumerable<Vector3>, IPoly> newFaceConstructor = null, float threshold = 0.001f)
        {
            if (newFaceConstructor == null)
                newFaceConstructor = x => new NGon(x);

            List<IPoly> insideFaces = new List<IPoly>();
            List<IPoly> outsideFaces = new List<IPoly>();
            List<Vector3> newPoints = new List<Vector3>();
            foreach (IPoly face in block.GetFaces())
            {
                object[] o = Divide(face, p0, pn, threshold);
                var inside = o[0];
                var outside = o[1];
                var newPoint = o[2];

                if (inside != null)
                    insideFaces.Add(inside as IPoly);
                if (outside != null)
                    outsideFaces.Add(outside as IPoly);
                if (newPoint != null)
                    newPoints.Add((Vector3)newPoint);
            }

            //construct new face
            if (newPoints.Count > 3)
            {
                Vector3 center = Vector3.zero;
                foreach (var point in newPoints)
                {
                    center += point;
                }
                center = center / (float)newPoints.Count;
                newPoints = Math3d.CircleSort(center, pn, newPoints.ToArray()).ToList();
            }

            outsideFaces.Add(newFaceConstructor(newPoints));
            newPoints.Reverse();
            insideFaces.Add(newFaceConstructor(newPoints));

            return new object[]{
                insideFaces.Count >= 4 ? block.Clone(insideFaces) : null,
                insideFaces.Count >= 4 ? block.Clone(outsideFaces) : null};
        }
        #endregion

        #region SetSubtract 2D
        /// <summary>
        /// Preforms the set subtract operation with multiple pos and a single neg.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <returns></returns>
        public static List<IPoly> SetSubtract(IEnumerable<IPoly> pos, IPoly neg)
        {
            return SetSubtract(pos, neg, (x, y) => x.Clone(y));
        }

        /// <summary>
        /// Preforms the set subtract operation with multiple pos and a single neg.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static List<IPoly> SetSubtract(IEnumerable<IPoly> pos, IPoly neg, Func<IPoly, IEnumerable<Vector3>, IPoly> constructor)
        {
            //set subtract algorithm
            //this version works for only 1 negative poly, and is the basis for multiple polys
            //append all positive polys to a working stack
            //pop polys from the working stack and compare them to the neg poly
            //if they intersect, split them, and push them on the working stack
            //if they are enclosed, remove them from the working list
            //if they aren't instersecting append them to the output stack
            //continue until working stack is empty

            //Initialization
            List<IPoly> outputList = new List<IPoly>();
            Stack<IPoly> workingStack = new Stack<IPoly>(pos);

            //working stack loop
            while (workingStack.Count > 0)
            {
                //initialization
                IPoly current = workingStack.Pop();
                int offendingIndex;

                //detect collision status
                var status = CSGPhysics.CalcCollision2D(neg, current, out offendingIndex);
                //enclosed, just throw away polygon
                if (status == CSGPhysics.CollisionType.BEnclosedInA)
                {
                    continue;
                }
                //if colliding, divide the polygon and push them on the working stack
                else if (status != CSGPhysics.CollisionType.NotColliding)
                {
                    IPoly inside, outside;
                    Divide(current, out inside, out outside, neg.GetPoint(offendingIndex), neg.GetSurfaceNormal(offendingIndex), constructor);
                    workingStack.Push(inside);
                    workingStack.Push(outside);
                }
                //if not colliding, append to output list
                else
                {
                    outputList.Add(current);
                }
            }
            return outputList;
        }

        /// <summary>
        /// Preforms the set subtract operation with multiple pos and multiple neg.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <returns></returns>
        public static List<IPoly> SetSubtract(IEnumerable<IPoly> pos, IEnumerable<IPoly> neg)
        {
            return SetSubtract(pos, neg, (x, y) => x.Clone(y));
        }

        /// <summary>
        /// Preforms the set subtract operation with multiple pos and multiple neg.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static List<IPoly> SetSubtract(IEnumerable<IPoly> pos, IEnumerable<IPoly> neg, Func<IPoly, IEnumerable<Vector3>, IPoly> constructor)
        {
            foreach (var n in neg)
            {
                pos = SetSubtract(pos, n, constructor);
            }
            return pos.ToList();
        }
        #endregion

        #region VectorSetSubtract 2D
        /// <summary>
        /// Modification of set subtract for vector space.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <param name="intersec"></param>
        /// <returns></returns>
        public static List<CSGShape> VectorSetSubtract(List<CSGShape> pos, List<CSGShape> neg, List<CSGShape> intersec = null)
        {
            foreach (var n in neg)
            {
                pos = VectorSetSubtract(pos, n, intersec);
            }
            return pos.ToList();
        }

        /// <summary>
        /// Modification of set subtract for vector space.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <param name="intersec"></param>
        /// <returns></returns>
        public static List<CSGShape> VectorSetSubtract(List<CSGShape> pos, CSGShape neg, List<CSGShape> intersec = null)
        {
            //Look to set subtract algorithm for explinations
            //The primary difference between this algorithm, and the set subtract algorithm, is that this algorithm passes on correct r values to the output

            List<CSGShape> outputList = new List<CSGShape>();
            Stack<CSGShape> workingStack = new Stack<CSGShape>(pos);
            while (workingStack.Count > 0)
            {
                CSGShape current = workingStack.Pop();
                int offendingIndex;
                var status = CSGPhysics.CalcCollision2D(neg, current, out offendingIndex);
                if (status == CSGPhysics.CollisionType.BEnclosedInA)
                {
                    if (intersec != null)
                    {
                        current.rValue += neg.rValue;
                        intersec.Add(current);
                    }
                }
                else if (status != CSGPhysics.CollisionType.NotColliding)
                {
                    IPoly inside, outside;
                    Divide(current, out inside, out outside, neg.GetPoint(offendingIndex), neg.GetSurfaceNormal(offendingIndex), (x, y) => x.Clone(y));
                    workingStack.Push(new CSGShape(inside, current.rValue));
                    workingStack.Push(new CSGShape(outside, current.rValue));
                }
                else
                {
                    outputList.Add(current);
                }
            }
            return outputList;
        }
        #endregion

        #region VectorSetSubtract 3D
        /// <summary>
        /// Modified version of set subtract for vector spaces.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <param name="intersec"></param>
        /// <param name="newFaceConstructor"></param>
        /// <returns></returns>
        public static List<CSGBlock> VectorSetSubtract(
            List<CSGBlock> pos, 
            List<CSGBlock> neg, 
            List<CSGBlock> intersec = null, 
            Func<IEnumerable<Vector3>, IPoly, IPoly> newFaceConstructor = null)
        {
            foreach (var n in neg)
            {
                pos = VectorSetSubtract(pos, n, intersec, newFaceConstructor);
            }
            return pos.ToList();
        }

        /// <summary>
        /// Modified version of set subtract for vector spaces
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="neg"></param>
        /// <param name="intersec"></param>
        /// <param name="newFaceConstructor"></param>
        /// <returns></returns>
        public static List<CSGBlock> VectorSetSubtract(
            List<CSGBlock> pos, 
            CSGBlock neg, 
            List<CSGBlock> intersec = null, 
            Func<IEnumerable<Vector3>, IPoly, IPoly> newFaceConstructor = null)
        {
            //Review Vector Set Subtract for 2D spaces, and ordinary set subtract to an explination of the algorithm
            if (newFaceConstructor == null)
                newFaceConstructor = (x, y) => y.Clone(x);
            List<CSGBlock> outputList = new List<CSGBlock>();
            Stack<CSGBlock> workingStack = new Stack<CSGBlock>(pos);
            while (workingStack.Count > 0)
            {
                CSGBlock current = workingStack.Pop();
                IPoly offendingFace;
                var status = CSGPhysics.CalcCollision3D(neg, current, out offendingFace);
                if (status == CSGPhysics.CollisionType.BEnclosedInA)
                {
                    if (intersec != null)
                    {
                        current.rValue += neg.rValue;
                        intersec.Add(current);
                    }
                }
                else if (status != CSGPhysics.CollisionType.NotColliding)
                {
                    var output = Divide(current, offendingFace.GetPoint(0), offendingFace.GetNormal(), x => newFaceConstructor(x, offendingFace));
                    if (output[0] != null)
                        workingStack.Push(new CSGBlock(output[0] as IBlock, current.rValue));
                    if (output[1] != null)
                        workingStack.Push(new CSGBlock(output[1] as IBlock, current.rValue));
                }
                else
                {
                    outputList.Add(current);
                }
            }
            return outputList;
        }
        #endregion
    }
}