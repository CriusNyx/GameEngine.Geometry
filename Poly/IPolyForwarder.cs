using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry {
    /// <summary>
    /// An interface to forward the behaviour of many extension methods for polygons.
    /// For polygons which optimize certain methods, this allows those optimizations to be preseved for nested interfaces.
    /// </summary>
    public interface IPolyForwarder
    {
        IPoly GetPoly();
    }
}