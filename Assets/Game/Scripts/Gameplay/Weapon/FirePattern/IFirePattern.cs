using System.Collections.Generic;
using UnityEngine;

public interface IFirePattern
{
    IReadOnlyList<Vector2> GetDirections(Vector2 baseDirection);
    
}