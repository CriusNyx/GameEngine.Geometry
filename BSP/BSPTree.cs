using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameEngine.Geometry
{
    public class BSPTree<T> where T : IPoly
    {
        public readonly BSPNode<T> root;

        /// <summary>
        /// Get a bsp tree for the polygon
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="optimize"></param>
        public BSPTree(T[] polygons, bool optimize = false) : this(polygons.Select(x => new BSPPoly<T>(x)).ToArray(), optimize)
        {

        }

        /// <summary>
        /// Get a bsp tree for the polygons
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="optimize"></param>
        private BSPTree(BSPPoly<T>[] polygons, bool optimize = false)
        {
            this.root = GetNode(polygons, optimize);
        }

        /// <summary>
        /// Get the next bsp node for the polygons
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        private BSPNode<T> GetNode(BSPPoly<T>[] polygons, bool optimize)
        {
            if(polygons.Length == 0)
            {
                return null;
            }
            if(polygons.Length == 1)
            {
                return new BSPNode<T>(polygons[0], null, null, Vector3.zero, Vector3.zero);
            }

            if(optimize)
            {
                polygons = FindOptimalPoly(polygons);

                Vector3 point, normal;
                List<BSPPoly<T>> left, right;

                (point, normal, left, right) = FindOptimalSplit(polygons);

                return new BSPNode<T>(polygons[0], GetNode(left.ToArray(), optimize), GetNode(right.ToArray(), optimize), point, normal);
            }

            else
            {
                IPoly working = polygons[0].working;
                Vector3 point = working.GetPoint(0),
                    normal = working.GetSurfaceNormal(0);

                (List<BSPPoly<T>> left, List<BSPPoly<T>> right) = GetSplit(polygons, point, normal);

                return new BSPNode<T>(polygons[0], GetNode(left.ToArray(), optimize), GetNode(right.ToArray(), optimize), point, normal);
            }
        }

        /// <summary>
        /// Finds the optimal polygon, and moves it to the front of the array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private BSPPoly<T>[] FindOptimalPoly(BSPPoly<T>[] array)
        {
            if(array.Length == 0 || array.Length == 1)
            {
                return array;
            }

            //calculate bounds
            Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity), max = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);
            foreach(var poly in array)
            {
                foreach(var point in poly.working.GetPoints())
                {
                    for(int i = 0; i < 3; i++)
                    {
                        if(point[i] < min[i])
                        {
                            min[i] = point[i];
                        }
                        if(point[i] > max[i])
                        {
                            max[i] = point[i];
                        }
                    }
                }
            }

            //calculate mid
            Vector3 mid = (min + max) / 2f;

            //find optimal index
            int opt = 0;
            float optValue = Vector3.Distance(array[0].working.Center(), mid);
            for(int i = 1; i < array.Length; i++)
            {
                float val = Vector3.Distance(array[i].working.Center(), mid);
                if(val < optValue)
                {
                    opt = i;
                    optValue = val;
                }
            }

            var temp = array[0];
            array[0] = array[opt];
            array[opt] = temp;

            return array;
        }

        /// <summary>
        /// Find the optimal split for a polygon
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        private (Vector3 point, Vector3 normal, List<BSPPoly<T>> left, List<BSPPoly<T>> right) FindOptimalSplit(BSPPoly<T>[] poly)
        {
            //init
            Vector3 point = Vector3.zero, normal = Vector3.zero;
            List<BSPPoly<T>> left = null, right = null;

            //optimal diff
            int diff = int.MaxValue;

            //get the working polygon
            IPoly working = poly[0].working;
            for(int i = 0; i < working.Resolution; i++)
            {
                //get the current point and normal
                Vector3 localPoint = working.GetPoint(i),
                    localNormal = working.GetSurfaceNormal(i);

                //get output
                (List<BSPPoly<T>> localInside, List<BSPPoly<T>> localOutput) = GetSplit(poly, localPoint, localNormal);

                //calc the diff, and compare
                int localDiff = Math.Abs(localInside.Count - localOutput.Count);
                if(localDiff < diff)
                {
                    left = localInside;
                    right = localOutput;
                    point = localPoint;
                    normal = localNormal;
                    diff = localDiff;
                }
            }

            return (point, normal, left, right);
        }

        /// <summary>
        /// Get the split for polys[0], for the specified point and normal
        /// </summary>
        /// <param name="polys"></param>
        /// <param name="point"></param>
        /// <param name="normal"></param>
        /// <param name="startPoly"></param>
        /// <returns></returns>
        private (List<BSPPoly<T>> left, List<BSPPoly<T>> right) GetSplit(BSPPoly<T>[] polys, Vector3 point, Vector3 normal, int startPoly = 1)
        {
            List<BSPPoly<T>> inside = new List<BSPPoly<T>>(), outside = new List<BSPPoly<T>>();
            for(int i = startPoly; i < polys.Length; i++)
            {
                BSPPoly<T> poly = polys[i];
                IPoly working = poly.working;

                object[] split = CSG.Geometry.CSGGeometry.Divide(working, point, normal);
                if(split[0] != null)
                {
                    inside.Add(new BSPPoly<T>(poly.original, (IPoly)split[0]));
                }
                if(split[1] != null)
                {
                    outside.Add(new BSPPoly<T>(poly.original, (IPoly)split[1]));
                }
            }
            return (inside, outside);
        }

        public T GetPoly(Vector3 point)
        {
            return GetPoly(root, point);
        }

        private T GetPoly(BSPNode<T> node, Vector3 point)
        {
            if(node == null)
            {
                return default(T);
            }

            if(node.poly.working.PointInPoly(point))
            {
                return node.poly.original;
            }


            float value = Math3d.SignedDistancePlanePoint(node.planeNormal, node.planePoint, point);

            if(value < 0f)
            {
                return GetPoly(node.inside, point);
            }
            else
            {
                return GetPoly(node.outside, point);
            }
        }
    }

    public class BSPNode<T> where T : IPoly
    {
        public readonly BSPPoly<T> poly;
        public readonly BSPNode<T> inside, outside;
        public readonly Vector3 planePoint, planeNormal;

        public BSPNode(BSPPoly<T> poly, BSPNode<T> inside, BSPNode<T> outside, Vector3 planePoint, Vector3 planeNormal)
        {
            this.poly = poly;
            this.inside = inside;
            this.outside = outside;
            this.planePoint = planePoint;
            this.planeNormal = planeNormal;
        }
    }

    public class BSPPoly<T> where T : IPoly
    {
        public readonly T original;
        public readonly NGon working;
        

        public BSPPoly(T original, IPoly working)
        {
            this.original = original;
            if(working is NGon)
            {
                this.working = (NGon)working;
            }
            else
            {
                this.working = new NGon(working);
            }
        }

        public BSPPoly(T poly) : this(poly, poly)
        {

        }
    }
}
