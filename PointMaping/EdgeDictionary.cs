using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameEngine.Geometry {
    /// <summary>
    /// Similar to the line dictionary, but allows entities to be mapped to edges instead of lines
    /// In the line dictionary, it's garranteed that you will have a 1 : 1 mapping between a line and an object,
    /// but in this dictionary M : N relationships are possible, since a line may map to multiple edges, 
    /// and a single edge may map to multiple entities
    /// 
    /// To create this dictionary, you must first add a set of "Key Edges" which all other edges will be compared against,
    /// After this, you will map non key edges to key edges, therefore mapping key edges to data.
    /// Edges passed in that do not map to key edges will be discarded.
    /// This dictionary does not allow duplicate values to be mapped to a single key edge
    /// </summary>
    public class EdgeDictionary<T> : IEnumerable<(IEdge, T)>
    {
        LineDictionary<Dictionary<IEdge, List<T>>> dic = new LineDictionary<Dictionary<IEdge, List<T>>>();

        /// <summary>
        /// Adds a key edge to the dictionary
        /// </summary>
        /// <param name="edge"></param>
        public void AddKeyEdge(IEdge edge)
        {
            Dictionary<IEdge, List<T>> dictionary;
            if(!dic.TryGet(edge.A, edge.Direction(), out dictionary))
            {
                dictionary = new Dictionary<IEdge, List<T>>();
                dic.Add(edge.A, edge.Direction(), dictionary);
            }
            dictionary.Add(edge, new List<T>());
        }

        /// <summary>
        /// Adds a mapping for a non key edge to the dictionary
        /// This will map any key edge that shares space with the edge with the value.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAddEntity(IEdge edge, T value)
        {
            Dictionary<IEdge, List<T>> mapping;
            if(dic.TryGet(edge.A, edge.Direction(), out mapping))
            {
                bool output = false;
                foreach(var pair in mapping)
                {
                    var otherKey = pair.Key;
                    if(Math3d.AreLineSegmentsIntersecting(edge.A, edge.B, otherKey.A, otherKey.B))
                    {
                        if(!pair.Value.Contains(value))
                        {
                            pair.Value.Add(value);
                        }
                        output = true;
                    }
                }
                return output;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a set of values that the key edge maps to
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(IEdge key)
        {
            Dictionary<IEdge, List<T>> mapping;
            if(dic.TryGet(key.A, key.Direction(), out mapping))
            {
                if(mapping.ContainsKey(key))
                {
                    return mapping[key];
                }
            }
            return null;
        }

        /// <summary>
        /// Get a set of edges in the dictionary
        /// </summary>
        /// <returns></returns>
        public IEnumerator<(IEdge, T)> GetEnumerator()
        {
            foreach((var point, var direction, var dictinoary) in dic)
            {
                foreach(var pair in dictinoary)
                {
                    foreach(var element in pair.Value)
                    {
                        yield return (pair.Key, element);
                    }
                }
            }
        }

        /// <summary>
        /// Get a set of all objects in the dictionary
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}