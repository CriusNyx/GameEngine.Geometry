using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameEngine.Geometry
{
    /// <summary>
    /// A simple implementation of IBlock
    /// </summary>
    public class Block : IBlock
    {
        List<IPoly> faces;

        public Vector3[] Bounds
        {
            get
            {
                Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
                Vector3 max = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);
                foreach(var f in faces)
                {
                    foreach(var v in f.GetPoints())
                    {
                        for(int i = 0; i < 3; i++)
                        {
                            if(v[i] < min[i])
                                min[i] = v[i];
                            if(v[i] > max[i])
                                max[i] = v[i];
                        }
                    }
                }
                return new Vector3[] { min, max };
            }
        }

        public Block(IEnumerable<IPoly> faces)
        {
            this.faces = faces.ToList();
        }

        public Block(params IPoly[] faces)
        {
            this.faces = faces.ToList();
        }

        public IEnumerable<IPoly> GetFaces()
        {
            return faces;
        }
        public IBlock Clone()
        {
            return new Block(faces.Select(x => x.Clone()));
        }
        public IBlock Clone(IEnumerable<IPoly> faces)
        {
            return new Block(faces);
        }

        public void Draw(float time = -1f)
        {
            Draw(Color.white, time);
        }
        public void Draw(Color color, float time = -1f)
        {
            foreach(var face in faces)
            {
                face.Draw(color, time);
            }
        }

        public IEnumerator<IPoly> GetEnumerator()
        {
            return faces.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return faces.GetEnumerator();
        }
    }
}