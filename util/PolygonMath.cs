using GameEngine.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.Geometry
{
    public static class PolygonMath
    {
        public const float TAU = 6.283185307179586476925286766559005768394338798750211641949f;

        public static IEdge GetIntersection(IPoly a, IPoly b)
        {
            return new GetIntersectionIEdge(a, b);
        }

        public static bool HasIntersection(IPoly a, IPoly b)
        {
            //This logic is just wrong
            //Needs to be recoded
            throw new NotImplementedException();
            for(int i = 0; i < a.Resolution; i++)
            {
                IEdge ea = a.GetEdge(i);
                for(int j = 0; j < b.Resolution; j++)
                {

                    IEdge eb = b.GetEdge(j);
                    if(ea.CompareLine(eb) && Vector3.Dot(ea.Direction(), eb.Direction()) < 0)
                    {
                        return ea.GetIntersecting(eb) != null;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Create a polygon from a set of edges
        /// For this algorithm to work as expected, the edges must wrap to form a polygon
        /// Optimize output will merge consecutive edges with have the same line
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="constructor"></param>
        /// <param name="optimizeOutput"></param>
        /// <returns></returns>
        public static IPoly GetPolygonFromEdges(IEnumerable<IEdge> edges, Func<IEnumerable<IEdge>, IPoly> constructor = null, bool optimizeOutput = true)
        {
            if(constructor == null)
            {
                constructor = (x) => new EdgePoly(x);
            }

            //create a hash set of all edges
            HashSet<IEdge> hashSet = new HashSet<IEdge>(edges);

            IEdge current = hashSet.First();
            DoubleLinkNode<IEdge> currentNode = new DoubleLinkNode<IEdge>(current);
            DoubleLinkNode<IEdge> firstNode = currentNode;
            hashSet.Remove(current);
            while(hashSet.Count > 0)
            {
                //find next node
                IEdge next = hashSet.FirstOrDefault(x => Math3d.Compare(current.B, x.A));
                //error check
                if(next == null)
                {
                    return null;
                }

                //remove the node from the hash set
                hashSet.Remove(next);

                //create the next link node
                currentNode.next = new DoubleLinkNode<IEdge>(next);
                currentNode.next.previous = currentNode;

                //advance the search space
                current = next;
                currentNode = currentNode.next;
            }

            //link ends
            currentNode.next = firstNode;
            firstNode.previous = currentNode;

            if(optimizeOutput)
            {
                //rotate list to find good canidate for first node
                for(currentNode = firstNode.next; currentNode != firstNode; currentNode = currentNode.next)
                {
                    if(!IsParallel(currentNode.previous.value, currentNode.value))
                    {
                        break;
                    }
                }
                firstNode = currentNode;

                //break link to prevent infinate loops over bad geometry
                firstNode.previous.next = null;
                firstNode.previous = null;

                var last = firstNode;

                //eliminate extra edges
                for(var node = firstNode; node != null; node = node.next)
                {
                    if(node != null)
                    {
                        last = node;
                    }
                    while(node.next != null && IsParallel(node.value, node.next.value))
                    {
                        node.value = new Edge(node.value.A, node.next.value.B);
                        node.next = node.next.next;
                    }
                }

                //relink ends
                last.next = firstNode;
                firstNode.previous = last;
            }

            //construct output to ngon
            List<IEdge> outputList = new List<IEdge>();
            outputList.Add(firstNode.value);
            for(var node = firstNode.next; node != firstNode; node = node.next)
            {
                outputList.Add(node.value);
            }

            return constructor(outputList);
        }

        public static IPoly InjectEdge(IPoly poly, IEdge newEdge)
        {
            if(poly is EdgePoly)
            {
                return InjectEdge(poly as EdgePoly, newEdge);
            }
            throw new System.NotImplementedException();
        }

        private static IPoly InjectEdge(EdgePoly poly, IEdge newEdge)
        {
            //get indes of new edge
            int index = 0;
            for(; index < poly.Resolution; index++)
            {
                var edge = poly.GetEdge(index);
                if(IsParallel(newEdge, edge))
                {
                    if(edge.ContainsEdge(newEdge))
                    {
                        break;
                    }
                }
            }

            if(index == poly.Resolution)
            {
                return poly;
            }

            List<IEdge> output = new List<IEdge>();

            for(int i = 0; i < poly.Resolution; i++)
            {
                if(i == index)
                {
                    IEdge orig = poly.GetEdge(i);
                    IEdge left, right;

                    left = orig.Clone(orig.A, newEdge.A);
                    if(left.Length() > 0.0001f)
                    {
                        output.Add(left);
                    }
                    output.Add(newEdge.Clone());

                    right = orig.Clone(newEdge.B, orig.B);
                    if(right.Length() > 0.0001f)
                    {
                        output.Add(right);
                    }
                }
                else
                {
                    output.Add(poly.GetEdge(i));
                }
            }

            return new EdgePoly(output.ToArray());
        }

        public static bool IsParallel(IEdge a, IEdge b, float threshold = 0.9999f)
        {
            float dot = Vector3.Dot(a.Direction().normalized, b.Direction().normalized);
            return dot > threshold;
        }

        public static bool IsParallel(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, float threshold = 0.9999f)
        {
            float dot = Vector3.Dot((a2 - a1).normalized, (b2 - b1).normalized);
            return Mathf.Abs(dot) > threshold;
        }

        /// <summary>
        /// Make a concave polygon into a convex one
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static IEnumerable<IPoly> MakeConvex(IPoly poly)
        {
            List<Vector3> input = poly.GetPoints().ToList();
            List<List<Vector3>> output = BayazitDecomposer.ConvexPartition(input);
            return output.Select(x => poly.Clone(x.ToArray()));
        }

        private static bool TryIntersectPolygon(IPoly polygon, float theta, int index, out IPoly a, out IPoly b)
        {
            a = b = null;
            if(theta > Mathf.PI)
            {
                float[] tValues;
                int[] indicies;
                (tValues, indicies) = GetBisectedThetaValues(polygon, index);

                for(int i = indicies.Length - 1; i >= 0; i--)
                {
                    Vector3 v1 = polygon.GetPoint(index);
                    Vector3 v2 = polygon.GetPoint(indicies[i]);
                    if(!IsEdgeIntersectingPolygon(polygon, v1, v2))
                    {
                        (a, b) = Divide(polygon, index, i);
                        return true;
                    }
                }
            }
            return false;
        }

        public static (float[], int[]) GetBisectedThetaValues(IPoly polygon, int index)
        {
            Vector3 center = polygon.GetPoint(index);
            Vector3 left = polygon.GetPointWrapped(index - 1);
            Vector3 right = polygon.GetPointWrapped(index + 1);
            Vector3 biSect = Vector3.Lerp((left - center).normalized, (right - center).normalized, 0.5f).normalized;

            int[] indicies = new int[polygon.Resolution - 1];
            float[] tValues = new float[polygon.Resolution - 1];
            for(int i = 0; i < polygon.Resolution; i++)
            {
                if(i < index)
                {
                    indicies[i] = i;
                    tValues[i] = Vector3.Dot(biSect, (polygon.GetPoint(i) - center).normalized);
                }
                else if(i == index)
                {
                    continue;
                }
                else
                {
                    indicies[i - 1] = i;
                    tValues[i - 1] = Vector3.Dot(biSect, (polygon.GetPoint(i) - center).normalized);
                }
            }
            Array.Sort(tValues, indicies);
            return (tValues, indicies);
        }

        public static bool IsEdgeIntersectingPolygon(IPoly polygon, Vector3 point1, Vector3 point2)
        {
            for(int i = 0; i < polygon.Resolution; i++)
            {
                Vector3 a = polygon.GetPoint(i);
                Vector3 b = polygon.GetPointWrapped(i + 1);
                if((point1 - a).sqrMagnitude < 0.0001f)
                {
                    continue;
                }
                if((point1 - b).sqrMagnitude < 0.0001f)
                {
                    continue;
                }
                if((point2 - a).sqrMagnitude < 0.0001f)
                {
                    continue;
                }
                if((point2 - b).sqrMagnitude < 0.0001f)
                {
                    continue;
                }

                if(Math3d.AreLineSegmentsCrossing(a, b, point1, point2))
                {
                    return true;
                }
            }
            return false;
        }

        public static (IPoly, IPoly) Divide(IPoly polygon, int startIndex, int endIndex)
        {
            throw new System.NotImplementedException();
        }

        class GetIntersectionIEdge : IEdgeFuture
        {
            IPoly a, b;

            public GetIntersectionIEdge(IPoly a, IPoly b)
            {
                this.a = a;
                this.b = b;
            }

            protected override IEdge Generate()
            {
                for(int i = 0; i < a.Resolution; i++)
                {
                    IEdge ea = a.GetEdge(i);
                    for(int j = 0; j < b.Resolution; j++)
                    {

                        IEdge eb = b.GetEdge(j);
                        if(ea.CompareLine(eb) && Vector3.Dot(ea.Direction(), eb.Direction()) < 0)
                        {
                            return ea.GetIntersecting(eb);
                        }
                    }
                }
                throw new IEdgeSplitException("Failed to find intersection.", a, b);
            }

            class IEdgeSplitException : Exception
            {
                //Suppress the warning because this class is intended for the debugger.
#pragma warning disable 0414
                IPoly a, b;

                public IEdgeSplitException(string message, IPoly a, IPoly b) : base(message)
                {
                    this.a = a;
                    this.b = b;
                }
            }
        }

        /// <summary>
        /// Find the unit angle for a corner moving from a to b to c. 
        /// Assumes clockwise winding
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float InteriorAngle(Vector3 a, Vector3 b, Vector3 c, Vector3 normal)
        {
            Vector3 left = (a - b).normalized;
            Vector3 right = (c - b).normalized;
            float output = Mathf.Acos(Vector3.Dot(left, right));
            if(Vector3.Dot(Vector3.Cross(left, right), normal) < 0f)
            {
                return TAU - output;
            }
            else
            {
                return output;
            }
        }

        public static IPoly RemoveDuplicateVerticies(IPoly poly)
        {
            List<Vector3> output = new List<Vector3>();
            for(int i = 0; i < poly.Resolution; i++)
            {
                if(output.Count == 0)
                {
                    output.Add(poly.GetPoint(i));
                }
                else
                {
                    Vector3 point = poly.GetPoint(i);
                    Vector3 last = output[output.Count - 1];
                    if(!Math3d.Compare(point, last))
                    {
                        output.Add(point);
                    }
                }
            }
            if (Math3d.Compare(output[0], output[output.Count - 1]))
            {
                output.RemoveAt(0);
            }
            return poly.Clone(output.ToArray());
        }
    }
}