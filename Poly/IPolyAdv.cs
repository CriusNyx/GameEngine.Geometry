using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Geometry
{
    /// <summary>
    /// Allows certain extension methods to be implemented for polygon.
    /// This allows for certain optimizations to be made, such as constants only being calculated the first time they are called
    /// 
    /// Deprecated. The functionality of this class was split up to give more fine grain control to other classes
    /// </summary>
    //public interface IPolyAdv
    //{
    //    float AdvGetArea();
    //    float AdvGetPerimeter();
    //    Vector3 AdvGetNormal();
    //    Vector3 AdvGetSurfaceNormal(int index);
    //}

    /// <summary>
    /// Implement this to override area extension method
    /// </summary>
    public interface IPolyExtensionArea
    {
        float AdvGetArea();
    }

    /// <summary>
    /// Implement this to override perimeter extension method
    /// </summary>
    public interface IPolyExtensionPerimeter
    {
        float AdvGetPerimeter();
    }

    /// <summary>
    /// Implement this to override normal extension method
    /// </summary>
    public interface IPolyExtensionNormal
    {
        Vector3 AdvGetNormal();
    }

    /// <summary>
    /// Implement this to override surface normal extension method
    /// </summary>
    public interface IPolyExtensionSurfaceNormal
    {
        Vector3 AdvGetSurfaceNormal(int index);
    }

    /// <summary>
    /// Implement this interface to allow the edges of the polygon to be extended, and wrapped
    /// </summary>
    //public interface IPolyExtensionEdgeWrapper
    //{
    //    IEdge AdvGetEdge(int index);
    //    IEnumerable<IEdge> AdvGetEdges();
    //    void SetEdge(int index, IEdge edge);
    //    void SetEdge(IEdge original, IEdge newEdge);
    //}
}