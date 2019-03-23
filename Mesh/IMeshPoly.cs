using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    public interface IMeshPoly<T> : IPolyForwarder
    {
        IEnumerable<T> adjacent { get; }
    }
}