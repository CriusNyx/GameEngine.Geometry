using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameEngine.CSG.Geometry;
using GameEngine.Geometry;

namespace GameEngine.CSG3D
{
    /// <summary>
    /// A vector made out of 3D blocks.
    /// Fulfills all of the properties of a vector spaces, allowing for arithmatic operations to be preformed on blocks.
    /// </summary>
    public class CSGVector3 : IEnumerable<CSGBlock>
    {
        List<CSGBlock> blocks = new List<CSGBlock>();

        #region Construction
        public CSGVector3()
        {

        }

        public CSGVector3(IEnumerable<CSGBlock> blocks)
        {
            this.blocks = blocks.ToList();
        }
        public CSGVector3(IEnumerable<IBlock> blocks)
        {
            this.blocks = blocks.Select(x => new CSGBlock(x)).ToList();
        }

        public CSGVector3(params CSGBlock[] blocks)
        {
            this.blocks = blocks.ToList();
        }
        public CSGVector3(params IBlock[] blocks)
        {
            this.blocks = blocks.Select(x => new CSGBlock(x)).ToList();
        }
        #endregion

        #region Operators
        public static CSGVector3 operator *(float a, CSGVector3 b)
        {
            return new CSGVector3(b.blocks.Select(x => a * x));
        }

        public static CSGVector3 operator *(CSGVector3 a, float b)
        {
            return b * a;
        }

        public static CSGVector3 operator -(CSGVector3 a)
        {
            return -1f * a;
        }

        public static CSGVector3 operator /(CSGVector3 a, float b)
        {
            return 1f / b * a;
        }

        public static CSGVector3 operator +(CSGVector3 a, CSGVector3 b)
        {
            //This operation works via set subtraction
            //aOnly = a - b
            //bOnly = b - a
            //intersec = a - aOnly
            List<CSGBlock> intersec = new List<CSGBlock>();
            var aOnly = CSGGeometry.VectorSetSubtract(a.blocks, b.blocks, intersec);
            var bOnly = CSGGeometry.VectorSetSubtract(b.blocks, a.blocks);

            //aOnly and bOnly are merged with intersec to create a new vector
            intersec.AddRange(aOnly);
            intersec.AddRange(bOnly);
            CSGVector3 output = new CSGVector3(intersec);

            //Remove zeroes is processeced, if required
            if (AutoRemoveZeros)
                output.RemoveZeros();

            //the vector is returned
            return output;
        }

        public static CSGVector3 operator -(CSGVector3 a, CSGVector3 b)
        {
            return a + -b;
        }
        #endregion

        #region Math Methods
        /// <summary>
        /// Sets each block to have an rValue of 1, 0, or -1.
        /// Useful to optimize complex geometry.
        /// </summary>
        public void Normalize()
        {
            foreach(var block in blocks)
            {
                block.rValue = Mathf.Sign(block.rValue);
            }
        }

        public void Simplify()
        {
            blocks.RemoveAll(x => x.rValue <= 0);
            blocks.ForEach(x => x.rValue = 1);
        }

        public static bool AutoRemoveZeros = true;
        /// <summary>
        /// Remove all zero blocks, since in many cases they represent empty space
        /// </summary>
        public void RemoveZeros()
        {
            blocks.RemoveAll(x => x.rValue == 0f);
        }
        #endregion

        #region Game Methods
        /// <summary>
        /// Converts everything to a mesh
        /// A game object is required as a mesh container to prevent leaking
        /// </summary>
        /// <param name="go"></param>
        //public void MakeMesh(GameObject go)
        //{
        //    foreach (GameObject child in go.transform)
        //    {
        //        GameObject.Destroy(child);
        //    }
        //    foreach (var block in blocks)
        //    {
        //        GameObject container = new GameObject("Block");
        //        container.transform.parent = go.transform;
        //        container.transform.position = go.transform.position;
        //        container.transform.rotation = go.transform.rotation;
        //        container.transform.localScale = go.transform.localScale;
        //        block.MakeMesh(container);
        //    }
        //}

        //public void MakeMeshCrude(GameObject go)
        //{
        //    //foreach (GameObject child in go.transform)
        //    //{
        //    //    GameObject.Destroy(child);
        //    //}
        //    foreach (var block in blocks)
        //    {
        //        GameObject container = new GameObject("Block");
        //        container.transform.parent = go.transform;
        //        container.transform.position = go.transform.position;
        //        container.transform.rotation = go.transform.rotation;
        //        container.transform.localScale = go.transform.localScale;
        //        block.MakeMeshCrude(container);
        //    }
        //}
        #endregion

        #region Interfaces
        public IEnumerator<CSGBlock> GetEnumerator()
        {
            return ((IEnumerable<CSGBlock>)blocks).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CSGBlock>)blocks).GetEnumerator();
        }
        #endregion
    }
}