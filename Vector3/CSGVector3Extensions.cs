using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Geometry;
using System;

namespace GameEngine.CSG3D
{
    static class CSGVector3Extensions
    {
        public static void MakeMesh(this CSGVector3 vector, GameObject go, Action<GameObject> goAction = null)
        {
            foreach (var block in vector)
            {
                GameObject container = new GameObject("Block");
                container.transform.parent = go.transform;
                container.transform.position = go.transform.position;
                container.transform.rotation = go.transform.rotation;
                container.transform.localScale = go.transform.localScale;
                block.MakeMesh(container);
                if (goAction != null) goAction(container);
            }
        }

        public static void MakeMeshCrude(this CSGVector3 vector, GameObject go)
        {
            foreach (var block in vector)
            {
                GameObject container = new GameObject("Block");
                container.transform.parent = go.transform;
                container.transform.position = go.transform.position;
                container.transform.rotation = go.transform.rotation;
                container.transform.localScale = go.transform.localScale;
                block.MakeMeshCrude(container);
            }
        }
    }
}