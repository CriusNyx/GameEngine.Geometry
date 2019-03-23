using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic interface for a drawable object
/// </summary>
public interface IDrawable
{
    void Draw(float time = -1f);
    void Draw(Color color, float time = -1f);
}