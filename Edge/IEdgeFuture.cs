using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    public abstract class IEdgeFuture : IEdge
    {
        private IEdge futurePrivate;
        protected IEdge future
        {
            get
            {
                if (futurePrivate == null)
                    futurePrivate = Generate();
                return futurePrivate;
            }
        }

        public Vector3 A
        {
            get
            {
                return future.A;
            }
        }

        public Vector3 B
        {
            get
            {
                return future.B;
            }
        }

        public Vector3 Normal
        {
            get
            {
                return future.Normal;
            }
        }

        public Vector3 SurfaceNormal
        {
            get
            {
                return future.SurfaceNormal;
            }
        }

        public IEdge Clone()
        {
            return future.Clone();
        }

        public IEdge Clone(Vector3 a, Vector3 b)
        {
            return future.Clone(a, b);
        }

        public void Draw(float time = -1F)
        {
            future.Draw(time);
        }

        public void Draw(Color color, float time = -1F)
        {
            future.Draw(color, time);
        }

        protected abstract IEdge Generate();

        public IEdge Unwrap()
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
    }
}