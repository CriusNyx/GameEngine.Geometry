using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameEngine.Geometry
{
    public class Parametric : ICurve
    {
        private Func<float, Vector3> pointFunction;

        public Parametric(Func<float, Vector3> pointFunction)
        {
            this.pointFunction = pointFunction;
        }

        public Parametric(Func<float, Vector3> pointFunction, float startTime, float endTime)
        {
            this.pointFunction = (x) => pointFunction((x) * (endTime - startTime) + startTime);
        }

        public void Draw(float time = -1F)
        {
            Draw(Color.white, time);
        }

        public void Draw(Color color, float time = -1F)
        {
            this.GenericDraw(color, time, 10);
        }

        public Vector3 GetCurvePoint(float t)
        {
            return pointFunction(t);
        }
    }
}