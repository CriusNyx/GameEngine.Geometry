using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameEngine.Geometry
{
    public class PolyMesh<T> : IDrawable where T : IPoly
    {
        T[] polygons;
        //Dictionary<T, List<T>> adjacencyMap = new Dictionary<T, List<T>>();
        Dictionary<T, MeshPoly<T, IEdge>> polyMap = new Dictionary<T, MeshPoly<T, IEdge>>();

        public PolyMesh(T[] polygons, IEnumerable<IEdge> keyEdges = null)
        {
            this.polygons = polygons;
            GeneratePolyMap(polygons);
            GenerateMesh(keyEdges);
        }

        public PolyMesh(IEnumerable<PolyMesh<T>> subMeshes, IEnumerable<IEdge> keyEdges = null)
        {
            List<T> polygons = new List<T>();
            foreach(var mesh in subMeshes)
            {
                foreach(var pair in mesh.polyMap)
                {
                    polyMap.Add(pair.Key, pair.Value);
                    polygons.Add(pair.Key);
                }
            }
            this.polygons = polygons.ToArray();
            if(keyEdges != null)
            {
                GenerateMesh(keyEdges, true);
            }
        }

        private void GeneratePolyMap(T[] polygons)
        {
            //Generate the poly map for the polygons
            foreach(var poly in polygons)
            {
                polyMap.Add(poly, new MeshPoly<T, IEdge>(poly));
            }
        }

        private void GenerateMesh(IEnumerable<IEdge> keyEdges, bool debug = false)
        {
            //Initialize algorithm
            EdgeDictionary<T> mapping = new EdgeDictionary<T>();

            //generate key edges if needed
            if(keyEdges == null)
            {
                List<IEdge> eList = new List<IEdge>();
                keyEdges = eList;
                foreach(var poly in polygons)
                {
                    foreach(var edge in poly.Edges())
                    {
                        eList.Add(edge);
                    }
                }
            }

            //add key edges to map
            foreach(var edge in keyEdges)
            {
                mapping.AddKeyEdge(edge);
            }

            if(debug)
            {
                foreach(var edge in keyEdges)
                {
                    var map = mapping.Get(edge);
                }
            }

            //add polygons to map
            foreach(var poly in polygons)
            {
                foreach(var edge in poly.Edges())
                {
                    mapping.TryAddEntity(edge, poly);
                }
            }

            if(debug)
            {
                foreach(var edge in keyEdges)
                {
                    var map = mapping.Get(edge);
                }
            }

            //link polygons
            foreach(var edge in keyEdges)
            {
                var polys = mapping.Get(edge);
                foreach(var poly in polys)
                {
                    foreach(var other in polys)
                    {
                        if((IPoly)other == (IPoly)poly)
                        {
                            continue;
                        }
                        polyMap[poly].LinkPolygon(edge, polyMap[other]);
                        polyMap[other].LinkPolygon(edge, polyMap[poly]);
                    }
                }
            }

            //LineDictionary<Dictionary<IEdge, T>> edges = new LineDictionary<Dictionary<IEdge, T>>();

            ////generate dictinoary from edges
            //foreach(var poly in polygons)
            //{
            //    foreach(var edge in poly.Edges())
            //    {
            //        AddEdgeToDictionary(edges, poly, edge);
            //    }
            //}

            //foreach(var poly in polygons)
            //{
            //    CheckPolyForLinks(poly, edges);
            //}
        }

        //private void AddEdgeToDictionary(LineDictionary<Dictionary<IEdge, T>> dic, T poly, IEdge edge)
        //{
        //    Vector3 point = edge.A;
        //    Vector3 direction = edge.Direction();
        //    Dictionary<IEdge, T> list;
        //    if(dic.TryGet(edge.A, direction, out list))
        //    {
        //        list.Add(edge, poly);
        //    }
        //    else
        //    {
        //        list = new Dictionary<IEdge, T>();
        //        list.Add(edge, poly);
        //        dic.Add(point, direction, list);
        //    }
        //}

        //private void CheckPolyForLinks(T poly, LineDictionary<Dictionary<IEdge, T>> dic)
        //{
        //    List<T> adj = new List<T>();
        //    foreach(var edge in poly.Edges())
        //    {
        //        Vector3 point = edge.A;
        //        Vector3 direction = -edge.Direction();
        //        Dictionary<IEdge, T> list;
        //        if(dic.TryGet(point, direction, out list))
        //        {
        //            foreach(var other in list)
        //            {
        //                if(Math3d.AreLineSegmentsIntersecting(edge.A, edge.B, other.Key.A, other.Key.B))
        //                {
        //                    if(!adj.Contains(other.Value))
        //                    {
        //                        adj.Add(other.Value);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    adjacencyMap.Add(poly, adj);
        //}

        public void Draw(float time = -1)
        {
            foreach(var poly in polyMap)
            {
                poly.Value.Draw(time);
            }
        }

        public void Draw(Color color, float time = -1)
        {
            foreach(var poly in polyMap)
            {
                poly.Value.Draw(color, time);
            }
        }
    }

    public class MeshPoly<T, U> : IDrawable
        where T : IPoly
        where U : IEdge
    {
        public readonly T polygon;
        List<MeshPortal<T, U>> adjacent = new List<MeshPortal<T, U>>();

        public MeshPoly(T polygon)
        {
            this.polygon = polygon;
        }

        public void Draw(float time = -1) => Draw(Color.white, time);

        public void Draw(Color color, float time = -1)
        {
            polygon.Draw(color, time);
            Vector3 center = polygon.Center();
            foreach(var link in adjacent)
            {
                Vector3 otherCenter = link.otherPoly.polygon.Center();
                Debug.DrawLine(center, Vector3.Lerp(center, otherCenter, 0.4f), color, time);
            }
        }

        public void LinkPolygon(U portal, MeshPoly<T, U> polygon, bool allowDuplicates = false)
        {
            if(allowDuplicates || !adjacent.Any(x => x.otherPoly == polygon))
            {
                adjacent.Add(new MeshPortal<T, U>(this, polygon, portal));
            }
        }
    }

    public class MeshPortal<T, U>
        where T : IPoly
        where U : IEdge
    {
        public readonly MeshPoly<T, U> thisPoly;
        public readonly MeshPoly<T, U> otherPoly;
        public readonly U connectingEdge;

        public MeshPortal(MeshPoly<T, U> thisPoly, MeshPoly<T, U> otherPoly, U connectingEdge)
        {
            this.thisPoly = thisPoly;
            this.otherPoly = otherPoly;
            this.connectingEdge = connectingEdge;
        }
    }
}