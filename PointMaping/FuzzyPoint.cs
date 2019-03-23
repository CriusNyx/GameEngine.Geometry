using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a fuzzy floating point
/// A fuzzy point is a point where two fuzzy point that are very close together are considered equal
/// Fuzzy vectors are also comparable, allowing them take advantage of algorithms such as quick sort, and binary search.
/// </summary>
public class FuzzyPoint : IComparable<FuzzyPoint>
{
    public readonly float value;
    public readonly float percision;

    public FuzzyPoint(float value, float percision)
    {
        if(percision <= 0)
        {
            throw new System.ArgumentException("Fuzzy point cannot have a percision of 0 or less. If such behaviour is desired, use a floating point");
        }

        this.value = value;
        this.percision = percision;
    }

    public int CompareTo(FuzzyPoint other)
    {
        if(other == null)
        {
            return 1;
        }
        if(Mathf.Abs(this.value - other.value) < percision)
        {
            return 0;
        }
        else if(this.value < other.value) return -1;
        else return 1;
    }
}