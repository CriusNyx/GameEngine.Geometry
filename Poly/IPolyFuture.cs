using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    public abstract class IPolyFuture : IPoly
    {
        private IPoly futurePrivate;
        protected IPoly future
        {
            get
            {
                if (futurePrivate == null)
                    futurePrivate = Generate();
                return futurePrivate;
            }
        }

        public int Resolution {
            get
            {
                return future.Resolution;
            }
        }

        public IPoly Clone()
        {
            return future.Clone();
        }

        public IPoly Clone(IEnumerable<Vector3> newPoints)
        {
            return future.Clone(newPoints);
        }

        public void Draw(float time = -1F)
        {
            future.Draw(time);
        }

        public void Draw(Color color, float time = -1F)
        {
            future.Draw(color, time);
        }

        public Vector3 GetPoint(int index)
        {
            return future.GetPoint(index);
        }

        protected abstract IPoly Generate();

        public IPoly Unwrap()
        {
            return future;
        }

        public bool IsReady
        {
            get
            {
                return futurePrivate != null;
            }
        }

        public override string ToString()
        {
            return future.ToString();
        }

        public Vector3[] GetPoints()
        {
            return future.GetPoints();
        }
    }
}