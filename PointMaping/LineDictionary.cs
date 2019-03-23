using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A dictinoary that maps lines to other objects
/// Note, this dictionary does not make a distinction based on line direction
/// IE: a line going up, and a line going down will be the same
/// </summary>
/// <typeparam name="T"></typeparam>
public class LineDictionary<T> : IEnumerable<Tuple<Vector3, Vector3, T>>
{
    PointDictionary<PointDictionary<T>> intersections = new PointDictionary<PointDictionary<T>>();

    /// <summary>
    /// Add a new mapping to the dictionary defined by any point on the line and it's direction, that maps to value.
    /// If such a line already exists in the dictionary, it's value will be overwritten
    /// </summary>
    /// <param name="point"></param>
    /// <param name="direction"></param>
    /// <param name="value"></param>
    public void Add(Vector3 point, Vector3 direction, T value)
    {
        point = Math3d.LinePlaneIntersection(point, direction, Vector3.zero, direction);
        direction = direction.normalized;

        PointDictionary<T> vectors;
        if(!intersections.TryGet(point, out vectors))
        {
            vectors = new PointDictionary<T>();
            intersections.Add(point, vectors);
        }
        T t = default(T);
        if(vectors.TryGet(-direction, out t))
        {
            vectors.Add(-direction, t);
        }
        else
        {
            vectors.Add(direction, value);
        }
    }

    /// <summary>
    /// Get an object out of the dictinoary that matches the line
    /// </summary>
    /// <param name="point"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public T Get(Vector3 point, Vector3 direction)
    {
        T t;
        if(TryGet(point, direction, out t))
        {
            return t;
        }
        return default(T);
    }

    /// <summary>
    /// Attempt to get an object out of the dictionary, returning true if the object is in the dictinoary, and false otherwise
    /// The object will be returned to the out parameter t
    /// </summary>
    /// <param name="point"></param>
    /// <param name="direction"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool TryGet(Vector3 point, Vector3 direction, out T t)
    {
        point = Math3d.LinePlaneIntersection(point, direction, Vector3.zero, direction);
        direction = direction.normalized;
        PointDictionary<T> vectors;
        if(intersections.TryGet(point, out vectors))
        {
            if(vectors.TryGet(direction, out t))
            {
                return true;
            }
            else if(vectors.TryGet(-direction, out t))
            {
                return true;
            }
            else
            {
                t = default(T);
                return false;
            }
        }
        else
        {
            t = default(T);
            return false;
        }
    }

    /// <summary>
    /// Returns a set of tuples representing every object currently in the dictionary
    /// Output will be in the form (Point, Direction, Value)
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Tuple<Vector3, Vector3, T>> GetValues()
    {
        foreach(var setA in intersections)
        {
            Vector3 point = setA.Key;
            foreach(var setB in setA.Value)
            {
                Vector3 direction = setB.Key;
                T t = setB.Value;
                yield return new Tuple<Vector3, Vector3, T>(point, direction, t);
            }
        }
    }

    /// <summary>
    /// Returns a set of tuples representing every object currently in the dictionary
    /// Output will be in the form (Point, Direction, Value)
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Tuple<Vector3, Vector3, T>> GetEnumerator()
    {
        foreach(var value in GetValues())
        {
            yield return value;
        }
    }

    /// <summary>
    /// Returns a set of tuples representing every object in the dictinoary
    /// Output will be in the form (Point, Direction, T)
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach(var value in GetValues())
        {
            yield return value;
        }
    }
}
