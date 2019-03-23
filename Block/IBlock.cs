using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    /// <summary>
    /// Represents a convex 3D shape, made from a surface of polygons.
    /// This class generalizes all such shapes, and provides extension methods for them.
    /// </summary>
    public interface IBlock : IDrawable, IEnumerable<IPoly>
    {
        IEnumerable<IPoly> GetFaces();
        IBlock Clone();
        IBlock Clone(IEnumerable<IPoly> faces);
    }
}