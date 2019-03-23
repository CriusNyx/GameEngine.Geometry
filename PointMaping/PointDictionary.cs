using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// An efficiant data structure for mapping 3D points to objects
/// This structure accounts for floating point inpercision
/// This structure works best when there exists tight clusters of points with large amounts of space between them
/// 
/// It works by using a divide and conquor algorithm to store and retreive points.
/// It does not always garentee that two points within the percision threshold will return the same value, 
/// but if the percision value is 2 or more times larger then the actual deviation of any "identical" points, 
/// it will produce consistant results
/// 
/// If points are spaced more uniformly, without large gaps, a data structure that maps points to
/// voxels would produce better results, and require less memory.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PointDictionary<T> : IEnumerable<KeyValuePair<Vector3, T>>
{
    /// <summary>
    /// The level of percision this dictionary operates under
    /// </summary>
    public readonly float percision;

    /// <summary>
    /// The lanes in the dictionary
    /// This dictionary operates by using binary search down the lanes
    /// </summary>
    PointLane<PointLane<PointLane<Vector3>>> lanes;

    /// <summary>
    /// The map from 3D points to the objects they map to
    /// NOTE TO SELF: Can't remember why the mapping was needed in the original design. It may not be needed in the current design.
    /// </summary>
    Dictionary<Vector3, T> mapping = new Dictionary<Vector3, T>();

    /// <summary>
    /// Create a new point dictionary with the required precision
    /// </summary>
    /// <param name="percision"></param>
    public PointDictionary(float percision = 0.0001f)
    {
        this.percision = percision;
        lanes = new PointLane<PointLane<PointLane<Vector3>>>(percision);
    }

    /// <summary>
    /// Add a new element to the dictionary
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    public void Add(Vector3 v, T t)
    {
        var xLane = lanes.GetOrInit(v.x, () => new PointLane<PointLane<Vector3>>(percision));
        var yLane = xLane.GetOrInit(v.y, () => new PointLane<Vector3>(percision));
        v = yLane.GetOrInit(v.z, () => v);
        mapping[v] = t;
    }

    /// <summary>
    /// Try to fetch the element out of the dictionary
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool TryGet(Vector3 v, out T t)
    {
        //This method works by searching the lanes
        if(TryGetX(lanes, v, out t))
        {
            return true;
        }
        else
        {
            t = default(T);
            return false;
        }
    }

    /// <summary>
    /// Get an element out of the dictionary
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public T Get(Vector3 v)
    {
        T t;
        if(TryGet(v, out t))
        {
            return t;
        }
        else
        {
            return default(T);
        }
    }

    /* Note about the algorithm to search lanes
     * There is a lot of repeated code between TryGetX and TryGetY.
     * Initially this algorithm was implemented recursively, however 
     * that implementation was very error prone, and challenging to debug given the tools available for C# development.
     * Normally I address problems like this by creating visualizations for the data structures,
     * but I couldn't come up with a decent way to visualize this.
     * 
     * Instead, given the small size of the problem (having at most 9 lanes to check) I determined that it would be faster
     * to reimplement this algorithm statically rather then attempting to debug the recurrsion.
     */

    private bool TryGetX(PointLane<PointLane<PointLane<Vector3>>> lane, Vector3 key, out T t)
    {
        //Get the sub lanes that match the x value
        PointLane<PointLane<Vector3>> left, center, right;
        (left, center, right) = lane.GetOrInitLanes(key.x, () => new PointLane<PointLane<Vector3>>(percision));
        //search down the sub lanes
        if(center != null && TryGetY(center, key, out t))
        {
            return true;
        }
        if(left != null && TryGetY(left, key, out t))
        {
            return true;
        }
        if(right != null && TryGetY(right, key, out t))
        {
            return true;
        }
        t = default(T);
        return false;
    }

    private bool TryGetY(PointLane<PointLane<Vector3>> lane, Vector3 key, out T t)
    {
        //search the sub lanes for sub sub lanes that match the y value
        PointLane<Vector3> left, center, right;
        (left, center, right) = lane.GetOrInitLanes(key.y, () => new PointLane<Vector3>(percision));
        //search down the sub lanes
        if(center != null && TryGetZ(center, key, out t))
        {
            return true;
        }
        if(left != null && TryGetZ(left, key, out t))
        {
            return true;
        }
        if(right != null && TryGetZ(right, key, out t))
        {
            return true;
        }
        t = default(T);
        return false;
    }

    private bool TryGetZ(PointLane<Vector3> lane, Vector3 key, out T t)
    {
        //search down the z lane for the coordianate
        Vector3 v;
        if(lane.TryGet(key.z, out v))
        {
            t = mapping[v];
            return true;
        }
        t = default(T);
        return false;
    }

    public IEnumerator<KeyValuePair<Vector3, T>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Vector3, T>>)mapping).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Vector3, T>>)mapping).GetEnumerator();
    }

    public class PointLane<U>
    {
        public List<FuzzyPoint> points = new List<FuzzyPoint>();
        public List<U> mapping = new List<U>();
        float percision;

        public PointLane(float percision)
        {
            this.percision = percision;
        }

        public U Get(float point)
        {
            FuzzyPoint p = new FuzzyPoint(point, percision);
            int index = points.BinarySearch(p);
            if(index >= 0)
            {
                return mapping[index];
            }
            return default(U);
        }

        public bool TryGet(float point, out U output)
        {
            FuzzyPoint p = new FuzzyPoint(point, percision);
            int index = points.BinarySearch(p);
            if(index >= 0)
            {
                output = mapping[index];
                return true;
            }
            output = default(U);
            return false;
        }

        public (U, U, U) GetLanes(float point)
        {
            FuzzyPoint p = new FuzzyPoint(point, percision);
            int index = points.BinarySearch(p);
            if(index >= 0)
            {
                return GetTuple(index);
            }
            else
            {
                return (default(U), default(U), default(U));
            }
        }

        public U GetOrInit(float point, Func<U> constructor)
        {
            FuzzyPoint p = new FuzzyPoint(point, percision);
            int index = points.BinarySearch(p);
            if(index < 0)
            {
                index = ~index;
                points.Insert(index, p);
                mapping.Insert(index, constructor());
            }
            return mapping[index];
        }

        public (U, U, U) GetOrInitLanes(float point, Func<U> constructor)
        {
            FuzzyPoint p = new FuzzyPoint(point, percision);
            int index = points.BinarySearch(p);
            if(index < 0)
            {
                index = ~index;
                points.Insert(index, p);
                mapping.Insert(index, constructor());
            }
            return GetTuple(index);
        }

        private (U, U, U) GetTuple(int index)
        {
            U left, center, right;
            center = mapping[index];
            var val = points[index];
            if(index > 0 && points[index - 1] == val)
            {
                left = mapping[index - 1];
            }
            else
            {
                left = default(U);
            }
            if(index < mapping.Count - 1 && points[index + 1] == val)
            {
                right = mapping[index + 1];
            }
            else
            {
                right = default(U);
            }
            return (left, center, right);
        }

        public void Set(float point, U value)
        {
            FuzzyPoint p = new FuzzyPoint(point, percision);
            int index = points.BinarySearch(p);
            if(index >= 0)
            {
                mapping[index] = value;
            }
            else
            {
                index = ~index;
                points.Insert(index, p);
                mapping.Insert(index, value);
            }
        }
    }
}