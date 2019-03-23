using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    public interface ICurve : IDrawable
    {
        Vector3 GetCurvePoint(float t);
    }

    public interface ICurveAdv
    {
        Vector3 AdvGetTangent(float t, float delta);
    }

    static class ICurveExtensions
    {
        public static void GenericDraw(this ICurve curve, Color color, float time, int resolution)
        {
            for(int i = 0; i < resolution; i++)
            {
                float t = (float)i / (resolution);
                float tPlus = (float)(i + 1) / resolution;
                Vector3 a = curve.GetCurvePoint(t);
                Vector3 b = curve.GetCurvePoint(tPlus);
                Debug.DrawLine(a, b, color, time);
            }
        }

        public static Vector3 GetTangent(this ICurve curve, float t, float delta = 0.001f)
        {
            if (curve is ICurveAdv)
            {
                return ((ICurveAdv)curve).AdvGetTangent(t, delta);
            }
            else
            {
                float tPlus = Mathf.Clamp01(t + delta);
                float tMinus = Mathf.Clamp01(t - delta);
                Vector3 a = curve.GetCurvePoint(tMinus);
                Vector3 b = curve.GetCurvePoint(tPlus);
                return b - a;
            }
        }
    }
}