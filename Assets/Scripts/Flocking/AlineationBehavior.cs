using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlineationBehavior : MonoBehaviour, IFlockBehavior
{
    public float alineationWeight;
    public Vector3 GetDir(List<IFlockEntity> entities, IFlockEntity entity)
    {
        Vector3 dir = Vector3.zero;
        for (int i = 0; i < entities.Count; i++)
        {
            dir += entities[i].Direction;
        }
        dir /= entities.Count;
        return dir.normalized * alineationWeight;
    }
}
