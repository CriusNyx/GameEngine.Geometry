using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameEngine.Geometry {
    public static class Draw
    {
        public static void Cross(Vector3 point, float distance, float time = -1f)
        {
            Cross(point, distance, Color.white, time);
        }

        public static void Cross(Vector3 point, float distance, Color color, float time = -1f)
        {
            Debug.DrawRay(point, Vector3.up * distance, color, time);
            Debug.DrawRay(point, Vector3.down * distance, color, time);
            Debug.DrawRay(point, Vector3.left * distance, color, time);
            Debug.DrawRay(point, Vector3.right * distance, color, time);
            Debug.DrawRay(point, Vector3.forward * distance, color, time);
            Debug.DrawRay(point, Vector3.back * distance, color, time);
        }
    }
}
