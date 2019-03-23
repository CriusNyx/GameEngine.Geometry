using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameEngine.Geometry
{
    public class Bezier : IPath, ICurve, ICurveAdv
    {
        IPath keyPoints;

        public Bezier(params Vector3[] keyPoints)
        {
            this.keyPoints = new Path(keyPoints);
        }

        public Bezier(IEnumerable<Vector3> keyPoints)
        {
            this.keyPoints = new Path(keyPoints);
        }

        public Bezier(IPath keyPoints)
        {
            this.keyPoints = keyPoints;
        }

        public int Resolution
        {
            get
            {
                return keyPoints.Resolution;
            }
        }

        public void Draw(float time = -1F)
        {
            Draw(Color.white, time);
        }

        public void Draw(Color color, float time = -1f)
        {
            Draw(color, time, 10);
        }

        public void Draw(Color color, float time, int resolution)
        {
            keyPoints.Draw(Color.red, time);
            this.GenericDraw(color, time, resolution);
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return keyPoints.GetEnumerator();
        }

        public Vector3 GetCurvePoint(float f)
        {
            return GetPoint(keyPoints.ToArray(), f, keyPoints.Resolution);
        }

        public Vector3 GetPathPoint(int index)
        {
            return keyPoints.GetPathPoint(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return keyPoints.GetEnumerator();
        }

        private Vector3 GetPoint(Vector3[] tempArr, float f, int max)
        {
            if (max == 2)
            {
                return Vector3.Lerp(tempArr[0], tempArr[1], f);
            }
            else
            {
                max--;
                for (int i = 0; i < max; i++)
                {
                    tempArr[i] = Vector3.Lerp(tempArr[i], tempArr[i + 1], f);
                }
                return GetPoint(tempArr, f, max);
            }
        }

        public Vector3 AdvGetTangent(float t, float delta)
        {
            Vector3[] directions = new Vector3[keyPoints.Resolution - 1];
            for(int i = 0; i < keyPoints.Resolution - 1; i++)
            {
                directions[i] = keyPoints.GetPathPoint(i + 1) - keyPoints.GetPathPoint(i);
            }
            return GetPoint(directions, t, directions.Length);
        }
    }
}