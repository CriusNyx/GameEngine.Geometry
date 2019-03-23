using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    public class LerpPath : IPath, ICurve
    {
        IPath keyPoints;

        public LerpPath(params Vector3[] keyPoints)
        {
            this.keyPoints = new Path(keyPoints);
        }

        public LerpPath(IEnumerable<Vector3> keyPoints)
        {
            this.keyPoints = new Path(keyPoints);
        }

        public LerpPath(IPath keyPoints)
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
            keyPoints.Draw(time);
        }

        public void Draw(Color color, float time = -1F)
        {
            keyPoints.Draw(color, time);
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return keyPoints.GetEnumerator();
        }

        public Vector3 GetPathPoint(int index)
        {
            return keyPoints.GetPathPoint(index);
        }

        public Vector3 GetCurvePoint(float t)
        {
            if (t == 1)
                return keyPoints.GetPathPoint(Resolution - 1);
            else
            {
                int index = Mathf.FloorToInt(t * Resolution);
                t = (t * Resolution - index);
                return Vector3.Lerp(keyPoints.GetPathPoint(index), keyPoints.GetPathPoint(index + 1), t);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return keyPoints.GetEnumerator();
        }
    }
}