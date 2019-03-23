using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameEngine.Geometry
{
    public class Path : IPath
    {
        Vector3[] points;

        public Path(params Vector3[] points)
        {
            this.points = points;
        }

        public Path(IEnumerable<Vector3> points)
        {
            this.points = points.ToArray();
        }

        public int Resolution
        {
            get
            {
                return points.Length;
            }
        }

        public void Draw(float time = -1F)
        {
            Draw(Color.white, time);
        }

        public void Draw(Color color, float time = -1F)
        {
            this.GenericDraw(color, time);
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            for (int i = 0; i < Resolution; i++)
            {
                yield return points[i];
            }
        }

        public Vector3 GetPathPoint(int index)
        {
            if (index >= Resolution)
                throw new System.ArgumentException("The requested point is outside of the path");
            return points[index];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return points.GetEnumerator();
        }
    }
}