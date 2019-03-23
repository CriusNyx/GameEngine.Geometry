using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Geometry;
using System.Linq;

namespace GameEngine.CSG3D
{
    /// <summary>
    /// A 3D block with an RValue
    /// The basis for a CSGVector3
    /// </summary>
    public class CSGBlock : IBlock
    {
        IBlock block;
        public float rValue = 1f;

        public CSGBlock()
        {
        }

        public CSGBlock(IBlock block, float rValue = 1f)
        {
            this.block = block;
            this.rValue = rValue;
        }

        public CSGBlock(IEnumerable<IPoly> faces, float rValue = 1f) : this(new Block(faces), rValue)
        {
        }

        public CSGBlock(float rValue, params IPoly[] faces) : this(faces, rValue)
        {
        }

        public CSGBlock(params IPoly[] faces) : this(1f, faces)
        {
        }

        public IEnumerable<IPoly> GetFaces()
        {
            return block.GetFaces();
        }
        public IBlock Clone()
        {
            return new CSGBlock(block.Clone(), rValue);
        }
        public IBlock Clone(IEnumerable<IPoly> faces)
        {
            return new CSGBlock(block.Clone(faces), rValue);
        }
        public void Draw(float time = -1f)
        {
            block.Draw(time);
        }
        public void Draw(Color color, float time = -1f)
        {
            block.Draw(color, time);
        }

        public IEnumerator<IPoly> GetEnumerator()
        {
            return block.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return block.GetEnumerator();
        }

        public static CSGBlock operator *(float a, CSGBlock b)
        {
            return new CSGBlock(b.block.Clone(), a * b.rValue);
        }
    }
}