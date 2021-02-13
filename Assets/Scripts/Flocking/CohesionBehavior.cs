using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohesionBehavior : MonoBehaviour, IFlockBehavior
{
    public float CohesionWeight;
    public Vector3 GetDir(List<IFlockEntity> entities, IFlockEntity entity)
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < entities.Count; i++)
        {
            center += entities[i].Position;
        }
        center /= entities.Count;
        return (center - entity.Position).normalized * CohesionWeight;
    }
}
