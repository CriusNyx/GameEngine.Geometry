using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameEngine.Geometry
{
    /// <summary>
    /// utility class for constructing cubes, and other block primitives
    /// </summary>
    public static class Cube
    {
        public static IBlock Create(Vector3 position, Quaternion rotation, Vector3 scale, Func<IEnumerable<IPoly>, Block> blockConstructor = null, Func<IEnumerable<Vector3>, IPoly> faceConstructor = null)
        {
            Matrix4x4 trs = Matrix4x4.TRS(position, rotation, scale);
            return Create(Vector3.one, trs, blockConstructor, faceConstructor);
        }

        public static IBlock Create(Vector3 scale, Func<IEnumerable<IPoly>, Block> blockConstructor = null, Func<IEnumerable<Vector3>, IPoly> faceConstructor = null)
        {
            return Create(scale, Matrix4x4.identity, blockConstructor, faceConstructor);
        }

        public static IBlock Create(Vector3 scale, Matrix4x4 transform, Func<IEnumerable<IPoly>, Block> blockConstructor = null, Func<IEnumerable<Vector3>, IPoly> faceConstructor = null)
        {
            if (faceConstructor == null) faceConstructor = x => new NGon(x);
            if (blockConstructor == null) blockConstructor = x => new Block(x);

            scale = 0.5f * scale;
            Vector3[] points = new Vector3[]{
                new Vector3(-scale.x, -scale.y, -scale.z),
                new Vector3(scale.x, -scale.y, -scale.z),
                new Vector3(-scale.x, -scale.y, scale.z),
                new Vector3(scale.x, -scale.y, scale.z),
                new Vector3(-scale.x, scale.y, -scale.z),
                new Vector3(scale.x, scale.y, -scale.z),
                new Vector3(-scale.x, scale.y, scale.z),
                new Vector3(scale.x, scale.y, scale.z)};

            for (int i = 0; i < 8; i++)
            {
                points[i] = transform.MultiplyPoint(points[i]);
            }

            //bottom, top, left, right, front, back
            IPoly[] faces = new IPoly[]{
                faceConstructor(new Vector3[]{points[0], points[1], points[3], points[2] }),
                faceConstructor(new Vector3[]{points[4], points[6], points[7], points[5] }),
                faceConstructor(new Vector3[]{points[6], points[4], points[0], points[2] }),
                faceConstructor(new Vector3[]{points[1], points[5], points[7], points[3] }),
                faceConstructor(new Vector3[]{points[0],points[4], points[5], points[1] }),
                faceConstructor(new Vector3[]{points[3], points[7], points[6], points[2] }) };

            return blockConstructor(faces);
        }
    }
}