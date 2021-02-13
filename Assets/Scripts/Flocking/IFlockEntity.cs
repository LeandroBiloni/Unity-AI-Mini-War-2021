using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlockEntity
{
    Vector3 Direction { get; }
    Vector3 Position { get; }
}
