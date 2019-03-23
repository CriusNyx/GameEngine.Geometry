using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameEngine.Geometry
{
    /// <summary>
    /// A poly defined by it's edges
    /// Edge polys are useful for wrapping edge data inside polygons.
    /// Be mindeful, some algorithms may destroy data warpped inside of edge polygons, since edge polygons are converted to NGons when cloning.
    /// </summary>
    public class EdgePoly : IPoly
    {
        IEdge[] edges;

        public EdgePoly(params IEdge[] edges)
        {
            this.edges = edges;
        }

        public EdgePoly(IEnumerable<IEdge> edges)
        {
            this.edges = edges.ToArray();
        }

        public int Resolution => edges.Length;

        public IPoly Clone()
        {
            return new EdgePoly(edges.Select(x => x.Clone()));
        }

        public IPoly Clone(IEnumerable<Vector3> newPoints)
        {
            return new NGon(newPoints);
        }

        public void Draw(float time = -1)
        {
            Draw(Color.white, time);
        }

        public void Draw(Color color, float time = -1)
        {
            foreach(var edge in edges)
            {
                edge.Draw(color, time);
            }
        }

        public Vector3 GetPoint(int index)
        {
            return edges[index].A;
        }

        public Vector3[] GetPoints()
        {
            return edges.Select(x => x.A).ToArray();
        }

        public IEdge GetEdge(int i)
        {
            return edges[i];
        }
    }
}