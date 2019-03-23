using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyVector
{
    public readonly Vector3 point;
    public readonly float percision;
    private float percisionSquared;

    public FuzzyVector(Vector3 point, float percision)
    {
        if(percision <= 0)
        {
            throw new System.ArgumentException("Fuzzy vector cannot have a percision of 0 or less. If such behaviour is desired, use a floating point");
        }
        this.point = point;
        this.percision = percision;
        this.percisionSquared = percision * percision;
    }

    public static bool operator ==(FuzzyVector a, FuzzyVector b)
    {
        if((object)a == null)
        {
            return (object)b == null;
        }
        else if((object)b == null)
        {
            return false;
        }
        else
        {
            float percision = Mathf.Max(a.percisionSquared, b.percisionSquared);
            return (a.point - b.point).sqrMagnitude <= percision;
        }
    }

    public static bool operator !=(FuzzyVector a, FuzzyVector b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if(obj is FuzzyVector)
        {
            return (this == (FuzzyVector)obj);
        }
        else return ReferenceEquals(this, obj);
    }
}