using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    /// <summary>
    /// Represents an interface to generalize all 2D and 3D polygons.
    /// </summary>
    public interface IPoly : IDrawable
    {
        /// <summary>
        /// Gets the nth point in the polygon
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vector3 GetPoint(int index);

        /// <summary>
        /// Gets all the points of the polygon
        /// </summary>
        /// <returns></returns>
        Vector3[] GetPoints();

        /// <summary>
        /// Gets the number of points of the polygon
        /// </summary>
        /// <returns></returns>
        int Resolution { get; }
        /// <summary>
        /// Clones the polygon
        /// </summary>
        /// <returns></returns>
        IPoly Clone();
        /// <summary>
        /// Creates a clone of the polygon with a different list of verticies
        /// </summary>
        /// <param name="newPoints"></param>
        /// <returns></returns>
        IPoly Clone(IEnumerable<Vector3> newPoints);
    }
}