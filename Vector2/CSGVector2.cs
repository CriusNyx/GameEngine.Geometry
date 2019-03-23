using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameEngine.CSG;
using GameEngine.CSG.Geometry;
using GameEngine.Geometry;

namespace GameEngine.CSG2D
{
    /// <summary>
    /// A Vector made out of 2D shapes.
    /// Fullfills all of the characteristics of a vector space, allowing vector space operations to be preformed on the shapes
    /// </summary>
    public class CSGVector2 : IEnumerable<CSGShape>
    {
        public static bool AutoRemoveZeros = true;

        List<CSGShape> shapes = new List<CSGShape>();

        public CSGVector2()
        {

        }

        public CSGVector2(IEnumerable<CSGShape> shapes)
        {
            this.shapes = shapes.ToList();
        }

        public CSGVector2(params CSGShape[] shapes)
        {
            this.shapes = shapes.ToList();
        }

        public static implicit operator CSGVector2(CSGShape shape)
        {
            return new CSGVector2(shape);
        }

        public static CSGVector2 operator +(CSGVector2 a, CSGVector2 b)
        {
            List<CSGShape> intersec = new List<CSGShape>();
            var aOnly = CSGGeometry.VectorSetSubtract(a.shapes, b.shapes, intersec);
            var bOnly = CSGGeometry.VectorSetSubtract(b.shapes, a.shapes);
            intersec.AddRange(aOnly);
            intersec.AddRange(bOnly);
            CSGVector2 output = new CSGVector2(intersec);
            if (AutoRemoveZeros)
                output.RemoveZeros();
            return output;
        }

        public static CSGVector2 operator -(CSGVector2 a, CSGVector2 b)
        {
            return a + -b;
        }

        public static CSGVector2 operator *(float a, CSGVector2 b)
        {
            return new CSGVector2(b.shapes.Select(x => a * x));
        }

        public static CSGVector2 operator *(CSGVector2 a, float b)
        {
            return b * a;
        }

        public static CSGVector2 operator -(CSGVector2 a)
        {
            return -1f * a;
        }

        public static CSGVector2 operator /(CSGVector2 a, float b)
        {
            return 1f / b * a;
        }

        public IEnumerator<CSGShape> GetEnumerator()
        {
            return ((IEnumerable<CSGShape>)shapes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<CSGShape>)shapes).GetEnumerator();
        }

        /// <summary>
        /// Remove all shapes with an r value of zero.
        /// This can be useful for preformance.
        /// </summary>
        public void RemoveZeros()
        {
            shapes.RemoveAll(x => x.rValue == 0f);
        }

        /// <summary>
        /// Sets each block to have an rValue of 1, 0, or -1.
        /// Useful to optimize complex geometry.
        /// </summary>
        public void Normalize()
        {
            foreach(var shape in shapes)
            {
                shape.rValue = Mathf.Sign(shape.rValue);
            }
        }
    }
}