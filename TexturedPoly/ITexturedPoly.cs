using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    /// <summary>
    /// An interface for a polygon with a material attached.
    /// </summary>
    public interface ITexturedPoly : IPoly
    {
        void SetMaterial(Material mat);
        Material GetMaterial();
        Vector2 GetTextureMapping(Vector3 point);
    }
}