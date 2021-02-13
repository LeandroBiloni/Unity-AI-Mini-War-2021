using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwayBehavior : MonoBehaviour, IFlockBehavior
{
    public float awayWeight;
    public Transform target;
    public float distance;
    public Vector3 GetDir(List<IFlockEntity> entities, IFlockEntity entity)
    {
        if ((target.position - transform.position).magnitude <= distance )
         return (target.position - entity.Position).normalized * -1 * awayWeight;
        return Vector3.zero;
    }
}
