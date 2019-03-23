using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameEngine.Geometry
{
    public static class Primitive
    {
        public static IPoly MakeBox(Vector3 center, Vector2 size, Func<Vector3[], IPoly> constructor = null)
        {
            return MakeBox(center, Quaternion.identity, size, constructor);
        }

        public static IPoly MakeBox(Vector3 center, Quaternion rotation, Vector2 size, Func<Vector3[], IPoly> constructor = null)
        {
            if (constructor == null)
                constructor = (x) => new NGon(x);
            Vector3 right = rotation * Vector3.right * size.x * 0.5f;
            Vector3 up = rotation * Vector3.forward * size.y * 0.5f;
            return constructor(new Vector3[] { center - right + up, center + right + up, center + right - up, center - right - up });
        }

        public static IBlock Extrude(IPoly polygon, Vector3 direction, float floorDistance, float ceilingDistance, Func<IEnumerable<Vector3>, IPoly> polyConstructor = null, Func<IEnumerable<IPoly>, IBlock> blockConstructor = null)
        {
            if(polyConstructor == null)
                polyConstructor = polygon.Clone;
            IPoly floor = polygon.Clone(polygon.GetPoints().Select(x => x + direction * floorDistance));
            IPoly ceiling = polygon.Clone(polygon.GetPoints().Select(x => x + direction * ceilingDistance));
            List<IPoly> faces = new List<IPoly>();
            for(int i = 0; i < polygon.Resolution; i++)
            {
                Vector3 lr = floor.GetPoint(i);
                Vector3 ll = floor.GetPoint((i + 1) % polygon.Resolution);
                Vector3 ul = ceiling.GetPoint((i + 1) % polygon.Resolution);
                Vector3 ur = ceiling.GetPoint(i);
                faces.Add(polyConstructor(new Vector3[] { lr, ll, ul, ur }));
            }
            faces.Add(polyConstructor(ceiling.GetPoints()));
            faces.Add(polyConstructor(floor.GetPoints().Reverse()));

            if (blockConstructor == null)
                blockConstructor = (x) => new Block(x);

            return blockConstructor(faces);
        }
    }
}