using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A utility class for calculating matricies.
/// </summary>
public static class MatrixUtil
{
    public static Matrix4x4 GetChangeOfBase(Vector3 x, Vector3 y, Vector3 z, Vector3 u, Vector3 v, Vector3 w)
    {
        Matrix4x4 a = new Matrix4x4(x, y, z, new Vector4(0, 0, 0, 1));
        Matrix4x4 b = new Matrix4x4(y, v, w, new Vector4(0, 0, 0, 1));
        try
        {
            return a.inverse * b;
        }
        catch
        {
            return Matrix4x4.identity;
        }
    }

    public static Matrix4x4 CoordinateSpaceTransformation(Matrix4x4 a, Matrix4x4 b)
    {
        Vector3 oldPos = a.GetColumn(3);
        a.SetColumn(3, new Vector4(0, 0, 0, 1));
        Vector3 newPos = b.GetColumn(3);
        b.SetColumn(3, new Vector4(0, 0, 0, 1));
        a = a.inverse * b;
        a.SetColumn(3, newPos - oldPos);
        return a;
    }
}